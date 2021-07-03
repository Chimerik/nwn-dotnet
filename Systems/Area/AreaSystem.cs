using NWN.API;
using NWN.API.Events;
using NWN.Services;
using System.Linq;
using NWN.Core.NWNX;
using NWN.Core;
using NLog;
using System;
using System.Numerics;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace NWN.Systems
{
  [ServiceBinding(typeof(AreaSystem))]
  partial class AreaSystem
  {
    private static readonly Logger Log = LogManager.GetCurrentClassLogger();
    public AreaSystem()
    {
      foreach(NwTrigger trigger in NwObject.FindObjectsWithTag("invi_unwalkable"))
      {
        trigger.OnEnter += OnEnterUnwalkableBlock;
      }

      foreach (NwArea area in NwModule.Instance.Areas)
      {
        area.OnEnter += OnAreaEnter;
        area.OnExit += OnAreaExit;

        DoAreaSpecificInitialisation(area);

          //Log.Info($"initializing area : {area.Name}");

        foreach (NwPlaceable coffre in area.Objects.Where(o => o.Tag == "loot_chest"))
        {
          coffre.Tag = coffre.GetLocalVariable<string>("_LOOT_REFERENCE").Value;
          //Log.Info($"initializing chest : {coffre.Name} with : {coffre.Tag}");
        }
      }
    }
    public static void OnAreaEnter(AreaEvents.OnEnter onEnter)
    {
      NwArea area = onEnter.Area;

      if (!PlayerSystem.Players.TryGetValue(onEnter.EnteringObject, out PlayerSystem.Player player)) //EN FONCTION DE SI LA ZONE EST REST OU PAS, ON AFFICHE LA PROGRESSION DU JOURNAL DE CRAFT
        return;

      Log.Info($"Map {area.Name} loaded in : {(DateTime.Now - player.mapLoadingTime).TotalSeconds}");

      AreaSpawner(area);

      if (player.menu.isOpen)
        player.menu.Close();

      if (area.GetLocalVariable<int>("_AREA_LEVEL") == 0)
      {
        if (player.craftJob.IsActive() && player.playerJournal.craftJobCountDown == null)
          player.craftJob.CreateCraftJournalEntry();
      }
      else if (player.playerJournal.craftJobCountDown != null)
        player.craftJob.CancelCraftJournalEntry();

      if (player.areaExplorationStateDictionnary.ContainsKey(area.Tag))
        player.oid.SetAreaExplorationState(area, player.areaExplorationStateDictionnary[area.Tag]);

      if (area.GetLocalVariable<int>("_AREA_LEVEL") < 2)
        player.oid.SetAreaExplorationState(area, true);

      foreach (NwCreature statue in area.FindObjectsOfTypeInArea<NwCreature>().Where(c => c.Tag == "Statuereptilienne"))
        statue.GetLocalVariable<int>($"_PERCEPTION_STATUS_{player.oid.CDKey}").Delete();
    }
    public static void OnAreaExit(AreaEvents.OnExit onExit)
    {
      if (!(onExit.ExitingObject is NwCreature creature))
        return;

      if (creature.IsPlayerControlled) // Cas normal de changement de zone
      {
        Log.Info($"{creature.ControllingPlayer.LoginCreature.Name} vient de quitter la zone {onExit.Area.Name}");

        if (!onExit.Area.FindObjectsOfTypeInArea<NwCreature>().Any(p => p.IsPlayerControlled))
          AreaCleaner(onExit.Area);

        if (!PlayerSystem.Players.TryGetValue(creature.ControllingPlayer.LoginCreature, out PlayerSystem.Player player))
          return;

        player.mapLoadingTime = DateTime.Now;

        player.previousLocation = player.location;

        if (!player.areaExplorationStateDictionnary.ContainsKey(onExit.Area.Tag))
          player.areaExplorationStateDictionnary.Add(onExit.Area.Tag, player.oid.GetAreaExplorationState(onExit.Area));
        else
          player.areaExplorationStateDictionnary[onExit.Area.Tag] = player.oid.GetAreaExplorationState(onExit.Area);
      }
      else // Edge case où le joueur se déconnecte
      {
        if (!PlayerSystem.Players.TryGetValue(onExit.ExitingObject, out PlayerSystem.Player player))
          return;

        Log.Info($"{player.oid.PlayerName} vient de quitter la zone {onExit.Area.Name} en se déconnectant.");

        if (!onExit.Area.FindObjectsOfTypeInArea<NwCreature>().Any(p => p.IsPlayerControlled))
          AreaCleaner(onExit.Area);
      }
    }
    public static void OnPersonnalStorageAreaExit(AreaEvents.OnExit onExit)
    {
      if (!PlayerSystem.Players.TryGetValue(onExit.ExitingObject, out PlayerSystem.Player player))
        return;

      Log.Info($"{player.oid.LoginCreature.Name} exited area {onExit.Area.Name}");

      if (!onExit.Area.FindObjectsOfTypeInArea<NwCreature>().Any(p => p.IsPlayerControlled))
        AreaDestroyer(onExit.Area);
    }
    public static void OnIntroAreaExit(AreaEvents.OnExit onExit)
    {
      if (!(onExit.ExitingObject is NwCreature oPC) || !oPC.IsPlayerControlled || onExit.Area.Tag != $"entry_scene_{oPC.ControllingPlayer.CDKey}")
        return;

      NWScript.WriteTimestampedLogEntry($"{oPC.Name} exited area {onExit.Area.Name}");
      AreaDestroyer(onExit.Area);
    }
    private void DoAreaSpecificInitialisation(NwArea area)
    {
      switch (area.Tag)
      {
        case "entry_scene":

          VisibilityPlugin.SetVisibilityOverride(NWScript.OBJECT_INVALID, NwObject.FindObjectsWithTag("intro_brouillard").FirstOrDefault(), VisibilityPlugin.NWNX_VISIBILITY_HIDDEN);
          area.GetLocalVariable<int>("_AREA_LEVEL").Value = 0;
          area.SetAreaWind(new Vector3(1, 0, 0), 4, 0, 0);

          break;
        case "SimilisseThetreSalledeSpectacle":

          NwTrigger trigger = area.FindObjectsOfTypeInArea<NwTrigger>().FirstOrDefault(t => t.Tag == "theater_scene");
          trigger.OnEnter += OnTheaterSceneEnter;
          trigger.OnExit += OnTheaterSceneExit;
          area.GetLocalVariable<int>("_AREA_LEVEL").Value = 0;

          break;
        case "Gothictest":
        case "CoteSudLaCrique":
          area.SetAreaWind(new Vector3(0, 1, 0), 3, 0, 0);
          break;
        case "laplage":
          area.SetAreaWind(new Vector3(1, 0, 0), 2, 0, 0);
          break;
        case "leschamps":
          area.SetAreaWind(new Vector3(-1, 0, 0), 1, 0, 0);
          break;
        case "Promenadetest":
        case "Governmenttest":
          area.SetAreaWind(new Vector3(1, 0, 0), 4, 0, 0);
          area.GetLocalVariable<int>("_AREA_LEVEL").Value = 0;
          break; 
        case "PalaceGardenTest":
          area.SetAreaWind(new Vector3(1, -1, 0), 2, 0, 0);
          area.GetLocalVariable<int>("_AREA_LEVEL").Value = 0;
          break;
        case "SimilisseTransitionPromenadeport":
        case "similissetempledistrict":
          area.SetAreaWind(new Vector3(0, -1, 0), 3, 0, 0);
          area.GetLocalVariable<int>("_AREA_LEVEL").Value = 0;
          break;
        case "SIMILISCITYGATE":
        case "Similiscityentrepot":
        case "Similisse":
        case "SimilisseQuartierdelaPromenadeAt":
        case "entrepotpersonnel":
        case "Forge":
        case "SimilisseQuartierdelaPromenadeTa":
        case "similisseslums":
        case "SIMILISSE_BIBLIOTHEQUE":
        case "Dispensaire":
        case "couronnedecuivre":
        case "SIMILISSE_THERMES":
        case "SimilisseQuartierduGouvernementP":
        case "ChateauRepoduction":
        case "SimilisseTribunalBureaudesAvocat":
        case "SimilisseTribunal":
        case "SimilisseTribunalPrison":
        case "SimilisseSalleDesDelibrations":
        case "Sawmill":
        case "similissebanque":
        case "tannery":
        case "qg_marten":
        case "ToursdesInventeurs":
        case "SIMILISPALAISNOU":
        case "qg_kathra":
          area.GetLocalVariable<int>("_AREA_LEVEL").Value = 0;
          break;
        case "cave_flooded":
          area.SetAreaWind(new Vector3(0, 1, 0), 8, 0, 0);
          area.GetLocalVariable<int>("_AREA_LEVEL").Value = 2;
          break;
        case "lepontdaruthen":
        case "Fermesnord":
        case "fermes_ouest":
        case "terres_de_fryar":
        case "vallee":
        case "cave_uw_ruins_entry":
          area.GetLocalVariable<int>("_AREA_LEVEL").Value = 2;
          break;
        case "chemin_interdit":
        case "collines_mugissantes":
        case "basse_montagne":
        case "haute_montagne":
        case "GoblinTunnels":
        case "caverne_kobolts":
          area.GetLocalVariable<int>("_AREA_LEVEL").Value = 3;
          break;
        case "epine_seeksa":
        case "OrcEncampment":
        case "vallee_caverne":
        case "cave_kuotoa":
          area.GetLocalVariable<int>("_AREA_LEVEL").Value = 4;
          break;
        case "SaltMines":
          area.GetLocalVariable<int>("_AREA_LEVEL").Value = 5;
          break;
        case "ant_nest":
          area.GetLocalVariable<int>("_AREA_LEVEL").Value = 6;
          break;
        default:
          area.GetLocalVariable<int>("_AREA_LEVEL").Value = 1;
          break;
      }
    }
    private static void OnTheaterSceneEnter(TriggerEvents.OnEnter onEnter)
    {
      onEnter.EnteringObject.VisualTransform.Translation = new Vector3(onEnter.EnteringObject.VisualTransform.Translation.X, 
        onEnter.EnteringObject.VisualTransform.Translation.Y, 2.01f);
      NWScript.SetCameraHeight(onEnter.EnteringObject, 1 + 2.01f);
    }

    private static void OnTheaterSceneExit(TriggerEvents.OnExit onExit)
    {
      if (onExit.ExitingObject == null)
      {
        Log.Info("Exiting object was null");
      }
      else
      {
        onExit.ExitingObject.VisualTransform.Translation = new Vector3(onExit.ExitingObject.VisualTransform.Translation.X, 
          onExit.ExitingObject.VisualTransform.Translation.Y, 0);
        NWScript.SetCameraHeight(onExit.ExitingObject, 0);
      }
    }
    public static NwArea CreatePersonnalStorageArea(NwCreature oPC, int characterId)
    {
      Log.Info($"Creating personnal storage area for : {oPC.Name} ID : {characterId}");

      NwArea area = NwArea.Create("entrepotperso", $"entrepotpersonnel_{oPC.ControllingPlayer.CDKey}", $"Entrepot dimensionnel de {oPC.ControllingPlayer.LoginCreature.Name}");
      area.GetLocalVariable<int>("_AREA_LEVEL").Value = 0;
      area.OnExit += OnPersonnalStorageAreaExit;

      NwPlaceable storage = area.FindObjectsOfTypeInArea<NwPlaceable>().FirstOrDefault(s => s.Tag == "ps_entrepot");
      storage.OnUsed += PlaceableSystem.OnUsedPersonnalStorage;
      storage.Name = $"Entrepôt de {oPC.ControllingPlayer.LoginCreature.Name}";

      var result = SqLiteUtils.SelectQuery("playerCharacters",
        new List<string>() { { "storage" } },
        new List<string[]>() { new string[] { "rowid", characterId.ToString() } });

      if (result != null && result.Count() > 0)
        NwStore.Deserialize(result.FirstOrDefault().GetString(0).ToByteArray()).Location = storage.Location;

      area.FindObjectsOfTypeInArea<NwPlaceable>().FirstOrDefault(p => p.Tag == "portal_storage_out").OnUsed += PlaceableSystem.OnUsedStoragePortalOut;
      area.FindObjectsOfTypeInArea<NwPlaceable>().FirstOrDefault(p => p.Tag == "hventes").OnUsed += DialogSystem.StartAuctionHouseDialog;
      area.FindObjectsOfTypeInArea<NwCreature>().FirstOrDefault(p => p.Tag == "bal_system").OnConversation += DialogSystem.StartStorageDialog;

      return area;
    }
    private async void OnEnterUnwalkableBlock(TriggerEvents.OnEnter onEnter)
    {
      Log.Info($"onEnter : {onEnter.EnteringObject.Name}");

      if (!(onEnter.EnteringObject is NwCreature oEntering))
        return;

      Vector3 initialPosition = oEntering.Position;
      oEntering.Commandable = false;

      await NwTask.Delay(TimeSpan.FromSeconds(0.8));
      await oEntering.ClearActionQueue();

      Vector3 calculations = 2 * (initialPosition - oEntering.Position);
      Vector3 kickback = initialPosition;
      kickback.Z = oEntering.Position.Z;

      NwPlaceable.Create("wall_invi", API.Location.Create(oEntering.Area, initialPosition, oEntering.Rotation));

      oEntering.Commandable = true;
      oEntering.Location = API.Location.Create(oEntering.Area, kickback + (initialPosition - oEntering.Position), oEntering.Rotation);
    }
  }
}
