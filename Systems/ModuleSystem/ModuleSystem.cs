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

      RestorePlayerCorpseFromDatabase();
      //RestorePlayerShopsFromDatabase();
      //RestorePlayerAuctionsFromDatabase();
      RestoreResourceBlocksFromDatabase();
      LoadHeadLists();

      TimeSpan nextActivation = DateTime.Now.Hour < 5 ? new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 5, 0, 0) - DateTime.Now : DateTime.Now.AddDays(1).AddHours(-(DateTime.Now.Hour - 5)) - DateTime.Now;

      scheduler.ScheduleRepeating(HandlePlayerLoop, TimeSpan.FromSeconds(1));
      scheduler.ScheduleRepeating(SaveGameDate, TimeSpan.FromMinutes(1));
      scheduler.ScheduleRepeating(SpawnCollectableResources, TimeSpan.FromHours(24), nextActivation);
      //scheduler.ScheduleRepeating(DeleteExpiredMail, TimeSpan.FromHours(24), nextActivation);

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
          Log.Info($"Found {resName} in {resAlias}");
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

      if (resources == null || resources.Count() < 1)
      {
        Log.Info($"--- {resourceName} absent du 2DA ---");
        return;
      }

      if(resources.Count() > 1)
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
        "'location' TEXT, 'itemAppearances' TEXT," +
        "'currentHP' INTEGER, 'bankGold' INTEGER, 'pveArenaCurrentPoints' INTEGER, 'menuOriginTop' INTEGER, 'menuOriginLeft' INTEGER, 'storage' TEXT, " +
        "'alchemyCauldron' TEXT, 'explorationState' TEXT, 'materialStorage' TEXT, 'craftJob' TEXT, 'grimoires' TEXT, 'quickbars' TEXT," +
        "'descriptions' TEXT)");

      SqLiteUtils.CreateQuery("CREATE TABLE IF NOT EXISTS playerDeathCorpses" +
        "('characterId' INTEGER NOT NULL, 'deathCorpse' TEXT NOT NULL, 'location' TEXT NOT NULL)");

      SqLiteUtils.CreateQuery("CREATE TABLE IF NOT EXISTS loot_containers" +
        "('chestTag' TEXT NOT NULL, 'accountID' INTEGER NOT NULL, 'serializedChest' TEXT NOT NULL, 'position' TEXT NOT NULL, 'facing' REAL NOT NULL, PRIMARY KEY(chestTag))");

      SqLiteUtils.CreateQuery("CREATE TABLE IF NOT EXISTS areaResourceStock" +
        "('id' INTEGER NOT NULL, 'areaTag' TEXT NOT NULL, 'type' TEXT NOT NULL, 'quantity' INTEGER NOT NULL, 'lastChecked' TEXT NOT NULL, UNIQUE (id, areaTag, type))");

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

      if (query != null)
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
    private void SaveGameDate()
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
    public static void RestorePlayerShopsFromDatabase()
    {
      // TODO : envoyer un mp discord + courrier aux joueurs 7 jours avant expiration + 1 jour avant expiration

      var query = SqLiteUtils.SelectQuery("playerShops",
        new List<string>() { { "shop" }, { "panel" }, { "characterId" }, { "rowid" }, { "expirationDate" }, { "areaTag" }, { "position" }, { "facing" } },
        new List<string[]>());

      foreach (var playerShop in query)
      {
        NwStore shop = SqLiteUtils.StoreSerializationFormatProtection(playerShop[0], Utils.GetLocationFromDatabase(playerShop[5], playerShop[6], float.Parse(playerShop[7])));
        NwPlaceable panel = SqLiteUtils.PlaceableSerializationFormatProtection(playerShop[1], Utils.GetLocationFromDatabase(playerShop[5], playerShop[6], float.Parse(playerShop[7])));
        shop.GetObjectVariable<LocalVariableInt>("_OWNER_ID").Value = int.Parse(playerShop[2]);
        shop.GetObjectVariable<LocalVariableInt>("_SHOP_ID").Value = int.Parse(playerShop[3]);
        panel.GetObjectVariable<LocalVariableInt>("_OWNER_ID").Value = int.Parse(playerShop[2]);
        panel.GetObjectVariable<LocalVariableInt>("_SHOP_ID").Value = int.Parse(playerShop[3]);
        double expirationTime = (DateTime.Now - DateTime.Parse(playerShop[4])).TotalDays;

        int ownerId = shop.GetObjectVariable<LocalVariableInt>("_OWNER_ID").Value;

        if (expirationTime < 0)
        {
          Utils.SendMailToPC(ownerId, "Hôtel des ventes de Similisse", "Expiration du certificat de votre échoppe",
            $"Cher Marchand, \n\n Nous sommes au regret de vous informer que votre échoppe {panel.Name} a expiré. Nos hommes n'étant plus en mesure de la protéger, il se peut qu'elle ait été pillée par des vandales de passage ! \n\n Nous vous enjoignons à renouveller au plus vite votre certificat auprès de nos service. \n\n Signé : Polpo");

          DeleteExpiredShop(ownerId);
          Utils.SendDiscordPMToPlayer(ownerId, $"Cher Marchand, \n\n Nous sommes au regret de vous informer que votre échoppe {panel.Name} a expiré. Nos hommes n'étant plus en mesure de la protéger, il se peut qu'elle ait été pillée par des vandales de passage! \n\n Nous vous enjoignons à renouveller au plus vite votre certificat auprès de nos service. \n\n Signé : Polpo");
        }
        if (expirationTime < 2)
        {
          Utils.SendMailToPC(ownerId, "Hôtel des ventes de Similisse", "Expiration prochaine du certificat de votre échoppe",
            $"Cher Marchand, \n\n Nous sommes au devoir de vous informer que le certificat de votre échoppe {panel.Name} aura expiré dès demain. Nos hommes ne seront alors plus en mesure de la protéger, c'est courir le risque de la voir pillée par des vandales de passage ! \n\n Nous vous enjoignons à renouveller au plus vite votre certificat auprès de nos service. \n\n Signé : Polpo");

          Utils.SendDiscordPMToPlayer(ownerId, $"Cher Marchand, \n\n Nous sommes au regret de vous informer que votre échoppe {panel.Name} aura expiré dès demain. Nos hommes ne seront alors plus en mesure de la protéger, c'est courir le risque de la voir pillée par des vandales de passage ! \n\n Nous vous enjoignons à renouveller au plus vite votre certificat auprès de nos service. \n\n Signé : Polpo");
        }
        else if (expirationTime < 8)
        {
          Utils.SendMailToPC(ownerId, "Hôtel des ventes de Similisse", "Expiration prochaine du certificat de votre échoppe",
            $"Cher Marchand, \n\n Nous sommes au devoir de vous informer que le certificat de votre échoppe {panel.Name} aura expiré dès la semaine prochaine. Nos hommes ne seront alors plus en mesure de la protéger, c'est courir le risque de la voir pillée par des vandales de passage ! \n\n Nous vous enjoignons à renouveller au plus vite votre certificat auprès de nos service. \n\n Signé : Polpo");

          Utils.SendDiscordPMToPlayer(ownerId, $"Cher Marchand, \n\n Nous sommes au regret de vous informer que votre échoppe {panel.Name} aura expiré dès la semaine prochaine. Nos hommes ne seront alors plus en mesure de la protéger, c'est courir le risque de la voir pillée par des vandales de passage ! \n\n Nous vous enjoignons à renouveller au plus vite votre certificat auprès de nos service. \n\n Signé : Polpo");
        }

        panel.OnUsed += PlaceableSystem.OnUsedPlayerOwnedShop;

        foreach (NwItem item in shop.Items)
          item.BaseGoldValue = (uint)item.GetObjectVariable<LocalVariableInt>("_SET_SELL_PRICE").Value;
      }
    }
    private static async void DeleteExpiredShop(int rowid)
    {
      await NwTask.Delay(TimeSpan.FromSeconds(0.2));

      SqLiteUtils.DeletionQuery("playerShops",
          new Dictionary<string, string>() { { "rowid", rowid.ToString() } });
    }
    private static void RestorePlayerAuctionsFromDatabase()
    {
      var result = SqLiteUtils.SelectQuery("playerAuctions",
        new List<string>() { { "shop" }, { "panel" }, { "characterId" }, { "rowid" }, { "expirationDate" }, { "highestAuction" }, { "highestAuctionner" }, { "areaTag" }, { "position" }, { "facing" } },
        new List<string[]>() { new string[] { "shop", "deleted", "!=" } });

      foreach (var auction in result)
      {
        NwStore shop = SqLiteUtils.StoreSerializationFormatProtection(auction[0], Utils.GetLocationFromDatabase(auction[7], auction[8], float.Parse(auction[2])));
        NwPlaceable panel = SqLiteUtils.PlaceableSerializationFormatProtection(auction[1], Utils.GetLocationFromDatabase(auction[7], auction[8], float.Parse(auction[2])));
        shop.GetObjectVariable<LocalVariableInt>("_OWNER_ID").Value = int.Parse(auction[2]);
        shop.GetObjectVariable<LocalVariableInt>("_SHOP_ID").Value = int.Parse(auction[3]);
        shop.GetObjectVariable<LocalVariableInt>("_CURRENT_AUCTION").Value = int.Parse(auction[5]);
        shop.GetObjectVariable<LocalVariableInt>("_CURRENT_AUCTIONNER").Value = int.Parse(auction[6]);
        panel.GetObjectVariable<LocalVariableInt>("_OWNER_ID").Value = int.Parse(auction[2]);
        panel.GetObjectVariable<LocalVariableInt>("_SHOP_ID").Value = int.Parse(auction[3]);

        panel.OnUsed += PlaceableSystem.OnUsedPlayerOwnedAuction;

        foreach (NwItem item in shop.Items)
          item.BaseGoldValue = (uint)item.GetObjectVariable<LocalVariableInt>("_CURRENT_AUCTION").Value;
      }
    }
    public static async void HandleExpiredAuctions()
    {
      var result = SqLiteUtils.SelectQuery("playerAuctions",
        new List<string>() { { "characterId" }, { "rowid" }, { "highestAuction" }, { "highestAuctionner" }, { "shop" } },
        new List<string[]>() { new string[] { "expirationDate", DateTime.Now.ToString(), ">" } });

      foreach (var auction in result)
      {
        int buyerId = int.Parse(auction[3]);
        int sellerId = int.Parse(auction[0]);
        int auctionId = int.Parse(auction[1]);

        NwPlayer oSeller = NwModule.Instance.Players.FirstOrDefault(p => p.LoginCreature != null && p.LoginCreature.GetObjectVariable<PersistentVariableInt>("characterId").Value == sellerId);
        NwStore store = NwObject.FindObjectsOfType<NwStore>().FirstOrDefault(p => p.GetObjectVariable<LocalVariableInt>("_AUCTION_ID").Value == auctionId);
        NwStore oStore = SqLiteUtils.StoreSerializationFormatProtection(auction[4], NwModule.Instance.StartingLocation);
        NwItem tempItem = oStore.Items.FirstOrDefault();
        oStore.Destroy();

        if (buyerId <= 0) // pas d'acheteur
        {
          // S'il est co, on rend l'item au seller et on détruit la ligne en BDD. S'il est pas co, on attend la prochaine occurence pour lui rendre l'item
          if (oSeller != null)
          {
            tempItem.Clone(oSeller.LoginCreature);
            NwItem authorization = await NwItem.Create("auction_clearanc", oSeller.LoginCreature);
            authorization.GetObjectVariable<LocalVariableString>("ITEM_KEY").Value = Config.itemKey;
            oSeller.SendServerMessage($"Aucune enchère sur votre {tempItem.Name.ColorString(ColorConstants.Orange)}. L'objet vous a donc été restitué.");
          }
          else
          {
            Utils.SendMailToPC(sellerId, "Hotel des ventes de Similisse", $"Enchère sur {tempItem.Name} expirée",
              $"Très honoré vendeur, \n\n Nous avons le regret de vous annoncer que votre mise aux enchères de {tempItem.Name} a expiré sans trouver d'acheteur. \n\n L'objet a été restitué à votre entrepot. \n\n Signé : Polpo");

            Utils.SendItemToPCStorage(sellerId, tempItem);
          }
        }
        else
        {
          // Si highestAuction > 0 On donne les sous au seller et on lui envoie un message s'il est co. S'il n'est pas co on met à jour en bdd et on lui envoie un courrier
          int highestAuction = int.Parse(auction[2]);

          if (highestAuction > 0)
          {
            if (oSeller != null)
            {
              if (PlayerSystem.Players.TryGetValue(oSeller.LoginCreature, out PlayerSystem.Player seller))
              {
                seller.bankGold += highestAuction * 95 / 100;
                oSeller.SendServerMessage($"Votre enchère vous a permis de remporter {(highestAuction * 95 / 100).ToString().ColorString(ColorConstants.Orange)}. L'or a été versé à votre banque !");
              }

              NwItem authorization = await NwItem.Create("auction_clearanc", oSeller.LoginCreature);
              authorization.GetObjectVariable<LocalVariableString>("ITEM_KEY").Value = Config.itemKey;
            }
            else
            {
              Task delayedUpdate = NwTask.Run(async () =>
              {
                await NwTask.Delay(TimeSpan.FromSeconds(0.2));

                SqLiteUtils.UpdateQuery("playerCharacters",
                  new List<string[]>() { new string[] { "bankGold", highestAuction.ToString(), "+" } },
                  new List<string[]>() { new string[] { "ROWID", sellerId.ToString() } });
              });

              Utils.SendMailToPC(sellerId, "Hotel des ventes de Similisse", $"Enchère sur {tempItem.Name} conclue",
                $"Très honoré vendeur, \n\n Nous avons l'immense plaisir de vous annoncer que votre enchère sur {tempItem.Name} a porté ses fruits. \n\n Celle-ci vous a permis d'acquérir {highestAuction} pièce(s) d'or ! \n\n Signé : Polpo");
            }
          }
          // Si le buyer est co, on lui file l'item et on détruit la ligne en BDD. S'il est pas co, on transfère dans son entrepot perso
          NwPlayer oBuyer = NwModule.Instance.Players.FirstOrDefault(p => p.LoginCreature != null && p.LoginCreature.GetObjectVariable<PersistentVariableInt>("characterId").Value == buyerId);

          if (oBuyer != null)
          {
            tempItem.Clone(oBuyer.LoginCreature);
            oSeller.SendServerMessage($"Vous venez de remporter l'enchère sur {tempItem.Name.ColorString(ColorConstants.Orange)}. L'objet se trouve désormais dans votre inventaire.");
          }
          else
          {
            Utils.SendMailToPC(sellerId, "Hotel des ventes de Similisse", $"Enchère sur {tempItem.Name} remportée",
                $"Très honoré vendeur, \n\n Nous avons l'immense plaisir de vous annoncer que vous avez remporté l'enchère sur {tempItem.Name}. \n\n L'objet a été transformé dans votre entrepôt personnel ! \n\n Signé : Polpo");

            Utils.SendItemToPCStorage(buyerId, tempItem);
          }
        }

        // On détruit la shop et le panel s'ils ne sont pas null
        if (store != null)
          store.Destroy();

        NwPlaceable panel = NwObject.FindObjectsOfType<NwPlaceable>().FirstOrDefault(p => p.GetObjectVariable<LocalVariableInt>("_AUCTION_ID").Value == auctionId);
        if (panel != null)
          panel.Destroy();

        tempItem.Destroy();

        Task delayedDeletion = NwTask.Run(async () =>
        {
          await NwTask.Delay(TimeSpan.FromSeconds(0.2));
          DeleteExpiredAuction(auctionId);
        });
      }
    }
    private static void DeleteExpiredAuction(int auctionId)
    {
      SqLiteUtils.DeletionQuery("playerAuctions",
          new Dictionary<string, string>() { { "rowid", auctionId.ToString() } });
    }
    private static void UpdateExpiredAuction(int auctionId)
    {
      SqLiteUtils.UpdateQuery("playerAuctions",
        new List<string[]>() { new string[] { "highestAuction", "0" } },
        new List<string[]>() { new string[] { "ROWID", auctionId.ToString() } });
    }
    private static void RestoreResourceBlocksFromDatabase()
    {
      var result = SqLiteUtils.SelectQuery("areaResourceStock",
          new List<string>() { { "id" }, { "areaTag" }, { "type" }, { "quantity" }, { "lastChecked" } },
          new List<string[]>() { });

      foreach (var resourceBlock in result)
      {
        int blockId = int.Parse(resourceBlock[0]);
        NwArea blockArea = (NwArea)NwObject.FindObjectsWithTag(resourceBlock[1]).FirstOrDefault();
        string spawnType = resourceBlock[2];
        int quantity = int.Parse(resourceBlock[3]);
        DateTime lastChecked = DateTime.Parse(resourceBlock[4]); 
        NwWaypoint blockWaypoint = blockArea.FindObjectsOfTypeInArea<NwWaypoint>().FirstOrDefault(w => w.Tag == spawnType && w.GetObjectVariable<LocalVariableInt>("id").Value == blockId);

        Log.Info($"Area {blockArea.Name} - Spawning {spawnType} - id {blockId} - Quantity {quantity} - lastChecked {lastChecked} - wp {blockWaypoint}");

        switch (spawnType)
        {
          case "ore_spawn_wp":
            SpawnResourceBlock("mineable_rock", blockWaypoint, quantity, lastChecked);
            break;
          case "wood_spawn_wp":
            SpawnResourceBlock("mineable_tree", blockWaypoint, quantity, lastChecked);
            break;
          case "animal_spawn_wp":
            SpawnResourceBlock("mineable_animal", blockWaypoint, quantity, lastChecked);
            break;
        }
      }
    }
    public static void SpawnCollectableResources()
    {
      Log.Info("Starting to spawn collectable ressources");

      foreach (NwWaypoint ressourcePoint in NwObject.FindObjectsWithTag<NwWaypoint>(new string[] { "ore_spawn_wp", "wood_spawn_wp", "animal_spawn_wp" }).Where(l => l.Area.GetObjectVariable<LocalVariableInt>("_AREA_LEVEL").Value > 1))
      {
        if (NwRandom.Roll(Utils.random, 100) > 85)
        {
          int resourceQuantity = 5 * NwRandom.Roll(Utils.random, 100 - (ressourcePoint.Area.GetObjectVariable<LocalVariableInt>("_AREA_LEVEL").Value * 10));
          switch (ressourcePoint.Tag)
          {
            case "ore_spawn_wp":
              SpawnResourceBlock("mineable_rock", ressourcePoint, resourceQuantity, DateTime.Now);
              break;
            case "wood_spawn_wp":
              SpawnResourceBlock("mineable_tree", ressourcePoint, resourceQuantity, DateTime.Now);
              break;
            case "animal_spawn_wp":
              SpawnResourceBlock("mineable_animal", ressourcePoint, resourceQuantity, DateTime.Now);
              break;
          }

          Log.Info($"REFILL - {ressourcePoint.Area.Name} - {ressourcePoint.Name}");
        }
        /*int areaLevel = ressourcePoint.Area.GetObjectVariable<LocalVariableInt>("_AREA_LEVEL").Value;
        if (NwRandom.Roll(Utils.random, 100) >= (areaLevel * 20) - 20)
        {
          string resRef = "";
          string name = "";

          switch (ressourcePoint.Tag)
          {
            case "ore_spawn_wp":
              resRef = "mineable_rock";
              name = Enum.GetName(typeof(OreType), GetRandomOreSpawnFromAreaLevel(areaLevel));
              break;
            case "wood_spawn_wp":
              resRef = "mineable_tree";
              name = Enum.GetName(typeof(WoodType), GetRandomOreSpawnFromAreaLevel(areaLevel));
              break;
          }

          var newRock = NwPlaceable.Create(resRef, ressourcePoint.Location);
          newRock.Name = name;
          newRock.GetObjectVariable<LocalVariableInt>("_ORE_AMOUNT").Value = 50 * NwRandom.Roll(Utils.random, 100);
          ressourcePoint.Destroy();

          Log.Info($"REFILL - {ressourcePoint.Area.Name} - {ressourcePoint.Name}");
        }*/
    }

    /*foreach (NwArea area in NwModule.Instance.Areas.Where(l => l.GetObjectVariable<LocalVariableInt>("_AREA_LEVEL").Value > 1))
    {
      int areaLevel = area.GetObjectVariable<LocalVariableInt>("_AREA_LEVEL").Value;

      SqLiteUtils.InsertQuery("areaResourceStock",
        new List<string[]>() { new string[] { "areaTag", area.Tag }, new string[] { "mining", (areaLevel * 2).ToString() }, new string[] { "wood", (areaLevel * 2).ToString() }, new string[] { "animals", (areaLevel * 2).ToString() } },
        new List<string>() { "areaTag" },
        new List<string[]>() { new string[] { "mining" }, new string[] { "wood" }, new string[] { "animals" } });
    }*/
  }
    private static async void SpawnResourceBlock(string resourceTemplate, NwWaypoint waypoint, int quantity, DateTime lastChecked)
    {
      try
      {
        int blockId = waypoint.GetObjectVariable<LocalVariableInt>("id").Value;

        NwPlaceable newResourceBlock = NwPlaceable.Create(resourceTemplate, waypoint.Location);
        newResourceBlock.GetObjectVariable<LocalVariableInt>("_ORE_AMOUNT").Value = quantity;
        newResourceBlock.GetObjectVariable<DateTimeLocalVariable>("_LAST_CHECK").Value = lastChecked;
        newResourceBlock.GetObjectVariable<LocalVariableInt>("id").Value = blockId;
        newResourceBlock.VisibilityOverride = VisibilityMode.AlwaysVisible;

        NwPlaceable interactibleMateria = NwPlaceable.Create("mineable_materia", waypoint.Location);
        interactibleMateria.GetObjectVariable<LocalVariableInt>("id").Value = blockId;
        interactibleMateria.GetObjectVariable<LocalVariableInt>("_ORE_AMOUNT").Value = quantity;
        interactibleMateria.GetObjectVariable<DateTimeLocalVariable>("_LAST_CHECK").Value = lastChecked;
        interactibleMateria.GetObjectVariable<LocalVariableString>("_RESOURCE_TYPE").Value = resourceTemplate;
        interactibleMateria.VisibilityOverride = VisibilityMode.Hidden;

        waypoint.Destroy();

        string id = waypoint.GetObjectVariable<LocalVariableInt>("id").Value.ToString();
        string areaTag = waypoint.Area.Tag;
        string type = waypoint.Tag;

        await SqLiteUtils.InsertQueryAsync("areaResourceStock",
            new List<string[]>() { new string[] { "id", id }, new string[] { "areaTag", areaTag }, new string[] { "type", type }, new string[] { "quantity", quantity.ToString() }, new string[] { "lastChecked", DateTime.Now.ToString() } },
            new List<string>() { "id", "areaTag", "type" },
            new List<string[]>() { new string[] { "quantity" }, new string[] { "lastChecked" } });
      }
      catch (Exception)
      {
        Log.Info($"Warning : could not spawn {waypoint.Tag} nb {waypoint.GetObjectVariable<LocalVariableInt>("id").Value} in {waypoint.Area.Name}");
        Utils.LogMessageToDMs($"Warning : could not spawn {waypoint.Tag} nb {waypoint.GetObjectVariable<LocalVariableInt>("id").Value} in {waypoint.Area.Name}");
      }
    }
    public static void DeleteExpiredMail()
    {
      SqLiteUtils.DeletionQuery("messenger",
          new Dictionary<string, string>() { { "expirationDate", DateTime.Now.AddDays(30).ToLongDateString() } }, ">");
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

      effectApplied.Effect.Tag = player.oid.CDKey;

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

      ItemUtils.weaponModelDictionary = new();

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
    private void HandleJobLoop(PlayerSystem.Player player)
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
        else player.craftJob.startTime = DateTime.Now;
      }
    }
    private void HandleLearnableLoop(PlayerSystem.Player player)
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
    private void HandleCheckAfkStatus(PlayerSystem.Player player)
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
