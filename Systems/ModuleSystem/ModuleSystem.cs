using Google.Cloud.Translation.V2;
using NLog;
using Anvil.API;
using Anvil.API.Events;
using NWN.Core.NWNX;
using Anvil.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using NWN.Core;
using Google.Apis.Drive.v3;
using Newtonsoft.Json;
using System.Numerics;
using System.IO;
using Microsoft.Data.Sqlite;

namespace NWN.Systems
{
  [ServiceBinding(typeof(ModuleSystem))]
  public class ModuleSystem
  {
    public static readonly Logger Log = LogManager.GetCurrentClassLogger();
    public static readonly TranslationClient googleTranslationClient = TranslationClient.Create();
    public static readonly DriveService googleDriveService = Config.AuthenticateServiceAccount();
    public static readonly Dictionary<string, GoldBalance> goldBalanceMonitoring = new();
    private readonly SchedulerService scheduler;

    public class HeadModels
    {
      public Gender gender { get; }
      public int appearanceRow { get; }
      public List<NuiComboEntry> heads { get; set; }

      public HeadModels(Gender gender, int appearanceRow)
      {
        this.gender = gender;
        this.appearanceRow = appearanceRow;
        heads = new List<NuiComboEntry>();
      }
    }

    public static readonly List<HeadModels> headModels = new();

    public ModuleSystem(SchedulerService schedulerService)
    {
      scheduler = schedulerService;

      LoadDiscordBot();
      CreateDatabase();
      InitializeEvents();

      SkillSystem.InitializeLearnables();
      LoadModulePalette();
      LoadRumors();
      LoadEditorNuiCombo();
      LoadCreatureSpawns();
      LoadPlaceableSpawns();
      LoadMailReceiverList();
      NwModule.Instance.OnModuleLoad += OnModuleLoad;
    }
    private static async void LoadDiscordBot()
    {
      try
      {
        await Bot.MainAsync();
      }
      catch (Exception e)
      {
        Log.Info($"Could not load discord bot : {e.Message} - {e.StackTrace}");
      }
    }
    private void OnModuleLoad(ModuleEvents.OnModuleLoad onModuleLoad)
    {
      NwModule.Instance.GetObjectVariable<LocalVariableString>("X2_S_UD_SPELLSCRIPT").Value = "spellhook";

      //NwModule.Instance.SetEventScript((EventScriptType)NWScript.EVENT_SCRIPT_MODULE_ON_PLAYER_TILE_ACTION, "on_tile_action");

      string serverName = "FR] Les Larmes des Erylies";
      NwServer.Instance.ServerInfo.ModuleName = "Demo technique ouverte";

      switch (Config.env)
      {
        case Config.Env.Bigby: 
          serverName = "FR] LDE - Bigby test server";
          NwServer.Instance.ServerInfo.ModuleName = "Recherche et developpement";
          break;
        case Config.Env.Chim: 
          serverName = "FR] LDE - Chim test server";
          NwServer.Instance.ServerInfo.ModuleName = "Recherche et developpement";
          break;
      }

      NwServer.Instance.ServerInfo.ServerName = serverName;
      
      NwServer.Instance.PlayerPassword = string.Empty;
      NwServer.Instance.DMPassword = string.IsNullOrEmpty(Environment.GetEnvironmentVariable("DMPASS")) ? "b" : Environment.GetEnvironmentVariable("DMPASS");
      NwServer.Instance.ServerInfo.PlayOptions.RestoreSpellUses = false;
      NwServer.Instance.ServerInfo.PlayOptions.ShowDMJoinMessage = false;

      ItemSystem.feedbackService.AddCombatLogMessageFilter(CombatLogMessage.ComplexAttack);

      SetModuleTime();

      foreach(NwItem item in NwObject.FindObjectsOfType<NwItem>()) // Permet de contrôler que les joueurs n'importent pas des items pétés en important des maps
      { // penser à la faire également lors de la création d'une zone dynamique
        item.Destroy();
        Utils.LogMessageToDMs($"OnModuleLoad - Destroyed item {item.Name} in area {item?.Area?.Name}");
      }

      RestorePlayerCorpseFromDatabase();
      RestoreResourceBlocksFromDatabase();
      LoadHeadLists();

      TimeSpan nextActivation = DateTime.Now.Hour < 5 ? new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 5, 0, 0) - DateTime.Now : DateTime.Now.AddDays(1).AddHours(-(DateTime.Now.Hour - 5)) - DateTime.Now;

      scheduler.ScheduleRepeating(HandlePlayerLoop, TimeSpan.FromSeconds(1));
      scheduler.ScheduleRepeating(HandleSaveDate, TimeSpan.FromMinutes(1));
      scheduler.ScheduleRepeating(HandleMateriaGrowth, TimeSpan.FromHours(1));
      scheduler.ScheduleRepeating(SpawnCollectableResources, TimeSpan.FromHours(24), nextActivation);
      scheduler.ScheduleRepeating(HandleSubscriptionDues, TimeSpan.FromHours(24), nextActivation);

      /*foreach (var duplicate in NwGameTables.PlaceableTable.GroupBy(p => p.ModelName).Where(p => p.Count() > 1).Select(p => p.Key))
      {
        Log.Info(duplicate);
        foreach (var plc in NwObject.FindObjectsOfType<NwPlaceable>().Where(p => p.Appearance.ModelName == duplicate))
          Log.Info($"{plc.Name} - rowID {plc.Appearance.RowIndex} - used in {plc.Area.Name}");
      }*/

      //foreach (var duplicate in NwGameTables.AppearanceTable.GroupBy(p => p.Race).Where(p => p.Count() > 1).Select(p => p.Key))
      //Log.Info(duplicate);

      /*Log.Info($"start");
      string[] files = Directory.GetFiles("/home/chim/checkres");
      foreach (string file in files)
      {
        string fileName = Path.GetFileName(file);
        string resName = fileName.Substring(0, fileName.LastIndexOf('.'));
        string extension = fileName.Substring(fileName.LastIndexOf('.') + 1);
        string resAlias = NWScript.ResManGetAliasFor(resName, Utils.GetResTypeFromFileExtension(extension, fileName));

        if (!string.IsNullOrEmpty(resAlias))
        {
          //File.Delete(file);
          Log.Info($"Found {resName}.{extension} in {resAlias}");
        }
      }
      Log.Info($"end");*/

      /*Dictionary<string, string> tilesets = new();

      foreach (NwArea area in NwModule.Instance.Areas)
        if (!tilesets.TryAdd(area.Tileset, area.Name))
          tilesets[area.Tileset] += $" | {area.Name}";

      foreach(var tile in tilesets)
        Log.Info($"{tile.Key} - {tile.Value}");*/

      /*foreach (NwArea area in NwModule.Instance.Areas)
        if (area.Name.Contains("toremove"))
          Log.Info($"{area.Name} - {area.Tileset} - {NWScript.ResManGetAliasFor(area.Tileset, NWScript.RESTYPE_SET)}");*/

      /*string[] array = new[] { "r1_plc561" };

      foreach(var mdl in array)
        Log.Info($";{mdl};{NWScript.ResManGetAliasFor(mdl, NWScript.RESTYPE_MDL)}");*/
    }
    private static async void ReadGDocLine()
    {
      var request = googleDriveService.Files.Export("1Q21R9JZdbajKK9S2F1pesHZo2Gh-zeO3LuXt7XAMJXY", "text/csv");
      List<string> resources = new();

      using var stream = new MemoryStream();
      await request.DownloadAsync(stream);

      stream.Position = 0;
      using(var reader = new StreamReader(stream))
      {
        string line;
        while ((line = reader.ReadLine()) != null)
          resources.Add(line);
      }

      await NwTask.SwitchToMainThread();

      List<PlaceableTableEntry> resourceList = new();

      foreach (var resource in resources)
        GetGameResourceTable(resource, resourceList);

      foreach(var resource in resourceList.OrderBy(r => NWScript.ResManGetAliasFor(r.ModelName, NWScript.RESTYPE_MDL)).ThenBy(r => r.ModelName).ThenBy(r => r.RowIndex).ThenBy(r => r.Label))
        Log.Info($"{resource.Label} | {resource.RowIndex} | {resource.ModelName} | {NWScript.ResManGetAliasFor(resource.ModelName, NWScript.RESTYPE_MDL)}");
    }
    private static void GetGameResourceTable(string resourceName, List<PlaceableTableEntry> resourceList)
    {
      var resources = NwGameTables.PlaceableTable.Where(r => r.Label == resourceName.Replace("\"", ""));

      if (resources == null || !resources.Any())
      {
        Log.Info($"--- {resourceName} absent du 2DA ---");
        return;
      }

      if(resources.Any())
      {
        Log.Info($"--- Label en doublon ---");

        foreach (var resource in resources)
          Log.Info($"{resourceName} | {resource.RowIndex} | {resource.ModelName} | {NWScript.ResManGetAliasFor(resource.ModelName, NWScript.RESTYPE_MDL)}");

        return;
      }

      foreach (var resource in resources)
        resourceList.Add(resource);
    }
    private static void CreateDatabase()
    {
      SqLiteUtils.CreateQuery("CREATE TABLE IF NOT EXISTS moduleInfo" +
        "('year' INTEGER NOT NULL, 'month' INTEGER NOT NULL, 'day' INTEGER NOT NULL, 'hour' INTEGER NOT NULL, 'minute' INTEGER NOT NULL, 'second' INTEGER NOT NULL)");

      SqLiteUtils.CreateQuery("CREATE TABLE IF NOT EXISTS PlayerAccounts" +
        "('accountName' TEXT NOT NULL, 'cdKey' TEXT, 'bonusRolePlay' INTEGER NOT NULL, 'discordId' TEXT, 'rank' TEXT, 'mapPins' TEXT, 'chatColors' TEXT, 'mutedPlayers' TEXT, 'windowRectangles' TEXT, 'customDMVisualEffects' TEXT, 'hideFromPlayerList' INTEGER NOT NULL DEFAULT 0)");

      SqLiteUtils.CreateQuery("CREATE TABLE IF NOT EXISTS playerCharacters" +
        "('accountId' INTEGER NOT NULL, 'characterName' TEXT NOT NULL, 'previousSPCalculation' TEXT, 'serializedLearnableSkills' TEXT, 'serializedLearnableSpells' TEXT," +
        "'location' TEXT, 'itemAppearances' TEXT, 'currentSkillPoints' INTEGER," +
        "'currentHP' INTEGER, 'bankGold' INTEGER, 'pveArenaCurrentPoints' INTEGER, 'menuOriginTop' INTEGER, 'menuOriginLeft' INTEGER, 'storage' TEXT, " +
        "'alchemyCauldron' TEXT, 'explorationState' TEXT, 'materialStorage' TEXT, 'craftJob' TEXT, 'grimoires' TEXT, 'quickbars' TEXT," +
        "'descriptions' TEXT, 'mails' TEXT, 'subscriptions' TEXT)");

      SqLiteUtils.CreateQuery("CREATE TABLE IF NOT EXISTS playerDeathCorpses" +
        "('characterId' INTEGER NOT NULL, 'deathCorpse' TEXT NOT NULL, 'location' TEXT NOT NULL)");

      SqLiteUtils.CreateQuery("CREATE TABLE IF NOT EXISTS loot_containers" +
        "('chestTag' TEXT NOT NULL, 'accountID' INTEGER NOT NULL, 'serializedChest' TEXT NOT NULL, 'position' TEXT NOT NULL, 'facing' REAL NOT NULL, PRIMARY KEY(chestTag))");

      SqLiteUtils.CreateQuery("CREATE TABLE IF NOT EXISTS areaResourceStock" +
        "('type' TEXT NOT NULL, 'quantity' INTEGER NOT NULL, 'grade' INTEGER NOT NULL, 'location' TEXT NOT NULL, UNIQUE (type, quantity, grade, location))");

      SqLiteUtils.CreateQuery("CREATE TABLE IF NOT EXISTS scriptPerformance" +
        "('script' TEXT NOT NULL, 'nbExecutions' INTEGER NOT NULL, 'averageExecutionTime' REAL NOT NULL, 'cumulatedExecutionTime' REAL NOT NULL, PRIMARY KEY(script))");

      SqLiteUtils.CreateQuery("CREATE TABLE IF NOT EXISTS goldBalance" +
        "('lootedTag' TEXT NOT NULL, 'nbTimesLooted' INTEGER NOT NULL, 'averageGold' INT NOT NULL, 'cumulatedGold' INT NOT NULL, PRIMARY KEY(lootedTag))");

      SqLiteUtils.CreateQuery("CREATE TABLE IF NOT EXISTS playerPrivateContracts" +
        "('characterId' INTEGER NOT NULL, 'expirationDate' TEXT NOT NULL, 'serializedContract' TEXT NOT NULL, 'totalValue' INTEGER NOT NULL)");

      SqLiteUtils.CreateQuery("CREATE TABLE IF NOT EXISTS playerBuyOrders" +
        "('characterId' INTEGER NOT NULL, 'expirationDate' TEXT NOT NULL, 'material' TEXT NOT NULL, 'quantity' INTEGER NOT NULL, 'unitPrice' INTEGER NOT NULL)");

      SqLiteUtils.CreateQuery("CREATE TABLE IF NOT EXISTS playerSellOrders" +
        "('characterId' INTEGER NOT NULL, 'expirationDate' TEXT NOT NULL, 'material' TEXT NOT NULL, 'quantity' INTEGER NOT NULL, 'unitPrice' INTEGER NOT NULL)");

      SqLiteUtils.CreateQuery("CREATE TABLE IF NOT EXISTS playerShops" +
        "('characterId' INTEGER NOT NULL, 'shop' TEXT NOT NULL, 'panel' TEXT NOT NULL, 'expirationDate' TEXT NOT NULL, 'areaTag' TEXT NOT NULL, 'position' TEXT NOT NULL, 'facing' REAL NOT NULL)");

      SqLiteUtils.CreateQuery("CREATE TABLE IF NOT EXISTS playerAuctions" +
        "('characterId' INTEGER NOT NULL, 'shop' TEXT NOT NULL, 'panel' TEXT NOT NULL, 'expirationDate' TEXT NOT NULL, 'highestAuction' INTEGER NOT NULL, 'highestAuctionner' INTEGER NOT NULL, 'areaTag' TEXT NOT NULL, 'position' TEXT NOT NULL, 'facing' REAL NOT NULL)");

      SqLiteUtils.CreateQuery("CREATE TABLE IF NOT EXISTS rumors" +
        "('rumors' TEXT DEFAULT NULL)");

      SqLiteUtils.CreateQuery("CREATE TABLE IF NOT EXISTS arenaRewardShop" +
        "('id' INTEGER NOT NULL, 'shop' TEXT NOT NULL, PRIMARY KEY(id))");

      SqLiteUtils.CreateQuery("CREATE TABLE IF NOT EXISTS messenger" +
        "('characterId' INTEGER NOT NULL, 'senderName' TEXT NOT NULL, 'title' TEXT NOT NULL, 'message', TEXT NOT NULL, 'sentDate' TEXT NOT NULL, 'read' INTEGER NOT NULL)");

      SqLiteUtils.CreateQuery("CREATE TABLE IF NOT EXISTS bankPlaceables" +
        "('id' INTEGER NOT NULL, 'areaTag' TEXT NOT NULL, 'ownerId' INTEGER NOT NULL, 'ownerName' TEXT NOT NULL, UNIQUE (id, areaTag))");

      SqLiteUtils.CreateQuery("CREATE TABLE IF NOT EXISTS modulePalette" +
        "('creatures' TEXT, 'placeables' TEXT, 'items' TEXT)");

      SqLiteUtils.CreateQuery("CREATE TABLE IF NOT EXISTS creatureSpawn " +
        "('areaTag' TEXT NOT NULL, 'position' TEXT NOT NULL, 'facing' REAL NOT NULL, 'serializedCreature' TEXT NOT NULL)");

      SqLiteUtils.CreateQuery("CREATE TABLE IF NOT EXISTS placeableSpawn " +
        "('areaTag' TEXT NOT NULL, 'position' TEXT NOT NULL, 'facing' REAL NOT NULL, 'serializedPlaceable' TEXT NOT NULL)");

      SqLiteUtils.CreateQuery("CREATE TABLE IF NOT EXISTS areaMusics" +
        "('areaTag' TEXT NOT NULL, 'backgroundDay' INTEGER NOT NULL, 'backgroundNight' INTEGER NOT NULL, 'battle' INTEGER NOT NULL, PRIMARY KEY(areaTag))");

      SqLiteUtils.CreateQuery("CREATE TABLE IF NOT EXISTS areaLoadScreens" +
        "('areaTag' TEXT NOT NULL, 'loadScreen' INTEGER NOT NULL, PRIMARY KEY(areaTag))");

      SqLiteUtils.CreateQuery("CREATE TABLE IF NOT EXISTS trade ('requests' TEXT, 'auctions' TEXT, 'buyOrders' TEXT, 'sellOrders' TEXT)");

      SqLiteUtils.CreateQuery("CREATE TABLE IF NOT EXISTS playerConnectionInfo" +
        "('playerAccount' TEXT NOT NULL, 'cdKey' TEXT NOT NULL, 'ipAdress' TEXT NOT NULL, 'lastConnection' TEXT NOT NULL, UNIQUE (playerAccount, cdKey, ipAdress))");
    }
    private void InitializeEvents()
    {
      //EventsPlugin.SubscribeEvent("NWNX_ON_DM_POSSESS_FULL_POWER_BEFORE", "b_dm_possess");
      //EventsPlugin.SubscribeEvent("NWNX_ON_DM_POSSESS_BEFORE", "b_dm_possess");

      EventsPlugin.SubscribeEvent("NWNX_ON_INPUT_EMOTE_BEFORE", "on_input_emote");
      EventsPlugin.SubscribeEvent("NWNX_ON_DECREMENT_SPELL_COUNT_BEFORE", "spell_dcr");

      //EventsPlugin.SubscribeEvent("NWNX_ON_HAS_FEAT_AFTER", "event_has_feat");

      NwModule.Instance.OnPlayerGuiEvent += PlayerSystem.HandleGuiEvents;
      NwModule.Instance.OnCreatureAttack += AttackSystem.HandleAttackEvent;
      NwModule.Instance.OnCreatureDamage += AttackSystem.HandleDamageEvent;
      NwModule.Instance.OnEffectApply += OnPlayerEffectApplied;
      NwModule.Instance.OnCreatureCheckProficiencies += OnCheckProficiencies;
    }

    private void OnCheckProficiencies(OnCreatureCheckProficiencies onCheck)
    {
      if (onCheck.Item != null && onCheck.Item.BaseItem != null && onCheck.Item.BaseItem.EquipmentSlots != EquipmentSlots.None)
        onCheck.ResultOverride = CheckProficiencyOverride.HasProficiency;
    }

    private static void SetModuleTime()
    {
      var query = SqLiteUtils.SelectQuery("moduleInfo",
        new List<string>() { { "year" }, { "month" }, { "day" }, { "hour" }, { "minute" }, { "second" } },
        new List<string[]>() { new string[] { "rowid", "1" } });

      if (query != null && query.Count > 0)
      {
        var result = query.FirstOrDefault();
        NwDateTime.Now = new NwDateTime(int.Parse(result[0]), int.Parse(result[1]), int.Parse(result[2]), int.Parse(result[3]), int.Parse(result[4]), int.Parse(result[5]));
      }
      else
      {
        SqLiteUtils.InsertQuery("moduleInfo",
          new List<string[]>() { new string[] { "year", NwDateTime.Now.Year.ToString() }, new string[] { "month", NwDateTime.Now.Month.ToString() }, new string[] { "day", NwDateTime.Now.DayInTenday.ToString() }, new string[] { "hour", NwDateTime.Now.Hour.ToString() }, new string[] { "minute", NwDateTime.Now.Minute.ToString() }, new string[] { "second", NwDateTime.Now.Second.ToString() } });
      }
    }
    private void HandleSaveDate()
    {
      SqLiteUtils.UpdateQuery("moduleInfo",
        new List<string[]>() { new string[] { "year", NwDateTime.Now.Year.ToString() }, { new string[] { "month", NwDateTime.Now.Month.ToString() } }, { new string[] { "day", NwDateTime.Now.DayInTenday.ToString() } }, { new string[] { "hour", NwDateTime.Now.Hour.ToString() } }, { new string[] { "minute", NwDateTime.Now.Minute.ToString() } }, { new string[] { "second", NwDateTime.Now.Second.ToString() } } },
        new List<string[]>() { new string[] { "ROWID", "1" } });
    }
    
    public static void RestorePlayerCorpseFromDatabase()
    {
      var query = SqLiteUtils.SelectQuery("playerDeathCorpses",
        new List<string>() { { "deathCorpse" }, { "location" }, { "characterId" } },
        new List<string[]>());

      foreach (var pcCorpse in query)
      {
        NwCreature corpse = NwCreature.Deserialize(pcCorpse[0].ToByteArray());
        corpse.Location = SqLiteUtils.DeserializeLocation(pcCorpse[1]);
        corpse.GetObjectVariable<LocalVariableInt>("_PC_ID").Value = int.Parse(pcCorpse[2]);

        foreach (NwItem item in corpse.Inventory.Items.Where(i => i.Tag != "item_pccorpse"))
          item.Destroy();

        PlayerSystem.SetupPCCorpse(corpse);
      }
    }
    private static void RestoreResourceBlocksFromDatabase()
    {
      var result = SqLiteUtils.SelectQuery("areaResourceStock",
          new List<string>() { { "type" }, { "quantity" }, { "grade" }, { "location" } },
          new List<string[]>() { });

      foreach (var resourceBlock in result)
      {
        Location materiaLocation = SqLiteUtils.DeserializeLocation(resourceBlock[3]);
        string resRef = resourceBlock[0];
        int quantity = int.Parse(resourceBlock[1]);
        int grade = int.Parse(resourceBlock[2]);
        NwPlaceable newResourceBlock = NwPlaceable.Create(resRef, materiaLocation, false, "mineable_materia");
        newResourceBlock.GetObjectVariable<LocalVariableInt>("_ORE_AMOUNT").Value = quantity;
        newResourceBlock.GetObjectVariable<LocalVariableInt>("_GRADE").Value = grade;

        Log.Info($"MATERIA SPAWN - Area {materiaLocation.Area.Name} - Spawning {resRef} - Quantity {grade} - grade {grade}");
      }
    }
    private async void HandleMateriaGrowth()
    {
      Dictionary<int, int> areaLevelUpdater = new();

      foreach (var materia in NwObject.FindObjectsWithTag<NwGameObject>("mineable_materia"))
      {
        int areaLevel = materia.Area.GetObjectVariable<LocalVariableInt>("_AREA_LEVEL").Value;
        int materiaGrade = materia.GetObjectVariable<LocalVariableInt>("_GRADE").Value;
        int resourceGrowth = (int)(Config.baseMateriaGrowth * (1.00 + (areaLevel - materiaGrade) * Config.baseMateriaGrowthMultiplier));
        int quantity = materia.GetObjectVariable<LocalVariableInt>("_ORE_AMOUNT").Value;
        string resRef = materia.ResRef;
        Location location = materia.Location;

        materia.GetObjectVariable<LocalVariableInt>("_ORE_AMOUNT").Value += resourceGrowth;

        if (areaLevelUpdater.TryAdd(areaLevel, areaLevel))
        {
          try
          {
            using (var connection = new SqliteConnection(Config.dbPath))
            {
              connection.Open();

              var sqlCommand = connection.CreateCommand();
              sqlCommand.CommandText = $"UPDATE areaResourceStock SET quantity = {resourceGrowth} " +
                                    $"WHERE type = '{resRef}' and quantity = {quantity} and grade = {materiaGrade} and location = '{SqLiteUtils.SerializeLocation(location)}' ";

              await sqlCommand.ExecuteNonQueryAsync();
            }
          }
          catch (Exception e)
          {
            Utils.LogMessageToDMs($"Update Query - Materia Block - {e.Message}");
          }
        }
      }
    }
    public static void SpawnCollectableResources()
    {
      Log.Info("Starting to spawn collectable ressources");

      foreach(NwArea area in NwModule.Instance.Areas)
      {
        int areaLevel = area.GetObjectVariable<LocalVariableInt>("_AREA_LEVEL").Value;
        if (areaLevel < 2)
          continue;

        int nbMateriaToSpawn = 12 - areaLevel - NwObject.FindObjectsWithTag<NwGameObject>("mineable_materia").Count(m => m.Area == area);
        Log.Info($"REFILL - {area.Name} - {nbMateriaToSpawn} can spawn");

        while (nbMateriaToSpawn > 0)
        {
          if (area.GetObjectVariable<LocalVariableInt>("_CAVE").HasValue)
            SpawnResourceBlock(area, areaLevel, "mineable_rock");

          if (area.GetObjectVariable<LocalVariableInt>("_WATER").HasValue)
            SpawnResourceBlock(area, areaLevel, "mineable_animal");

          if (area.GetObjectVariable<LocalVariableInt>("_FOREST").HasValue)
            SpawnResourceBlock(area, areaLevel, "mineable_tree");

          nbMateriaToSpawn -= 1;
        }
      }
    }
    private static async void SpawnResourceBlock(NwArea area, int areaLevel, string resourceTemplate)
    {
      if (NwRandom.Roll(Utils.random, 100) > Config.materiaSpawnChance)
        return;

      try
      {
        Location randomLocation = await Utils.GetRandomLocationInArea(area);
        await NwTask.SwitchToMainThread();

        if (randomLocation is null)
          return;

        int materiaGrade = Utils.GetSpawnedMateriaGrade(areaLevel);
        int resourceQuantity = (int)(Utils.random.NextDouble(Config.minMateriaSpawnYield, Config.maxMateriaSpawnYield) * (1.00 + (areaLevel - materiaGrade) * Config.baseMateriaGrowthMultiplier));
      
        NwPlaceable newResourceBlock = NwPlaceable.Create(resourceTemplate, randomLocation, false, "mineable_materia");
        newResourceBlock.GetObjectVariable<LocalVariableInt>("_ORE_AMOUNT").Value = resourceQuantity;
        newResourceBlock.GetObjectVariable<LocalVariableInt>("_GRADE").Value = materiaGrade;
        Utils.SetResourceBlockData(newResourceBlock);

        Log.Info($"MATERIA SPAWN - spawned {resourceTemplate} {materiaGrade} ({resourceQuantity}) in {area.Name}");

        await SqLiteUtils.InsertQueryAsync("areaResourceStock",
          new List<string[]>() { new string[] { "type", resourceTemplate }, new string[] { "quantity", resourceQuantity.ToString() }, new string[] { "grade", materiaGrade.ToString() }, new string[] { "location", SqLiteUtils.SerializeLocation(randomLocation) } },
          new List<string>() { "type", "quantity", "grade", "location" },
          new List<string[]>() { new string[] { "quantity" } });
      }
      catch (Exception)
      {
        Utils.LogMessageToDMs($"MATERIA SPAWN - could not spawn {resourceTemplate} in {area.Name}");
      }
    }
    public static async void HandleSubscriptionDues()
    {
      Log.Info("Handling player subscription dues");

      var query = await SqLiteUtils.SelectQueryAsync("playerCharacters",
        new List<string>() { { "ROWID" }, { "subscriptions" } },
        new List<string[]>());

      foreach (var character in query)
      {
        if (!int.TryParse(character[0], out int characterId) || string.IsNullOrEmpty(character[1]) || character[1] == "null")
          continue;

        List<Subscription.SerializableSubscription> serializedSubscription = JsonConvert.DeserializeObject<List<Subscription.SerializableSubscription>>(character[1]);

        foreach (var subscription in serializedSubscription)
          if (subscription.nextDueDate < DateTime.Now)
          {
            subscription.nextDueDate = DateTime.Now.AddDays(subscription.daysToNextDueDate);

            PlayerSystem.Player player = PlayerSystem.Players.FirstOrDefault(p => p.Value.characterId == characterId).Value;
            TradeSystem.UpdatePlayerBankAccount(characterId, -subscription.fee, "", "", $"Subscription to {subscription.type}");
          }

        SqLiteUtils.UpdateQuery("playerCharacters",
          new List<string[]>() { new string[] { "subscriptions", JsonConvert.SerializeObject(serializedSubscription) } },
          new List<string[]>() { new string[] { "ROWID", characterId.ToString() } });
      }
    }
    private static void LoadHeadLists()
    {
      /*Log.Info($"karandas found in {NWScript.ResManGetAliasFor("c_karandas", NWScript.RESTYPE_MDL)}");
      Log.Info($"c_envy found in {NWScript.ResManGetAliasFor("c_envy", NWScript.RESTYPE_MDL)}");
      Log.Info($"hp_drgtiamat_1 found in {NWScript.ResManGetAliasFor("hp_drgtiamat_1", NWScript.RESTYPE_MDL)}");*/

      for (int appearance = 0; appearance < 7; appearance++)
      {
        headModels.Add(new HeadModels(Gender.Male, appearance));
        headModels.Add(new HeadModels(Gender.Female, appearance));
      }
      for (int i = 1; i < 255; i++)
      {
        /*string search1 = $"iit_midmisc_{i.ToString().PadLeft(3, '0')}";
        string found = NWScript.ResManGetAliasFor(search1, NWScript.RESTYPE_TGA);
        Log.Info($"{search1} found : {found}");*/

        string search = i.ToString().PadLeft(3, '0');

        if (NWScript.ResManGetAliasFor($"pMD0_HEAD{search}", NWScript.RESTYPE_MDL) != "")
          headModels.FirstOrDefault(h => h.appearanceRow == (int)AppearanceType.Dwarf && h.gender == Gender.Male).heads.Add(new NuiComboEntry(i.ToString(), i));

        if (NWScript.ResManGetAliasFor($"pFD0_HEAD{search}", NWScript.RESTYPE_MDL) != "")
          headModels.FirstOrDefault(h => h.appearanceRow == (int)AppearanceType.Dwarf && h.gender == Gender.Female).heads.Add(new NuiComboEntry(i.ToString(), i));

        if (NWScript.ResManGetAliasFor($"pME0_HEAD{search}", NWScript.RESTYPE_MDL) != "")
          headModels.FirstOrDefault(h => h.appearanceRow == (int)AppearanceType.Elf && h.gender == Gender.Male).heads.Add(new NuiComboEntry(i.ToString(), i));

        if (NWScript.ResManGetAliasFor($"pFE0_HEAD{search}", NWScript.RESTYPE_MDL) != "")
          headModels.FirstOrDefault(h => h.appearanceRow == (int)AppearanceType.Elf && h.gender == Gender.Female).heads.Add(new NuiComboEntry(i.ToString(), i));

        if (NWScript.ResManGetAliasFor($"pMG0_HEAD{search}", NWScript.RESTYPE_MDL) != "")
          headModels.FirstOrDefault(h => h.appearanceRow == (int)AppearanceType.Gnome && h.gender == Gender.Male).heads.Add(new NuiComboEntry(i.ToString(), i));

        if (NWScript.ResManGetAliasFor($"pFG0_HEAD{search}", NWScript.RESTYPE_MDL) != "")
          headModels.FirstOrDefault(h => h.appearanceRow == (int)AppearanceType.Gnome && h.gender == Gender.Female).heads.Add(new NuiComboEntry(i.ToString(), i));

        if (NWScript.ResManGetAliasFor($"pMA0_HEAD{search}", NWScript.RESTYPE_MDL) != "")
          headModels.FirstOrDefault(h => h.appearanceRow == (int)AppearanceType.Halfling && h.gender == Gender.Male).heads.Add(new NuiComboEntry(i.ToString(), i));

        if (NWScript.ResManGetAliasFor($"pFA0_HEAD{search}", NWScript.RESTYPE_MDL) != "")
          headModels.FirstOrDefault(h => h.appearanceRow == (int)AppearanceType.Halfling && h.gender == Gender.Female).heads.Add(new NuiComboEntry(i.ToString(), i));

        if (NWScript.ResManGetAliasFor($"pMH0_HEAD{search}", NWScript.RESTYPE_MDL) != "")
        {
          headModels.FirstOrDefault(h => h.appearanceRow == (int)AppearanceType.Human && h.gender == Gender.Male).heads.Add(new NuiComboEntry(i.ToString(), i));
          headModels.FirstOrDefault(h => h.appearanceRow == (int)AppearanceType.HalfElf && h.gender == Gender.Male).heads.Add(new NuiComboEntry(i.ToString(), i));
        }

        if (NWScript.ResManGetAliasFor($"pFH0_HEAD{search}", NWScript.RESTYPE_MDL) != "")
        {
          headModels.FirstOrDefault(h => h.appearanceRow == (int)AppearanceType.Human && h.gender == Gender.Female).heads.Add(new NuiComboEntry(i.ToString(), i));
          headModels.FirstOrDefault(h => h.appearanceRow == (int)AppearanceType.HalfElf && h.gender == Gender.Female).heads.Add(new NuiComboEntry(i.ToString(), i));
        }

        if (NWScript.ResManGetAliasFor($"pMO0_HEAD{search}", NWScript.RESTYPE_MDL) != "")
          headModels.FirstOrDefault(h => h.appearanceRow == (int)AppearanceType.HalfOrc && h.gender == Gender.Male).heads.Add(new NuiComboEntry(i.ToString(), i));

        if (NWScript.ResManGetAliasFor($"pFO0_HEAD{search}", NWScript.RESTYPE_MDL) != "")
          headModels.FirstOrDefault(h => h.appearanceRow == (int)AppearanceType.HalfOrc && h.gender == Gender.Female).heads.Add(new NuiComboEntry(i.ToString(), i));
      }
    }
    private void OnPlayerEffectApplied(OnEffectApply effectApplied)
    {
      PlayerSystem.Player player = null;

      if (effectApplied.Effect.Spell == null || effectApplied.Effect.Creator == null
        || effectApplied.Effect.EffectType == EffectType.InvalidEffect || effectApplied.Effect.DurationType == EffectDuration.Instant)
        return;

      if (effectApplied.Effect.Creator is NwCreature caster)
        if (caster.IsPlayerControlled && !PlayerSystem.Players.TryGetValue(caster.ControllingPlayer.LoginCreature, out player))
          return;
        else if (caster.Master != null && caster.Master.IsPlayerControlled && !PlayerSystem.Players.TryGetValue(caster.Master, out player))
          return;

      if (player == null)
        return;

      effectApplied.Effect.Tag = $"_PLAYER_{player.characterId}";

      if (effectApplied.Object is NwGameObject gameObject && !player.effectTargets.Contains(gameObject))
        player.effectTargets.Add(gameObject);
    }
    private static void LoadEditorNuiCombo()
    {
      foreach (RacialType racialType in (RacialType[])Enum.GetValues(typeof(RacialType)))
        if (racialType != RacialType.Invalid && racialType != RacialType.All)
          Utils.raceList.Add(new NuiComboEntry(NwRace.FromRacialType(racialType).Name, (int)racialType));

      Utils.raceList = Utils.raceList.OrderBy(r => r.Label).ToList();

      foreach (Gender genderType in (Gender[])Enum.GetValues(typeof(Gender)))
      {
        switch (genderType)
        {
          case Gender.Male: Utils.genderList.Add(new NuiComboEntry("Masculin", (int)genderType)); break;
          case Gender.Female: Utils.genderList.Add(new NuiComboEntry("Féminin", (int)genderType)); break;
          case Gender.Both: Utils.genderList.Add(new NuiComboEntry("Les deux", (int)genderType)); break;
          case Gender.None: Utils.genderList.Add(new NuiComboEntry("Aucun", (int)genderType)); break;
          case Gender.Other: Utils.genderList.Add(new NuiComboEntry("Autre", (int)genderType)); break;
        }
      }
      
      foreach (var entry in SoundSet2da.soundSetTable)
        if (!string.IsNullOrEmpty(entry.resRef))
          Utils.soundSetList.Add(new NuiComboEntry(entry.label, entry.RowIndex));

      foreach (StandardFaction faction in (StandardFaction[])Enum.GetValues(typeof(StandardFaction)))
        Utils.factionList.Add(new NuiComboEntry(faction.ToString(), (int)faction));

      foreach (MovementRate movement in (MovementRate[])Enum.GetValues(typeof(MovementRate)))
        Utils.movementRateList.Add(new NuiComboEntry(movement.ToString(), (int)movement));

      for (int i = 0; i < 51; i++)
        Utils.sizeList.Add(new NuiComboEntry($"x{((float)(i + 75)) / 100}", i));

      for (int i = 0; i < 256; i++)
      {
        Utils.paletteColorBindings[i] = new NuiBind<string>($"color{i}");

        Utils.colorPaletteLeather.Add($"leather{i + 1}");
        Utils.colorPaletteMetal.Add(NWScript.ResManGetAliasFor($"metal{i + 1}", NWScript.RESTYPE_TGA) != "" ? $"metal{i + 1}" : $"leather{i + 1}");
      }

      foreach (var model in NwGameTables.AppearanceTable)
        if (!string.IsNullOrEmpty(model.Label))
          Utils.appearanceEntries.Add(new NuiComboEntry(StringUtils.ConvertToUTF8(model.Label)  , model.RowIndex));

      foreach (var model in NwGameTables.PlaceableTable)
        if (!string.IsNullOrEmpty(model.Label))
          Utils.placeableEntries.Add(new NuiComboEntry(model.Label, model.RowIndex));

      foreach (var baseItem in BaseItems2da.baseItemTable)
      {
        Dictionary<ItemAppearanceWeaponModel, List<NuiComboEntry>> models = new();

        List<NuiComboEntry> topEntries = new();
        List<NuiComboEntry> midEntries = new();
        List<NuiComboEntry> botEntries = new();

        string resRef = baseItem.resRef;

        for (byte i = 10; i < 255; i++)
        {
          string search = i.ToString().PadLeft(3, '0');

          if (NWScript.ResManGetAliasFor($"{resRef}_t_{search}", NWScript.RESTYPE_MDL) != "")
            topEntries.Add(new NuiComboEntry(i.ToString(), i));
          
          if (NWScript.ResManGetAliasFor($"{resRef}_m_{search}", NWScript.RESTYPE_MDL) != "")
            midEntries.Add(new NuiComboEntry(i.ToString(), i));

          if (NWScript.ResManGetAliasFor($"{resRef}_b_{search}", NWScript.RESTYPE_MDL) != "")
            botEntries.Add(new NuiComboEntry(i.ToString(), i));
        }

        models.Add(ItemAppearanceWeaponModel.Top, topEntries);
        models.Add(ItemAppearanceWeaponModel.Middle, midEntries);
        models.Add(ItemAppearanceWeaponModel.Bottom, botEntries);

        ItemUtils.weaponModelDictionary.Add((BaseItemType)baseItem.RowIndex, models);
      }

      int j = 0;

      foreach (CraftResource resource in Craft.Collect.System.craftResourceArray)
      {
        Utils.tradeMaterialList.Add(new NuiComboEntry(resource.name, j));
        j++;
      }
    }
    private static async void LoadMailReceiverList()
    {
      var results = await SqLiteUtils.SelectQueryAsync("bankPlaceables",
            new List<string>() { { "ownerId" }, { "ownerName" } },
            new List<string[]>() { });

      if (results != null && results.Count > 0)
        foreach (var result in results)
          if(int.TryParse(result[0], out int charId) && charId > -1)
            Utils.mailReceiverEntries.Add(new NuiComboEntry(result[1], charId));
    }
    private static void LoadModulePalette()
    {
      LoadCreaturePalette();
      LoadItemPalette();
      LoadPlaceablePalette();
    }
    private static async void LoadCreaturePalette()
    {
      var result = await SqLiteUtils.SelectQueryAsync("modulePalette",
            new List<string>() { { "creatures" } },
            new List<string[]>() { });

      if (result == null || result.Count < 1) 
      {
        await SqLiteUtils.InsertQueryAsync("modulePalette",
                  new List<string[]>() { new string[] { "creatures", "" } });

        return;
      }

      string serializedCreaturePalette = result.FirstOrDefault()[0];

      await Task.Run(() =>
      {
        if (string.IsNullOrEmpty(serializedCreaturePalette) || serializedCreaturePalette == "null")
          return;

        Utils.creaturePaletteList = JsonConvert.DeserializeObject<List<PaletteEntry>>(serializedCreaturePalette);
      });

      Utils.creaturePaletteCreatorsList.Add(new NuiComboEntry("Tous", 0));
      int index = 1;

      foreach (var entry in Utils.creaturePaletteList.DistinctBy(c => c.creator).OrderBy(c => c.creator))
      {
        Utils.creaturePaletteCreatorsList.Add(new NuiComboEntry(entry.creator, index));
        index++;
      }
    }
    private static async void LoadItemPalette()
    {
      var result = await SqLiteUtils.SelectQueryAsync("modulePalette",
            new List<string>() { { "items" } },
            new List<string[]>() { });
      
      if (result == null || result.Count < 1) 
      {
        await SqLiteUtils.InsertQueryAsync("modulePalette",
                  new List<string[]>() { new string[] { "items", "" } });

        return;
      }

      string serializedItemPalette = result.FirstOrDefault()[0];

      await Task.Run(() =>
      {
        if (string.IsNullOrEmpty(serializedItemPalette) || serializedItemPalette == "null")
          return;

        Utils.itemPaletteList = JsonConvert.DeserializeObject<List<PaletteEntry>>(serializedItemPalette);
      });

      Utils.itemPaletteCreatorsList.Add(new NuiComboEntry("Tous", 0));
      int index = 1;

      foreach (var entry in Utils.itemPaletteList.DistinctBy(c => c.creator).OrderBy(c => c.creator))
      {
        Utils.itemPaletteCreatorsList.Add(new NuiComboEntry(entry.creator, index));
        index++;
      }
    }
    private static async void LoadPlaceablePalette()
    {
      var result = await SqLiteUtils.SelectQueryAsync("modulePalette",
            new List<string>() { { "placeables" } },
            new List<string[]>() { });

      if (result == null || result.Count < 1)
      {
        await SqLiteUtils.InsertQueryAsync("modulePalette",
                  new List<string[]>() { new string[] { "placeables", "" } });

        return;
      }

      string serializedPlaceablePalette = result.FirstOrDefault()[0];

      await Task.Run(() =>
      {
        if (string.IsNullOrEmpty(serializedPlaceablePalette) || serializedPlaceablePalette == "null")
          return;

        Utils.placeablePaletteList = JsonConvert.DeserializeObject<List<PaletteEntry>>(serializedPlaceablePalette);
      });

      Utils.placeablePaletteCreatorsList.Add(new NuiComboEntry("Tous", 0));
      int index = 1;

      foreach (var entry in Utils.itemPaletteList.DistinctBy(c => c.creator).OrderBy(c => c.creator))
      {
        Utils.placeablePaletteCreatorsList.Add(new NuiComboEntry(entry.creator, index));
        index++;
      }
    }
    private static async void LoadRumors()
    {
      var result = await SqLiteUtils.SelectQueryAsync("rumors",
            new List<string>() { { "rumors" } },
            new List<string[]>() { });

      if (result == null || result.Count < 1)
      {
        await SqLiteUtils.InsertQueryAsync("rumors",
                  new List<string[]>() { new string[] { "rumors", "" } });

        return;
      }

      string serializedRumors = result.FirstOrDefault()[0];

      await Task.Run(() =>
      {
        if (string.IsNullOrEmpty(serializedRumors) || serializedRumors == "null")
          return;

        Utils.rumors = JsonConvert.DeserializeObject<List<Rumor>>(serializedRumors);
        Utils.rumors = Utils.rumors.OrderByDescending(r => r.id).ToList();
      });
    }
    private static void LoadCreatureSpawns()
    {
      var result = SqLiteUtils.SelectQuery("creatureSpawn",
            new List<string>() { { "areaTag" }, { "position" }, { "facing" }, { "serializedCreature" }, { "rowid" } },
            new List<string[]>() { });

      foreach (var spawn in result)
      {
        NwCreature creature = NwCreature.Deserialize(spawn[3].ToByteArray());
        creature.GetObjectVariable<LocalVariableInt>("_SPAWN_ID").Value = int.Parse(spawn[4]);
        creature.Location = Utils.GetLocationFromDatabase(spawn[0], spawn[1], float.Parse(spawn[2]));
      }
    }
    private static void LoadPlaceableSpawns()
    {
      var result = SqLiteUtils.SelectQuery("placeableSpawn",
            new List<string>() { { "areaTag" }, { "position" }, { "facing" }, { "serializedPlaceable" }, { "rowid" } },
            new List<string[]>() { });

      foreach (var spawn in result)
      {
        NwPlaceable plc = NwPlaceable.Deserialize(spawn[3].ToByteArray());
        plc.GetObjectVariable<LocalVariableInt>("_SPAWN_ID").Value = int.Parse(spawn[4]);
        plc.Location = Utils.GetLocationFromDatabase(spawn[0], spawn[1], float.Parse(spawn[2]));
      }
    }
    private void HandlePlayerLoop()
    {
      foreach (var player in PlayerSystem.Players.Values)
      {
        if (player.pcState != PlayerSystem.Player.PcState.Offline && player.oid != null && player.oid.IsConnected && player.oid.LoginCreature != null)
        {
          HandleJobLoop(player);
          HandleLearnableLoop(player);
          HandleCheckAfkStatus(player);
        }
      }
    }
    private static void HandleJobLoop(PlayerSystem.Player player)
    {
      if (player.craftJob != null && (player.oid.LoginCreature.Area != null || player.previousLocation != null))
      {
        int areaLevel = player.oid.LoginCreature.Area != null ? player.oid.LoginCreature.Area.GetObjectVariable<LocalVariableInt>("_AREA_LEVEL").Value : player.previousLocation.Area.GetObjectVariable<LocalVariableInt>("_AREA_LEVEL").Value;
        if (areaLevel < 1)
        {
          player.craftJob.remainingTime -= 1;
          if (player.TryGetOpenedWindow("activeCraftJob", out PlayerSystem.Player.PlayerWindow window) && window is PlayerSystem.Player.ActiveCraftJobWindow craftWindow)
            craftWindow.timeLeft.SetBindValue(player.oid, craftWindow.nuiToken.Token, player.craftJob.GetReadableJobCompletionTime());

          if (player.craftJob.remainingTime < 1)
          {
            if (player.craftJob.type != PlayerSystem.JobType.Invalid)
            {
              PlayerSystem.HandleSpecificJobCompletion[player.craftJob.type].Invoke(player, true);
              player.craftJob.HandleGenericJobCompletion(player);
            }
            else
              player.craftJob = null;
          }
        }
        else 
          player.craftJob.startTime = DateTime.Now;
      }
    }
    private static void HandleLearnableLoop(PlayerSystem.Player player)
    {
      if (player.activeLearnable != null && player.activeLearnable.active)
      {
        player.activeLearnable.acquiredPoints += player.GetSkillPointsPerSecond(player.activeLearnable);
        if (player.activeLearnable.acquiredPoints >= player.activeLearnable.pointsToNextLevel)
          player.activeLearnable.LevelUpWrapper(player);

        if (player.TryGetOpenedWindow("activeLearnable", out PlayerSystem.Player.PlayerWindow window) && window is PlayerSystem.Player.ActiveLearnableWindow learnableWindow)
          learnableWindow.timeLeft.SetBindValue(player.oid, learnableWindow.nuiToken.Token, player.activeLearnable.GetReadableTimeSpanToNextLevel(player));

        if (player.TryGetOpenedWindow("learnables", out PlayerSystem.Player.PlayerWindow menu) && menu is PlayerSystem.Player.LearnableWindow learnableMenu)
        {
          int listPosition = learnableMenu.currentList.ToList().IndexOf(player.activeLearnable);

          if (listPosition > -1)
          {
            List<string> time = learnableMenu.remainingTime.GetBindValues(player.oid, learnableMenu.nuiToken.Token);

            if (time != null)
            {
              time[listPosition] = player.activeLearnable.GetReadableTimeSpanToNextLevel(player);
              learnableMenu.remainingTime.SetBindValues(player.oid, learnableMenu.nuiToken.Token, time);
            }
          }
        }
      }
    }
    private static void HandleCheckAfkStatus(PlayerSystem.Player player)
    {
      DateTime lastActionDate = player.oid.LoginCreature.GetObjectVariable<DateTimeLocalVariable>("_LAST_ACTION_DATE").Value;
      double minutesSinceLastAction = (DateTime.Now - lastActionDate).TotalMinutes;

      if(player.pcState == PlayerSystem.Player.PcState.AFK)
      {
        if (player.oid.IsValid)
        {
          if (minutesSinceLastAction < 5.0)
          {
            player.pcState = PlayerSystem.Player.PcState.Online;

            foreach (Effect eff in player.oid.LoginCreature.ActiveEffects)
              if (eff.Tag == "EFFECT_VFX_AFK")
                player.oid.LoginCreature.RemoveEffect(eff);
          }
          else if (player.oid.LoginCreature.GetObjectVariable<LocalVariableLocation>("_AFK_LOCATION").HasValue)
          {
            Vector3 afkPos = player.oid.LoginCreature.GetObjectVariable<LocalVariableLocation>("_AFK_LOCATION").Value.Position;
            
            if(afkPos.X != player.oid.ControlledCreature.Location.Position.X || afkPos.Y != player.oid.ControlledCreature.Location.Position.Y || afkPos.Z != player.oid.ControlledCreature.Location.Position.Z)
              player.oid.LoginCreature.GetObjectVariable<DateTimeLocalVariable>("_LAST_ACTION_DATE").Value = DateTime.Now;
          }
        }
      }
      else if (player.pcState == PlayerSystem.Player.PcState.Online && player.oid.IsValid && minutesSinceLastAction > 5.0)
      {
        player.pcState = PlayerSystem.Player.PcState.AFK;
        Effect afkVXF = Effect.VisualEffect((VfxType)751);
        afkVXF.Tag = "EFFECT_VFX_AFK";
        afkVXF.SubType = EffectSubType.Supernatural;
        player.oid.LoginCreature.ApplyEffect(EffectDuration.Permanent, afkVXF);

        player.oid.LoginCreature.GetObjectVariable<LocalVariableLocation>("_AFK_LOCATION").Value = player.oid.ControlledCreature.Location;
      }
    }
  }
}
