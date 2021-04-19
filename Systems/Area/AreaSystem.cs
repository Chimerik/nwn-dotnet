using NWN.API;
using NWN.API.Events;
using NWN.Services;
using System.Linq;
using NWN.Core.NWNX;
using NWN.Core;
using NLog;

namespace NWN.Systems
{
  [ServiceBinding(typeof(AreaSystem))]
  partial class AreaSystem
  {
    private static readonly Logger Log = LogManager.GetCurrentClassLogger();
    public AreaSystem()
    {
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
      NwGameObject oEntered = onEnter.EnteringObject;

      if (!(oEntered is NwPlayer))
        return;

      NwPlayer oPC = (NwPlayer)oEntered;

      if (oPC.IsDM || oPC.IsDMPossessed || oPC.IsPlayerDM)
        return;

      AreaSpawner(area);

      if (PlayerSystem.Players.TryGetValue(oPC, out PlayerSystem.Player player)) //EN FONCTION DE SI LA ZONE EST REST OU PAS, ON AFFICHE LA PROGRESSION DU JOURNAL DE CRAFT
      {
        if (player.menu.isOpen)
          player.menu.Close();

        if (area.GetLocalVariable<int>("_AREA_LEVEL") < 2)
          NWScript.ExploreAreaForPlayer(area, player.oid, 1);

        if (area.GetLocalVariable<int>("_AREA_LEVEL") == 0)
        {
          if (player.craftJob.IsActive() && player.playerJournal.craftJobCountDown == null)
            player.craftJob.CreateCraftJournalEntry();
        }
        else if (player.playerJournal.craftJobCountDown != null)
          player.craftJob.CancelCraftJournalEntry();

        if (player.areaExplorationStateDictionnary.ContainsKey(onEnter.Area.Tag))
          PlayerPlugin.SetAreaExplorationState(onEnter.EnteringObject, onEnter.Area, player.areaExplorationStateDictionnary[onEnter.Area.Tag]);
      }

      foreach (NwCreature statue in area.FindObjectsOfTypeInArea<NwCreature>().Where(c => c.Tag == "Statuereptilienne"))
        statue.GetLocalVariable<int>($"_PERCEPTION_STATUS_{oPC.CDKey}").Delete();
    }
    public static void OnAreaExit(AreaEvents.OnExit onExit)
    {
      if (!PlayerSystem.Players.TryGetValue(onExit.ExitingObject, out PlayerSystem.Player player))
        return;
      
      if (player.oid.IsDM)
        return;

      Log.Info($"{player.oid.Name} vient de quitter la zone {onExit.Area.Name}");

      player.previousLocation = player.location;

      if (onExit.Area.FindObjectsOfTypeInArea<NwPlayer>().Count(p => !p.IsDM) < 1)
        AreaCleaner(onExit.Area);

      if (onExit.IsDisconnectingPlayer)
        return;

      if (!player.areaExplorationStateDictionnary.ContainsKey(onExit.Area.Tag))
        player.areaExplorationStateDictionnary.Add(onExit.Area.Tag, PlayerPlugin.GetAreaExplorationState(player.oid, onExit.Area));
      else
        player.areaExplorationStateDictionnary[onExit.Area.Tag] = PlayerPlugin.GetAreaExplorationState(player.oid, onExit.Area);
    }
    public static void OnPersonnalStorageAreaExit(AreaEvents.OnExit onExit)
    {
      if (!PlayerSystem.Players.TryGetValue(onExit.ExitingObject, out PlayerSystem.Player player))
        return;

      Log.Info($"{player.oid.Name} exited area {onExit.Area.Name}");

      if (onExit.Area.Tag == $"entrepotpersonnel_{player.oid.CDKey}")
      {
        NwStore storage = onExit.Area.FindObjectsOfTypeInArea<NwStore>().FirstOrDefault();

        if (!storage.IsValid)
        {
          Utils.LogMessageToDMs($"{onExit.Area.Name} - {player.oid.Name} - Le coffre personnel n'a pas pu être trouvé.");
          return;
        }

        var saveStorage = NWScript.SqlPrepareQueryCampaign(Config.database, $"UPDATE playerCharacters set storage = @storage where rowid = @characterId");
        NWScript.SqlBindInt(saveStorage, "@characterId", player.characterId);
        NWScript.SqlBindObject(saveStorage, "@storage", storage);
        NWScript.SqlStep(saveStorage);
        AreaDestroyer(onExit.Area);
      }
    }
    public static void OnIntroAreaExit(AreaEvents.OnExit onExit)
    {
      NwArea area = onExit.Area;
      NwGameObject oExited = onExit.ExitingObject;

      if (!(oExited is NwPlayer))
        return;

      NwPlayer oPC = (NwPlayer)oExited;

      NWScript.WriteTimestampedLogEntry($"{oPC.Name} exited area {area.Name}");

      if (oPC.IsDM)
        return;

      if (area.Tag == $"entry_scene_{oPC.CDKey}")
        AreaDestroyer(area);
    }
    private void DoAreaSpecificInitialisation(NwArea area)
    {
      switch (area.Tag)
      {
        case "entry_scene":
          foreach (NwObject fog in NwModule.FindObjectsWithTag("intro_brouillard"))
            VisibilityPlugin.SetVisibilityOverride(NWScript.OBJECT_INVALID, fog, VisibilityPlugin.NWNX_VISIBILITY_HIDDEN);

          area.GetLocalVariable<int>("_AREA_LEVEL").Value = 0;
          break;
        case "SimilisseThetreSalledeSpectacle":
          NwTrigger trigger = area.FindObjectsOfTypeInArea<NwTrigger>().FirstOrDefault(t => t.Tag == "theater_scene");
          trigger.OnEnter += OnTheaterSceneEnter;
          trigger.OnExit += OnTheaterSceneExit;
          area.GetLocalVariable<int>("_AREA_LEVEL").Value = 0;
          break;
        case "SIMILISCITYGATE":
        case "Similiscityentrepot":
        case "Similisse":
        case "Promenadetest":
        case "SimilisseQuartierdelaPromenadeAt":
        case "entrepotpersonnel":
        case "Forge":
        case "SimilisseQuartierdelaPromenadeTa":
        case "similisseslums":
        case "SIMILISSE_BIBLIOTHEQUE":
        case "Dispensaire":
        case "couronnedecuivre":
        case "similissetempledistrict":
        case "SIMILISSE_THERMES":
        case "Governmenttest":
        case "ChateauRepoduction":
        case "PalaceGardenTest":
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
        case "SimilisseQuartierduGouvernementP":
        case "qg_kathra":
          area.GetLocalVariable<int>("_AREA_LEVEL").Value = 0;
          break;
        case "lepontdaruthen":
        case "Fermesnord":
        case "fermes_ouest":
        case "terres_de_fryar":
        case "vallee":
        case "cave_flooded":
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
      VisualTransform transfo = onEnter.EnteringObject.VisualTransform;
      transfo.Translation.Z += 2.01f;
      onEnter.EnteringObject.VisualTransform = transfo;
    }

    private static void OnTheaterSceneExit(TriggerEvents.OnExit onExit)
    {
      if (onExit.ExitingObject == null)
      {
        Log.Info("Exiting object was null");
      }
      else
      {
        VisualTransform transfo = onExit.ExitingObject.VisualTransform;
        transfo.Translation.Z = 0.0f;
        onExit.ExitingObject.VisualTransform = transfo;
      }
    }
  }
}
