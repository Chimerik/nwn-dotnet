using Anvil.API;
using Anvil.API.Events;
using Anvil.Services;
using System;
using System.Numerics;
using System.Collections.Generic;
using System.Linq;

namespace NWN.Systems
{
  [ServiceBinding(typeof(AreaSystem))]
  public partial class AreaSystem
  {
    public static readonly Dictionary<string, string> areaDescriptions = new();
    public static readonly List<string> areaDescriptionsToDownload = new();
    private static readonly int[] rockRandomAppearances = new int[] { 1603, 4480, 4481, 5266, 5267, 5268, 5269, 14669, 14670, 14671, 14672, 14673, 14674, 14675, 14676, 14677, 14678 };
    private readonly DialogSystem dialogSystem;
    private readonly ScriptHandleFactory scriptHandleFactory;
    private readonly ScriptCallbackHandle mobRegenIntervalHandle;
    private readonly ScriptCallbackHandle mobRunAway;
    private readonly SchedulerService scheduler;

    public AreaSystem(ModuleSystem moduleSystem, DialogSystem dialogSystem, ScriptHandleFactory scriptFactory, SchedulerService schedulerService)
    {
      this.dialogSystem = dialogSystem;
      scriptHandleFactory = scriptFactory;
      scheduler = schedulerService;
      mobRegenIntervalHandle = scriptHandleFactory.CreateUniqueHandler(onMobRegenInterval);
      mobRunAway = scriptHandleFactory.CreateUniqueHandler(HandleRunAwayFromPlayer);

      HandleSpecificAreaBehaviour();
    }
    private async void HandleSpecificAreaBehaviour()
    {
      await NwTask.NextFrame();

      LoadGenericSpawnAppearance();

      //foreach (NwTrigger trigger in NwObject.FindObjectsWithTag<NwTrigger>("invi_unwalkable"))
      //trigger.OnEnter += OnEnterUnwalkableBlock;

      var resultMusics = SqLiteUtils.SelectQuery("areaMusics",
        new List<string>() { { "areaTag" }, { "backgroundDay" }, { "backgroundNight" }, { "battle" } },
        new List<string[]>() { });

      Dictionary<string, int[]> areaMusics = new();
      Dictionary<string, int> areaLoadScreens = new();

      foreach (var area in resultMusics)
        areaMusics.TryAdd(area[0], new int[] { int.Parse(area[1]), int.Parse(area[2]), int.Parse(area[3]) });

      var resultLoadScreens = SqLiteUtils.SelectQuery("areaLoadScreens",
        new List<string>() { { "areaTag" }, { "loadScreen" } },
        new List<string[]>() { });

      foreach (var area in resultLoadScreens)
        areaLoadScreens.TryAdd(area[0], int.Parse(area[1]));

      foreach (NwArea area in NwModule.Instance.Areas)
      {
        area.OnEnter += OnAreaEnter;
        area.OnEnter += OnEnterApplyDrowLightSensitivity;
        area.OnExit += OnAreaExit;
        area.OnHeartbeat += OnAreaHeartbeat;
        area.RestingAllowed = false;
        areaDescriptionsToDownload.Add(area.Name);

        area.GetObjectVariable<LocalVariableInt>("X2_L_WILD_MAGIC").Value = 1;

        if (areaMusics.TryGetValue(area.Tag, out var music))
        {
          int[] musicTab = music;
          area.MusicBackgroundDayTrack = musicTab[0];
          area.MusicBackgroundNightTrack = musicTab[1];
          area.MusicBattleTrack = musicTab[2];
        }

        if (areaLoadScreens.TryGetValue(area.Tag, out var loadScreen))
          area.LoadScreen = NwGameTables.LoadScreenTable.GetRow(loadScreen);

        DoAreaSpecificInitialisation(area);

        //Log.Info($"initializing area : {area.Name}");
      }

      foreach (NwPlaceable coffre in NwObject.FindObjectsWithTag<NwPlaceable>("loot_chest"))
      {
        coffre.Tag = coffre.GetObjectVariable<LocalVariableString>("_LOOT_REFERENCE").Value;
        //Log.Info($"initializing chest : {coffre.Name} with : {coffre.Tag}");
      }

      List<NwCreature> creatureToSerialize = new();

      foreach (NwCreature creature in NwObject.FindObjectsOfType<NwCreature>())
      {
        if (creature.Tag == "dead_werat" || creature.Tag == "damage_trainer")
          continue;

        creatureToSerialize.Add(creature);
      }

      SerializeCreaturesAndCreateSpawn(creatureToSerialize);
      InitializeBankPlaceableNames();

      foreach (string areaName in areaDescriptionsToDownload)
        AreaUtils.LoadAreaDescription(areaName);
    }
    private static void SerializeCreaturesAndCreateSpawn(List<NwCreature> creatureList)
    {
      foreach (NwCreature creature in creatureList)
        CreatureUtils.HandleSpawnPointCreation(creature);
    }
    public void OnAreaHeartbeat(AreaEvents.OnHeartbeat onHB)
    {
      if (onHB.Area.PlayerCount > 0)
      {
        CheckSpawns(onHB.Area);

        if (onHB.Area.GetObjectVariable<DateTimeLocalVariable>("_CLEANING_TIME").HasValue) // Si date de nettoyage planifiée, supprimer la planification
        {
          onHB.Area.GetObjectVariable<DateTimeLocalVariable>("_CLEANING_TIME").Delete();
          LogUtils.LogMessage($"{onHB.Area.Name} canceling scheduled cleaning", LogUtils.LogType.AreaManagement);
        }
        else
          onHB.Area.GetObjectVariable<LocalVariableBool>("_CLEANING_TRIGGER").Value = true;
      }
      else
      {
        // Si pas de date de nettoyage planifiée, alors planifier une date de nettoyage
        if (onHB.Area.GetObjectVariable<DateTimeLocalVariable>("_CLEANING_TIME").HasNothing)
        {
          if (onHB.Area.GetObjectVariable<LocalVariableBool>("_CLEANING_TRIGGER").HasValue)
          {
            onHB.Area.GetObjectVariable<DateTimeLocalVariable>("_CLEANING_TIME").Value = DateTime.Now;
            LogUtils.LogMessage($"{onHB.Area.Name} scheduled for cleaning at {DateTime.Now.AddMinutes(25)}", LogUtils.LogType.AreaManagement);
          }
        }
        else if ((DateTime.Now - onHB.Area.GetObjectVariable<DateTimeLocalVariable>("_CLEANING_TIME").Value).TotalMinutes > 25) // Si date de nettoyage dépassée, alors nettoyer et supprimer la planification
        {
          LogUtils.LogMessage($"{onHB.Area.Name} cleaning area", LogUtils.LogType.AreaManagement);
          CleanArea(onHB.Area);
          onHB.Area.GetObjectVariable<DateTimeLocalVariable>("_CLEANING_TIME").Delete();
          onHB.Area.GetObjectVariable<LocalVariableBool>("_CLEANING_TRIGGER").Delete();
        }
      }
    }
    public void OnAreaEnter(AreaEvents.OnEnter onEnter)
    {
      NwArea area = onEnter.Area;

      if (onEnter.EnteringObject is NwCreature { IsPlayerControlled: false })
        return;

      if (NwModule.Instance.Players.Count(p => p.ControlledCreature != null && p.ControlledCreature.Area == area) == 1)
        CheckSpawns(area);

      if (!PlayerSystem.Players.TryGetValue(onEnter.EnteringObject, out PlayerSystem.Player player))
        return;

      LogUtils.LogMessage($"Map {area.Name} loaded in : {Math.Round((DateTime.Now - player.mapLoadingTime).TotalSeconds, 3, MidpointRounding.ToEven)} by {onEnter.EnteringObject.Name}", LogUtils.LogType.AreaManagement);

      player.location = player.oid.LoginCreature.Location;

      if (player.menu.isOpen)
        player.menu.Close();

      if (area.GetObjectVariable<LocalVariableInt>(AreaUtils.AreaLevelVariable) < 2)
        player.oid.SetAreaExplorationState(area, true);
      else if (player.areaExplorationStateDictionnary.TryGetValue(area.Tag, out var value))
        player.oid.SetAreaExplorationState(area, value);

      if (player.craftJob is not null && area.GetObjectVariable<LocalVariableInt>(AreaUtils.AreaLevelVariable).Value > 0 && player.TryGetOpenedWindow("activeCraftJob", out PlayerSystem.Player.PlayerWindow jobWindow))
        ((PlayerSystem.Player.ActiveCraftJobWindow)jobWindow).timeLeft.SetBindValue(player.oid, jobWindow.nuiToken.Token, "En pause (Hors Cité)");
    }
    public static void OnAreaExit(AreaEvents.OnExit onExit)
    {
      if (onExit.ExitingObject is not NwCreature creature)
        return;

      NwArea area = onExit.Area;

      if (creature.IsPlayerControlled) // Cas normal de changement de zone
      {
       LogUtils.LogMessage($"{creature.ControllingPlayer.LoginCreature.Name} vient de quitter la zone {area.Name}", LogUtils.LogType.AreaManagement);

        if (!PlayerSystem.Players.TryGetValue(creature, out PlayerSystem.Player player))
          return;

        CloseWindows(player);

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
      /*else // Edge case où le joueur se déconnecte
      {
        if (!PlayerSystem.Players.TryGetValue(onExit.ExitingObject, out PlayerSystem.Player player))
          return;

        Log.Info($"{player.oid.PlayerName} vient de quitter la zone {area.Name} en se déconnectant.");
      }*/

      //if (!NwModule.Instance.Players.Any(p => p.ControlledCreature != null && p.ControlledCreature.Area == area))
      //AreaCleaner(area);
    }
    private void DoAreaSpecificInitialisation(NwArea area)
    {
      switch (area.Tag)
      {
        case "Alphazone01":
        case "Alphazone02":
          area.GetObjectVariable<LocalVariableInt>("_AREA_LEVEL").Value = 0;
          break;

        case "entry_scene_out":
        case "entry_scene_in":

          NwObject.FindObjectsWithTag<NwPlaceable>("intro_brouillard").FirstOrDefault().VisibilityOverride = VisibilityMode.Hidden;
          area.GetObjectVariable<LocalVariableInt>("_AREA_LEVEL").Value = 0;
          area.SetAreaWind(new Vector3(1, 0, 0), 4, 0, 0);

          //ScheduleRockSpawn(area);

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
        case "Similisse":
        case "SimilisseQuartierdelaPromenadeTa":
          area.OnEnter += TaverneOnEnter;
          area.OnExit += TaverneOnExit;
          area.GetObjectVariable<LocalVariableInt>("_AREA_LEVEL").Value = 0;
          break;
        case "similissebanque":
          area.OnExit += BankOnExit;
          area.GetObjectVariable<LocalVariableInt>("_AREA_LEVEL").Value = 0;
          break;
        case "SIMILISCITYGATE":
        case "Similiscityentrepot":
        case "SimilisseQuartierdelaPromenadeAt":
        case "entrepotpersonnel":
        case "Forge":
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
        case "tannery":
        case "qg_marten":
        case "ToursdesInventeurs":
        case "SIMILISPALAISNOU":
        case "qg_kathra":
        case "alchemy":
        case "test":
        case "QuartierdesTemplesLesQuartiersde": area.GetObjectVariable<LocalVariableInt>("_AREA_LEVEL").Value = 0; break;
        case "cave_flooded":
          area.SetAreaWind(new Vector3(0, 1, 0), 8, 0, 0);
          area.GetObjectVariable<LocalVariableInt>("_AREA_LEVEL").Value = 2;
          area.GetObjectVariable<LocalVariableInt>("_CAVE").Value = 1;
          area.GetObjectVariable<LocalVariableInt>("_WATER").Value = 1;
          break;
        case "cave_uw_ruins_entry":
          area.GetObjectVariable<LocalVariableInt>("_AREA_LEVEL").Value = 2;
          area.GetObjectVariable<LocalVariableInt>("_CAVE").Value = 1;
          area.GetObjectVariable<LocalVariableInt>("_WATER").Value = 1;
          break;
        case "lepontdaruthen":
        case "Fermesnord":
        case "fermes_ouest":
        case "terres_de_fryar":
        case "vallee":
          area.GetObjectVariable<LocalVariableInt>("_AREA_LEVEL").Value = 2;
          area.GetObjectVariable<LocalVariableInt>("_FOREST").Value = 1;
          area.GetObjectVariable<LocalVariableInt>("_WATER").Value = 1;
          break;
        case "chemin_interdit":
          area.GetObjectVariable<LocalVariableInt>("_AREA_LEVEL").Value = 3;
          area.GetObjectVariable<LocalVariableInt>("_FOREST").Value = 1;
          area.GetObjectVariable<LocalVariableInt>("_WATER").Value = 1;
          break;
        case "collines_mugissantes":
        case "basse_montagne":
        case "haute_montagne":
          area.GetObjectVariable<LocalVariableInt>("_AREA_LEVEL").Value = 3;
          area.GetObjectVariable<LocalVariableInt>("_FOREST").Value = 1;
          break;
        case "GoblinTunnels":
        case "caverne_kobolts":
          area.GetObjectVariable<LocalVariableInt>("_AREA_LEVEL").Value = 3;
          area.GetObjectVariable<LocalVariableInt>("_CAVE").Value = 1;
          break;
        case "epine_seeksa":
        case "OrcEncampment":
          area.GetObjectVariable<LocalVariableInt>("_AREA_LEVEL").Value = 4;
          area.GetObjectVariable<LocalVariableInt>("_FOREST").Value = 1;
          break;
        case "vallee_caverne":
        case "cave_kuotoa":
          area.GetObjectVariable<LocalVariableInt>("_AREA_LEVEL").Value = 4;
          area.GetObjectVariable<LocalVariableInt>("_CAVE").Value = 1;
          area.GetObjectVariable<LocalVariableInt>("_WATER").Value = 1;
          break;
        case "AkkabanGothrasLair":
          area.GetObjectVariable<LocalVariableInt>("_AREA_LEVEL").Value = 5;
          area.GetObjectVariable<LocalVariableInt>("_CAVE").Value = 1;
          area.GetObjectVariable<LocalVariableInt>("_WATER").Value = 1;
          break;
        case "SaltMines":
          area.GetObjectVariable<LocalVariableInt>("_AREA_LEVEL").Value = 5;
          area.GetObjectVariable<LocalVariableInt>("_WATER").Value = 1;
          break;
        case "Senraad":
          area.GetObjectVariable<LocalVariableInt>("_AREA_LEVEL").Value = 5; break;
        case "ant_nest":
          area.GetObjectVariable<LocalVariableInt>("_AREA_LEVEL").Value = 6;
          area.GetObjectVariable<LocalVariableInt>("_WATER").Value = 1;
          break;
        default:
          area.GetObjectVariable<LocalVariableInt>("_AREA_LEVEL").Value = 1;
          break;
      }
    }
    private void TaverneOnEnter(AreaEvents.OnEnter onEnter)
    {
      if (onEnter.EnteringObject is NwCreature { IsPlayerControlled: true } oPC && PlayerSystem.Players.TryGetValue(oPC.ControllingPlayer.LoginCreature, out PlayerSystem.Player player))
      {
        if (!player.windows.ContainsKey("jukeBoxCurrentSong")) player.windows.Add("jukeBoxCurrentSong", new PlayerSystem.Player.JukeBoxCurrentSongWindow(player, onEnter.Area));
        else ((PlayerSystem.Player.JukeBoxCurrentSongWindow)player.windows["jukeBoxCurrentSong"]).CreateWindow(onEnter.Area);
      }
    }
    private void TaverneOnExit(AreaEvents.OnExit onExit)
    {
      if (onExit.ExitingObject is NwCreature { IsPlayerControlled: true } oPC && PlayerSystem.Players.TryGetValue(oPC.ControllingPlayer.LoginCreature, out PlayerSystem.Player player)
        && player.TryGetOpenedWindow("jukeBoxCurrentSong", out PlayerSystem.Player.PlayerWindow window))
        window.CloseWindow();
    }
    private void BankOnExit(AreaEvents.OnExit onExit)
    {
      if (onExit.ExitingObject is NwCreature { IsPlayerControlled: true } oPC && oPC.IsLoginPlayerCharacter && PlayerSystem.Players.TryGetValue(oPC, out PlayerSystem.Player player))
      {
        if(player.windows.ContainsKey("bankStorage"))
          ((PlayerSystem.Player.BankStorageWindow)player.windows["bankStorage"]).items = null;
      }
        
    }
    private void OnTheaterSceneEnter(TriggerEvents.OnEnter onEnter)
    {
      onEnter.EnteringObject.VisualTransform.Translation = new Vector3(onEnter.EnteringObject.VisualTransform.Translation.X,
        onEnter.EnteringObject.VisualTransform.Translation.Y, 2.01f);

      if (onEnter.EnteringObject is NwCreature { IsPlayerControlled: true } oPC)
        oPC.ControllingPlayer.CameraHeight = 1 + 2.01f;
    }
    private void OnTheaterSceneExit(TriggerEvents.OnExit onExit)
    {
      if (onExit.ExitingObject == null)
      {
        LogUtils.LogMessage("THEATER SCENE - Exiting object was null", LogUtils.LogType.AreaManagement);
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
      //Log.Info($"onEnter : {onEnter.EnteringObject.Name}");

      if (onEnter.EnteringObject is not NwCreature oEntering)
        return;

      Vector3 initialPosition = oEntering.Position;
      oEntering.Commandable = false;

      await NwTask.Delay(TimeSpan.FromSeconds(0.8));
      oEntering.ClearActionQueue();

      //Vector3 calculations = 2 * (initialPosition - oEntering.Position);
      Vector3 kickback = initialPosition;
      kickback.Z = oEntering.Position.Z;

      NwPlaceable.Create("wall_invi", Location.Create(oEntering.Area, initialPosition, oEntering.Rotation));

      oEntering.Commandable = true;
      oEntering.Location = Location.Create(oEntering.Area, kickback + (initialPosition - oEntering.Position), oEntering.Rotation);
    }
    private static void InitializeBankPlaceableNames()
    {
      try
      {
        var result = SqLiteUtils.SelectQuery("bankPlaceables",
          new List<string>() { { "id" }, { "areaTag" }, { "ownerId" }, { "ownerName" } },
          new List<string[]>() { });

        foreach (var bank in result)
        {
          NwObject bankPlaceable = NwObject.FindObjectsWithTag<NwPlaceable>("player_bank").FirstOrDefault(b => /*b.Area.Tag == bank.GetString(1) &&*/ b.GetObjectVariable<LocalVariableInt>("id").Value == int.Parse(bank[0]));
          bankPlaceable.GetObjectVariable<LocalVariableInt>("ownerId").Value = int.Parse(bank[2]);
          bankPlaceable.Name = bank[3];
        }
      }
      catch(Exception e)
      {
        Utils.LogMessageToDMs($"ERREUR - Impossible de charger les placeables de banque\n{e.Message}\n{e.StackTrace}");
      }
    }
    public void InitializeEventsAfterDMSpawnCreature(OnDMSpawnObject onSpawn)
    {
      if (onSpawn.SpawnedObject is not NwCreature creature)
        return;

      creature.OnPerception += CreatureUtils.OnMobPerception;
    }
    public static async void ScheduleRockSpawn(NwArea area, int side)
    {
      SpawnMovingRock(area, side);
      await NwTask.Delay(TimeSpan.FromSeconds(Utils.random.Next(5, 20)));

      if (area.GetObjectVariable<LocalVariableBool>("_STOP_INTRO_ROCK_SPAWN").HasValue)
        return;

      ScheduleRockSpawn(area, side);
    }
    private static async void SpawnMovingRock(NwArea area, int side)
    {
      NwPlaceable rock = NwPlaceable.Create("wall_invi", GetRandomRockSpawn(area, side));
      rock.VisibilityOverride = VisibilityMode.AlwaysVisible;

      int xTranslation = side < 1 ? Utils.random.Next(0, 30) : Utils.random.Next(-30, 0);

      rock.VisualTransform.Translation = new Vector3(xTranslation, -71, 0);
      rock.VisualTransform.Rotation = new Vector3(Utils.random.Next(0, 360), 0, 0);
      rock.VisualTransform.Scale = Utils.random.NextFloat(0.4f, 1.4f);
      rock.Appearance = NwGameTables.PlaceableTable.GetRow(rockRandomAppearances[Utils.random.Next(0, rockRandomAppearances.Length)]);
      rock.VisualTransform.Lerp(new VisualTransformLerpSettings { LerpType = VisualTransformLerpType.Linear, Duration = TimeSpan.FromSeconds(60), PauseWithGame = true }, transform => { transform.Translation = new Vector3(xTranslation, 120, 0); });
      //Log.Info($"new rock y : {rock.Position.Y} - visual translation x {xTranslation}");
      await NwTask.Delay(TimeSpan.FromSeconds(60));
      rock.Destroy();
    }
    private static Location GetRandomRockSpawn(NwArea area, int side)
    {
      int yPos = side < 1 ? Utils.random.Next(5, 12) : Utils.random.Next(57, 81);
      return Location.Create(area, new Vector3(36, yPos, 0), 0);
    }
    private void LoadGenericSpawnAppearance()
    {
      foreach (var appearance in NwGameTables.AppearanceTable)
      {
        switch (appearance.Name)
        {
          case "plage": randomAppearanceDictionary["plage"].Add(appearance); break;
          case "city": randomAppearanceDictionary["city"].Add(appearance); break;
          case "cave": randomAppearanceDictionary["cave"].Add(appearance); break;
          case "civilian": randomAppearanceDictionary["civilian"].Add(appearance); break;
          case "generic": randomAppearanceDictionary["generic"].Add(appearance); break;
        }
      }
    }
   public static void CloseWindows(PlayerSystem.Player player)
    {
      foreach(var window in player.windows)
      {
        if (!window.Value.IsOpen)
          continue;

        switch(window.Key)
        {
          case "areaMusicEditor":
          case "areaLoadScreenEditor":
          case "areaWindSettings":
          case "bankStorage":
          case "craftWorkshop":
          case "fishing":
          case "playerInput":
          case "jukebox":
          case "materiaDetector":
          case "materiaStorage":
          case "refinery":
          case "resourceExchange":
          case "rumors":
          case "auctionHouse":
          case "bankCounter":
            window.Value.CloseWindow();
            break;
        }
      }
    }
  }
}
