using Anvil.API;
using Anvil.API.Events;
using Anvil.Services;
using System.Linq;
using NLog;
using System;
using System.Numerics;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NWN.Systems
{
  [ServiceBinding(typeof(AreaSystem))]
  partial class AreaSystem
  {
    private static readonly Logger Log = LogManager.GetCurrentClassLogger();
    private DialogSystem dialogSystem;
    public AreaSystem(DialogSystem dialogSystem)
    {
      this.dialogSystem = dialogSystem;
      foreach (NwTrigger trigger in NwObject.FindObjectsWithTag("invi_unwalkable"))
        trigger.OnEnter += OnEnterUnwalkableBlock;

      foreach (NwArea area in NwModule.Instance.Areas)
      {
        area.OnEnter += OnAreaEnter;
        area.OnExit += OnAreaExit;
        
        DoAreaSpecificInitialisation(area);

        //Log.Info($"initializing area : {area.Name}");

        foreach (NwPlaceable coffre in area.Objects.Where(o => o.Tag == "loot_chest"))
        {
          coffre.Tag = coffre.GetObjectVariable<LocalVariableString>("_LOOT_REFERENCE").Value;
          //Log.Info($"initializing chest : {coffre.Name} with : {coffre.Tag}");
        }
      }

      foreach (NwCreature creature in NwObject.FindObjectsOfType<NwCreature>().Where(c => c.Tag != "dead_wererat" && c.Tag != "damage_trainer"))
      {
        Task waitLoopEnd = NwTask.Run(async () =>
        {
          await NwTask.Delay(TimeSpan.FromSeconds(0.2));
          NwWaypoint spawnPoint = NwWaypoint.Create("creature_spawn", creature.Location);
          spawnPoint.GetObjectVariable<LocalVariableString>("creature").Value = creature.Serialize().ToBase64EncodedString();
          creature.Destroy();
        });
      }

      InitializeBankPlaceableNames();
    }

    public void OnAreaEnter(AreaEvents.OnEnter onEnter)
    {
      NwArea area = onEnter.Area;

      if(onEnter.EnteringObject is NwCreature { IsPlayerControlled: false })
        return;
      
       if (NwModule.Instance.Players.Count(p => p.ControlledCreature?.Area == area) == 1)
        CreateSpawnChecker(area);

      if (!PlayerSystem.Players.TryGetValue(onEnter.EnteringObject, out PlayerSystem.Player player))
        return;

      Log.Info($"Map {area.Name} loaded in : {(DateTime.Now - player.mapLoadingTime).TotalSeconds}");

      player.location = player.oid.LoginCreature.Location;

      if (player.menu.isOpen)
        player.menu.Close();

      if (area.GetObjectVariable<LocalVariableInt>("_AREA_LEVEL") < 2)
        player.oid.SetAreaExplorationState(area, true);
      else if (player.areaExplorationStateDictionnary.ContainsKey(area.Tag))
        player.oid.SetAreaExplorationState(area, player.areaExplorationStateDictionnary[area.Tag]);

      Log.Info("area enter pc off");
    }
    public static void OnAreaExit(AreaEvents.OnExit onExit)
    {
      if (!(onExit.ExitingObject is NwCreature creature))
        return;

      NwArea area = onExit.Area;

      if (creature.IsPlayerControlled) // Cas normal de changement de zone
      {
        Log.Info($"{creature.ControllingPlayer.LoginCreature.Name} vient de quitter la zone {area.Name}");

        if (!PlayerSystem.Players.TryGetValue(creature.ControllingPlayer.LoginCreature, out PlayerSystem.Player player))
          return;

        player.mapLoadingTime = DateTime.Now;

        player.previousLocation = player.location;

        if (area.GetObjectVariable<LocalVariableInt>("_AREA_LEVEL") > 1)
        {
          if (!player.areaExplorationStateDictionnary.ContainsKey(onExit.Area.Tag))
            player.areaExplorationStateDictionnary.Add(area.Tag, player.oid.GetAreaExplorationState(area));
          else
            player.areaExplorationStateDictionnary[area.Tag] = player.oid.GetAreaExplorationState(area);
        }
      }
      else // Edge case où le joueur se déconnecte
      {
        if (!PlayerSystem.Players.TryGetValue(onExit.ExitingObject, out PlayerSystem.Player player))
          return;

        Log.Info($"{player.oid.PlayerName} vient de quitter la zone {area.Name} en se déconnectant.");
      }
      Log.Info("before cleaner");
      if (!NwModule.Instance.Players.Any(p => p.ControlledCreature?.Area == area))
        AreaCleaner(area);
      Log.Info("after cleaner");
    }
    public static void OnPersonnalStorageAreaExit(AreaEvents.OnExit onExit)
    {
      if (!PlayerSystem.Players.TryGetValue(onExit.ExitingObject, out PlayerSystem.Player player))
        return;

      Log.Info($"{player.oid.LoginCreature.Name} exited area {onExit.Area.Name}");

      if (!NwModule.Instance.Players.Any(p => p.ControlledCreature.Area == onExit.Area))
        AreaDestroyer(onExit.Area);
    }
    public static void OnIntroAreaExit(AreaEvents.OnExit onExit)
    {
      if (!(onExit.ExitingObject is NwCreature oPC) || !oPC.IsPlayerControlled || onExit.Area.Tag != $"entry_scene_{oPC.ControllingPlayer.CDKey}")
        return;

      Log.Info($"{oPC.Name} exited area {onExit.Area.Name}");
      AreaDestroyer(onExit.Area);
    }
    private void DoAreaSpecificInitialisation(NwArea area)
    {
      switch (area.Tag)
      {
        case "entry_scene":

          NwObject.FindObjectsWithTag<NwPlaceable>("intro_brouillard").FirstOrDefault().VisibilityOverride = VisibilityMode.Hidden;
          area.GetObjectVariable<LocalVariableInt>("_AREA_LEVEL").Value = 0;
          area.SetAreaWind(new Vector3(1, 0, 0), 4, 0, 0);

          break;
        case "SimilisseThetreSalledeSpectacle":

          NwTrigger trigger = area.FindObjectsOfTypeInArea<NwTrigger>().FirstOrDefault(t => t.Tag == "theater_scene");
          trigger.OnEnter += OnTheaterSceneEnter;
          trigger.OnExit += OnTheaterSceneExit;
          area.GetObjectVariable<LocalVariableInt>("_AREA_LEVEL").Value = 0;

          break;
        case "Gothictest":
        case "CoteSudLaCrique":
          area.SetAreaWind(new Vector3(0, 1, 0), 3, 0, 0);
          area.GetObjectVariable<LocalVariableInt>("_AREA_LEVEL").Value = 1;
          break;
        case "laplage":
          area.SetAreaWind(new Vector3(1, 0, 0), 2, 0, 0);
          area.GetObjectVariable<LocalVariableInt>("_AREA_LEVEL").Value = 1;
          break;
        case "leschamps":
          area.SetAreaWind(new Vector3(-1, 0, 0), 1, 0, 0);
          area.GetObjectVariable<LocalVariableInt>("_AREA_LEVEL").Value = 1;
          break;
        case "Promenadetest":
        case "Governmenttest":
          area.SetAreaWind(new Vector3(1, 0, 0), 4, 0, 0);
          area.GetObjectVariable<LocalVariableInt>("_AREA_LEVEL").Value = 0;
          break; 
        case "PalaceGardenTest":
          area.SetAreaWind(new Vector3(1, -1, 0), 2, 0, 0);
          area.GetObjectVariable<LocalVariableInt>("_AREA_LEVEL").Value = 0;
          break;
        case "SimilisseTransitionPromenadeport":
        case "similissetempledistrict":
          area.SetAreaWind(new Vector3(0, -1, 0), 3, 0, 0);
          area.GetObjectVariable<LocalVariableInt>("_AREA_LEVEL").Value = 0;
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
        case "alchemy":
          area.GetObjectVariable<LocalVariableInt>("_AREA_LEVEL").Value = 0;
          break;
        case "cave_flooded":
          area.SetAreaWind(new Vector3(0, 1, 0), 8, 0, 0);
          area.GetObjectVariable<LocalVariableInt>("_AREA_LEVEL").Value = 2;
          break;
        case "lepontdaruthen":
        case "Fermesnord":
        case "fermes_ouest":
        case "terres_de_fryar":
        case "vallee":
        case "cave_uw_ruins_entry":
          area.GetObjectVariable<LocalVariableInt>("_AREA_LEVEL").Value = 2;
          break;
        case "chemin_interdit":
        case "collines_mugissantes":
        case "basse_montagne":
        case "haute_montagne":
        case "GoblinTunnels":
        case "caverne_kobolts":
          area.GetObjectVariable<LocalVariableInt>("_AREA_LEVEL").Value = 3;
          break;
        case "epine_seeksa":
        case "OrcEncampment":
        case "vallee_caverne":
        case "cave_kuotoa":
          area.GetObjectVariable<LocalVariableInt>("_AREA_LEVEL").Value = 4;
          break;
        case "SaltMines":
          area.GetObjectVariable<LocalVariableInt>("_AREA_LEVEL").Value = 5;
          break;
        case "ant_nest":
          area.GetObjectVariable<LocalVariableInt>("_AREA_LEVEL").Value = 6;
          break;
        default:
          area.GetObjectVariable<LocalVariableInt>("_AREA_LEVEL").Value = 1;
          break;
      }
    }
    private static void OnTheaterSceneEnter(TriggerEvents.OnEnter onEnter)
    {
      onEnter.EnteringObject.VisualTransform.Translation = new Vector3(onEnter.EnteringObject.VisualTransform.Translation.X, 
        onEnter.EnteringObject.VisualTransform.Translation.Y, 2.01f);

      if (onEnter.EnteringObject is NwCreature { IsPlayerControlled: true } oPC)
        oPC.ControllingPlayer.CameraHeight = 1 + 2.01f;
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

        if (onExit.ExitingObject is NwCreature { IsPlayerControlled: true } oPC)
          oPC.ControllingPlayer.CameraHeight = 0;
      }
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

      NwPlaceable.Create("wall_invi", Location.Create(oEntering.Area, initialPosition, oEntering.Rotation));

      oEntering.Commandable = true;
      oEntering.Location = Location.Create(oEntering.Area, kickback + (initialPosition - oEntering.Position), oEntering.Rotation);
    }
    private void InitializeBankPlaceableNames()
    {
      var result = SqLiteUtils.SelectQuery("bankPlaceables",
        new List<string>() { { "id" }, { "areaTag" }, { "ownerId" }, { "ownerName" } },
        new List<string[]>() { });

      foreach(var bank in result.Results)
      {
        NwObject bankPlaceable = NwObject.FindObjectsWithTag<NwPlaceable>("player_bank").FirstOrDefault(b => /*b.Area.Tag == bank.GetString(1) &&*/ b.GetObjectVariable<LocalVariableInt>("id").Value == bank.GetInt(0));
        bankPlaceable.GetObjectVariable<LocalVariableInt>("ownerId").Value = bank.GetInt(2);
        bankPlaceable.Name = bank.GetString(3);
      }
    }
  }
}
