using Discord;
using Google.Cloud.Translation.V2;
using NLog;
using NWN.API;
using NWN.API.Events;
using NWN.Core;
using NWN.Core.NWNX;
using NWN.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static NWN.Systems.Craft.Collect.Config;

namespace NWN.Systems
{
  [ServiceBinding(typeof(ModuleSystem))]
  public class ModuleSystem
  {
    public static readonly Logger Log = LogManager.GetCurrentClassLogger();
    public static readonly TranslationClient googleTranslationClient = TranslationClient.Create();
    public static Dictionary<string, GoldBalance> goldBalanceMonitoring = new Dictionary<string, GoldBalance>();
    public ModuleSystem()
    {
      LoadDiscordBot();
      CreateDatabase();
      InitializeEvents();
      //FeatSystem.InitializeFeatModifiers();

      NwModule.Instance.OnModuleLoad += OnModuleLoad;
    }
    private void OnModuleLoad(ModuleEvents.OnModuleLoad onModuleLoad)
    {
      NwModule.Instance.GetLocalVariable<string>("X2_S_UD_SPELLSCRIPT").Value = "spellhook";

      SetModuleTime();
      SaveServerVault();

      RestorePlayerCorpseFromDatabase();
      RestorePlayerShopsFromDatabase();
      RestoreDMPersistentPlaceableFromDatabase();

      Log.Info($"test hasvalue : {NwModule.Instance.GetLocalVariable<int>("_TEST").HasValue}");
      Log.Info($"test hasnothing : {NwModule.Instance.GetLocalVariable<int>("_TEST").HasNothing}");
      Log.Info($"test value : {NwModule.Instance.GetLocalVariable<int>("_TEST").Value}");
      NwModule.Instance.GetLocalVariable<int>("_TEST").Value = 1;
      Log.Info($"test hasvalue : {NwModule.Instance.GetLocalVariable<int>("_TEST").HasValue}");
      Log.Info($"test hasnothing : {NwModule.Instance.GetLocalVariable<int>("_TEST").HasNothing}");
      Log.Info($"test value : {NwModule.Instance.GetLocalVariable<int>("_TEST").Value}");

      float resourceRespawnTime;
      if (DateTime.Now.Hour < 5)
        resourceRespawnTime = (float)(TimeSpan.Parse("05:00:00") - DateTime.Now.TimeOfDay).TotalSeconds;
      else
        resourceRespawnTime = (float)((DateTime.Now.AddDays(1).Date).AddHours(5) - DateTime.Now).TotalSeconds;

      Task spawnResources = NwTask.Run(async () =>
      {
        await NwTask.Delay(TimeSpan.FromSeconds(resourceRespawnTime));
        await SpawnCollectableResources(resourceRespawnTime);
      });
    }
    private async void LoadDiscordBot()
    {
      await Bot.MainAsync();
    }

    private void CreateDatabase()
    {
      var query = NWScript.SqlPrepareQueryCampaign(Config.database, $"CREATE TABLE IF NOT EXISTS moduleInfo" +
        $"('year' INTEGER NOT NULL, 'month' INTEGER NOT NULL, 'day' INTEGER NOT NULL, 'hour' INTEGER NOT NULL, 'minute' INTEGER NOT NULL, 'second' INTEGER NOT NULL)");
      NWScript.SqlStep(query);

      query = NWScript.SqlPrepareQueryCampaign(Config.database, $"CREATE TABLE IF NOT EXISTS PlayerAccounts" +
        $"('accountName' TEXT NOT NULL, 'cdKey' TEXT, 'bonusRolePlay' INTEGER NOT NULL, 'discordId' TEXT, 'rank' TEXT)");
      NWScript.SqlStep(query);

      query = NWScript.SqlPrepareQueryCampaign(Config.database, $"CREATE TABLE IF NOT EXISTS playerCharacters" +
        $"('accountId' INTEGER NOT NULL, 'characterName' TEXT NOT NULL, 'dateLastSaved' TEXT NOT NULL, 'currentSkillType' INTEGER NOT NULL, 'currentSkillJob' INTEGER NOT NULL," +
        $"'currentCraftJobRemainingTime' REAL, 'currentCraftJob' INTEGER NOT NULL, 'currentCraftObject' TEXT NOT NULL," +
        $"currentCraftJobMaterial TEXT, 'frostAttackOn' INTEGER NOT NULL, areaTag TEXT, position TEXT, facing REAL," +
        $"currentHP INTEGER, bankGold INTEGER, menuOriginTop INTEGER, menuOriginLeft INTEGER, storage TEXT)");
      NWScript.SqlStep(query);

      query = NWScript.SqlPrepareQueryCampaign(Config.database, $"CREATE TABLE IF NOT EXISTS playerLearnableSkills" +
        $"('characterId' INTEGER NOT NULL, 'skillId' INTEGER NOT NULL, 'skillPoints' INTEGER NOT NULL, 'trained' INTEGER, UNIQUE (characterId, skillId))");
      NWScript.SqlStep(query);

      query = NWScript.SqlPrepareQueryCampaign(Config.database, $"CREATE TABLE IF NOT EXISTS playerLearnableSpells" +
        $"('characterId' INTEGER NOT NULL, 'skillId' INTEGER NOT NULL, 'skillPoints' INTEGER NOT NULL, 'trained' INTEGER, UNIQUE (characterId, skillId))");
      NWScript.SqlStep(query);

      query = NWScript.SqlPrepareQueryCampaign(Config.database, $"CREATE TABLE IF NOT EXISTS playerMaterialStorage" +
        $"('characterId' INTEGER NOT NULL, 'materialName' TEXT NOT NULL, 'materialStock' INTEGER, UNIQUE (characterId, materialName))");
      NWScript.SqlStep(query);

      query = NWScript.SqlPrepareQueryCampaign(Config.database, $"CREATE TABLE IF NOT EXISTS playerDeathCorpses" +
        $"('characterId' INTEGER NOT NULL, 'deathCorpse' TEXT NOT NULL, 'areaTag' TEXT NOT NULL, 'position' TEXT NOT NULL)");
      NWScript.SqlStep(query);

      query = NWScript.SqlPrepareQueryCampaign(Config.database, $"CREATE TABLE IF NOT EXISTS loot_containers" +
        $"('chestTag' TEXT NOT NULL, 'accountID' INTEGER NOT NULL, 'serializedChest' TEXT NOT NULL, 'position' TEXT NOT NULL, 'facing' REAL NOT NULL, PRIMARY KEY(chestTag))");
      NWScript.SqlStep(query);

      query = NWScript.SqlPrepareQueryCampaign(Config.database, $"CREATE TABLE IF NOT EXISTS dm_persistant_placeable" +
        $"('accountID' INTEGER NOT NULL, 'serializedPlaceable' TEXT NOT NULL, 'areaTag' TEXT NOT NULL, 'position' TEXT NOT NULL, 'facing' REAL NOT NULL)");
      NWScript.SqlStep(query);

      query = NWScript.SqlPrepareQueryCampaign(Config.database, $"CREATE TABLE IF NOT EXISTS playerMapPins" +
        $"('characterId' INTEGER NOT NULL, 'mapPinId' INTEGER NOT NULL, 'areaTag' TEXT NOT NULL, 'x' REAL NOT NULL, 'y' REAL NOT NULL, 'note' TEXT, UNIQUE (characterId, mapPinId))");
      NWScript.SqlStep(query);

      query = NWScript.SqlPrepareQueryCampaign(Config.database, $"CREATE TABLE IF NOT EXISTS playerDescriptions" +
        $"('characterId' INTEGER NOT NULL, 'descriptionName' TEXT NOT NULL, 'description' TEXT NOT NULL, UNIQUE (characterId, descriptionName))");
      NWScript.SqlStep(query);

      query = NWScript.SqlPrepareQueryCampaign(Config.database, $"CREATE TABLE IF NOT EXISTS areaResourceStock" +
        $"('areaTag' TEXT NOT NULL, 'mining' INTEGER, 'wood' INTEGER, 'animals' INTEGER, PRIMARY KEY(areaTag))");
      NWScript.SqlStep(query);

      query = NWScript.SqlPrepareQueryCampaign(Config.database, $"CREATE TABLE IF NOT EXISTS scriptPerformance" +
        $"('script' TEXT NOT NULL, 'nbExecutions' INTEGER NOT NULL, 'averageExecutionTime' REAL NOT NULL, 'cumulatedExecutionTime' REAL NOT NULL, PRIMARY KEY(script))");
      NWScript.SqlStep(query);

      query = NWScript.SqlPrepareQueryCampaign(Config.database, $"CREATE TABLE IF NOT EXISTS goldBalance" +
        $"('lootedTag' TEXT NOT NULL, 'nbTimesLooted' INTEGER NOT NULL, 'averageGold' INT NOT NULL, 'cumulatedGold' INT NOT NULL, PRIMARY KEY(lootedTag))");
      NWScript.SqlStep(query);

      query = NWScript.SqlPrepareQueryCampaign(Config.database, $"CREATE TABLE IF NOT EXISTS playerAreaExplorationState" +
        $"('characterId' INTEGER NOT NULL, 'areaTag' TEXT NOT NULL, 'explorationState' TEXT NOT NULL, UNIQUE (characterId, areaTag))");
      NWScript.SqlStep(query);

      query = NWScript.SqlPrepareQueryCampaign(Config.database, $"CREATE TABLE IF NOT EXISTS playerPrivateContracts" +
        $"('characterId' INTEGER NOT NULL, 'expirationDate' TEXT NOT NULL, 'serializedContract' TEXT NOT NULL, 'totalValue' INTEGER NOT NULL)");
      NWScript.SqlStep(query);

      query = NWScript.SqlPrepareQueryCampaign(Config.database, $"CREATE TABLE IF NOT EXISTS playerBuyOrders" +
        $"('characterId' INTEGER NOT NULL, 'expirationDate' TEXT NOT NULL, 'material' TEXT NOT NULL, 'quantity' INTEGER NOT NULL, 'unitPrice' INTEGER NOT NULL)");
      NWScript.SqlStep(query);

      query = NWScript.SqlPrepareQueryCampaign(Config.database, $"CREATE TABLE IF NOT EXISTS playerSellOrders" +
        $"('characterId' INTEGER NOT NULL, 'expirationDate' TEXT NOT NULL, 'material' TEXT NOT NULL, 'quantity' INTEGER NOT NULL, 'unitPrice' INTEGER NOT NULL)");
      NWScript.SqlStep(query);

      query = NWScript.SqlPrepareQueryCampaign(Config.database, $"CREATE TABLE IF NOT EXISTS playerShops" +
        $"('characterId' INTEGER NOT NULL, 'shop' TEXT NOT NULL, 'panel' TEXT NOT NULL, 'expirationDate' TEXT NOT NULL, 'areaTag' TEXT NOT NULL, 'position' TEXT NOT NULL, 'facing' REAL NOT NULL)");
      NWScript.SqlStep(query);

      query = NWScript.SqlPrepareQueryCampaign(Config.database, $"CREATE TABLE IF NOT EXISTS playerAuctions" +
        $"('characterId' INTEGER NOT NULL, 'shop' TEXT NOT NULL, 'panel' TEXT NOT NULL, 'expirationDate' TEXT NOT NULL, 'highestAuction' INTEGER NOT NULL, 'highestAuctionner' INTEGER NOT NULL, 'areaTag' TEXT NOT NULL, 'position' TEXT NOT NULL, 'facing' REAL NOT NULL)");
      NWScript.SqlStep(query);

      query = NWScript.SqlPrepareQueryCampaign(Config.database, $"CREATE TABLE IF NOT EXISTS playerItemAppearance" +
        $"('characterId' INTEGER NOT NULL, 'appearanceName' TEXT NOT NULL, 'serializedAppearance' TEXT NOT NULL, 'baseItemType' INTEGER NOT NULL, 'AC' INTEGER NOT NULL, UNIQUE (characterId, appearanceName))");
      NWScript.SqlStep(query);
    }
    private void InitializeEvents()
    {
      EventsPlugin.SubscribeEvent("NWNX_ON_BROADCAST_CAST_SPELL_AFTER", "a_spellbroadcast");
      EventsPlugin.ToggleDispatchListMode("NWNX_ON_BROADCAST_CAST_SPELL_AFTER", "a_spellbroadcast", 1);
      EventsPlugin.SubscribeEvent("NWNX_ON_CAST_SPELL_BEFORE", "b_spellcast");
      EventsPlugin.ToggleDispatchListMode("NWNX_ON_CAST_SPELL_BEFORE", "b_spellcast", 1);
      EventsPlugin.SubscribeEvent("NWNX_ON_CAST_SPELL_AFTER", "a_spellcast");
      EventsPlugin.ToggleDispatchListMode("NWNX_ON_CAST_SPELL_AFTER", "a_spellcast", 1);
      //EventsPlugin.SubscribeEvent("NWNX_ON_SPELL_INTERRUPTED_AFTER", "_onspellinterrupted_after");
      //EventsPlugin.ToggleDispatchListMode("NWNX_ON_SPELL_INTERRUPTED_AFTER", "_onspellinterrupted_after", 1);

      EventsPlugin.SubscribeEvent("NWNX_ON_ITEM_SCROLL_LEARN_BEFORE", "b_learn_scroll");

      EventsPlugin.SubscribeEvent("NWNX_ON_INPUT_TOGGLE_PAUSE_BEFORE", "spacebar_down");
      EventsPlugin.ToggleDispatchListMode("NWNX_ON_INPUT_TOGGLE_PAUSE_BEFORE", "spacebar_down", 1);

      EventsPlugin.SubscribeEvent("NWNX_ON_ITEM_EQUIP_BEFORE", "event_equip_items_before");
      EventsPlugin.ToggleDispatchListMode("NWNX_ON_ITEM_EQUIP_BEFORE", "event_equip_items_before", 1);
      EventsPlugin.SubscribeEvent("NWNX_ON_ITEM_UNEQUIP_BEFORE", "b_unequip");
      EventsPlugin.ToggleDispatchListMode("NWNX_ON_ITEM_UNEQUIP_BEFORE", "b_unequip", 1);

      EventsPlugin.SubscribeEvent("NWNX_ON_DM_POSSESS_FULL_POWER_BEFORE", "b_dm_possess");
      EventsPlugin.SubscribeEvent("NWNX_ON_DM_POSSESS_BEFORE", "b_dm_possess");
      EventsPlugin.SubscribeEvent("NWNX_ON_DM_SPAWN_OBJECT_AFTER", "dm_spawn_object");
      EventsPlugin.SubscribeEvent("NWNX_ON_DM_JUMP_TARGET_TO_POINT_AFTER", "a_dm_jump_target");
      EventsPlugin.SubscribeEvent("NWNX_ON_DM_GIVE_XP_BEFORE", "on_dm_give_xp");
      EventsPlugin.SubscribeEvent("NWNX_ON_DM_GIVE_LEVEL_BEFORE", "on_dm_give_xp");
      EventsPlugin.SubscribeEvent("NWNX_ON_DM_GIVE_GOLD_BEFORE", "on_dm_give_gold");
      EventsPlugin.SubscribeEvent("NWNX_ON_DM_GIVE_ITEM_AFTER", "on_dm_give_item");

      EventsPlugin.SubscribeEvent("NWNX_ON_START_COMBAT_ROUND_AFTER", "a_start_combat");
      EventsPlugin.ToggleDispatchListMode("NWNX_ON_START_COMBAT_ROUND_AFTER", "a_start_combat", 1);
      EventsPlugin.SubscribeEvent("NWNX_ON_COMBAT_MODE_OFF", "event_combatmode");
      EventsPlugin.ToggleDispatchListMode("NWNX_ON_COMBAT_MODE_OFF", "event_combatmode", 1);
      EventsPlugin.SubscribeEvent("NWNX_ON_USE_SKILL_BEFORE", "event_skillused");
      EventsPlugin.ToggleDispatchListMode("NWNX_ON_USE_SKILL_BEFORE", "event_skillused", 1);

      EventsPlugin.SubscribeEvent("NWNX_ON_DO_LISTEN_DETECTION_AFTER", "a_detection");
      EventsPlugin.ToggleDispatchListMode("NWNX_ON_DO_LISTEN_DETECTION_AFTER", "a_detection", 1);

      EventsPlugin.SubscribeEvent("NWNX_ON_TIMING_BAR_CANCEL_BEFORE", "collect_cancel");
      EventsPlugin.ToggleDispatchListMode("NWNX_ON_TIMING_BAR_CANCEL_BEFORE", "collect_cancel", 1);
      EventsPlugin.SubscribeEvent("NWNX_ON_CLIENT_DISCONNECT_BEFORE", "collect_cancel");
      EventsPlugin.ToggleDispatchListMode("NWNX_ON_CLIENT_DISCONNECT_BEFORE", "collect_cancel", 1);
      EventsPlugin.SubscribeEvent("NWNX_ON_ITEM_EQUIP_BEFORE", "collect_cancel");
      EventsPlugin.ToggleDispatchListMode("NWNX_ON_ITEM_EQUIP_BEFORE", "collect_cancel", 1);
      EventsPlugin.SubscribeEvent("NWNX_ON_ITEM_UNEQUIP_BEFORE", "collect_cancel");
      EventsPlugin.ToggleDispatchListMode("NWNX_ON_ITEM_UNEQUIP_BEFORE", "collect_cancel", 1);
      EventsPlugin.SubscribeEvent("NWNX_ON_START_COMBAT_ROUND_AFTER", "collect_cancel");
      EventsPlugin.ToggleDispatchListMode("NWNX_ON_START_COMBAT_ROUND_AFTER", "collect_cancel", 1);
      EventsPlugin.SubscribeEvent("NWNX_ON_INPUT_CAST_SPELL_BEFORE", "collect_cancel");
      EventsPlugin.ToggleDispatchListMode("NWNX_ON_INPUT_CAST_SPELL_BEFORE", "collect_cancel", 1);

      EventsPlugin.SubscribeEvent("NWNX_ON_JOURNAL_OPEN_AFTER", "on_journal_open");
      EventsPlugin.SubscribeEvent("NWNX_ON_JOURNAL_CLOSE_AFTER", "on_journal_close");

      EventsPlugin.SubscribeEvent("NWNX_ON_STORE_REQUEST_BUY_BEFORE", "before_store_buy");
      EventsPlugin.SubscribeEvent("NWNX_ON_STORE_REQUEST_BUY_AFTER", "after_store_buy");
      EventsPlugin.SubscribeEvent("NWNX_ON_STORE_REQUEST_SELL_BEFORE", "b_store_sell");

      EventsPlugin.SubscribeEvent("NWNX_ON_MAP_PIN_ADD_PIN_AFTER", "map_pin_added");
      EventsPlugin.SubscribeEvent("NWNX_ON_MAP_PIN_CHANGE_PIN_AFTER", "map_pin_changed");
      EventsPlugin.SubscribeEvent("NWNX_ON_MAP_PIN_DESTROY_PIN_AFTER", "mappin_destroyed");

      EventsPlugin.SubscribeEvent("NWNX_ON_INPUT_EMOTE_BEFORE", "on_input_emote");

      EventsPlugin.SubscribeEvent("NWNX_ON_ELC_VALIDATE_CHARACTER_BEFORE", "before_elc");
      EventsPlugin.SubscribeEvent("NWNX_ON_CLIENT_LEVEL_UP_BEGIN_BEFORE", "client_lvlup");

      DamagePlugin.SetAttackEventScript("on_attack");

      //EventsPlugin.SubscribeEvent("NWNX_ON_HAS_FEAT_AFTER", "event_has_feat");
    }
    private void SetModuleTime()
    {
      var query = NWScript.SqlPrepareQueryCampaign(Config.database, $"SELECT year, month, day, hour, minute, second from moduleInfo where rowid = 1");
      if(NWScript.SqlStep(query) == 1)
        NwDateTime.Now = new NwDateTime(NWScript.SqlGetInt(query, 0), NWScript.SqlGetInt(query, 1), NWScript.SqlGetInt(query, 2), NWScript.SqlGetInt(query, 3), NWScript.SqlGetInt(query, 4), NWScript.SqlGetInt(query, 5));
      else
      {
        query = NWScript.SqlPrepareQueryCampaign(Config.database, $"INSERT INTO moduleInfo (year, month, day, hour, minute, second) VALUES (@year, @month, @day, @hour, @minute, @second)");
        NWScript.SqlStep(query);
        NWScript.SqlBindInt(query, "@year", NwDateTime.Now.Year);
        NWScript.SqlBindInt(query, "@month", NwDateTime.Now.Month);
        NWScript.SqlBindInt(query, "@day", NwDateTime.Now.DayInTenday);
        NWScript.SqlBindInt(query, "@hour", NwDateTime.Now.Hour);
        NWScript.SqlBindInt(query, "@minute", NwDateTime.Now.Minute);
        NWScript.SqlBindInt(query, "@second", NwDateTime.Now.Second);
      }
    }
    public static async Task SpawnCollectableResources(float delay)
    {
      Log.Info("Starting to spawn collectable ressources");

      foreach (NwWaypoint ressourcePoint in NwModule.FindObjectsWithTag<NwWaypoint>(new string[] { "ore_spawn_wp", "wood_spawn_wp" }).Where(l => l.Area.GetLocalVariable<int>("_AREA_LEVEL").Value > 1))
      {
        int areaLevel = ressourcePoint.Area.GetLocalVariable<int>("_AREA_LEVEL").Value;
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
          newRock.GetLocalVariable<int>("_ORE_AMOUNT").Value = 50 * NwRandom.Roll(NWN.Utils.random, 100);
          ressourcePoint.Destroy();

          Log.Info($"REFILL - {ressourcePoint.Area} - {ressourcePoint.Name}");
        }
      }

      foreach (NwArea area in NwModule.Instance.Areas.Where(l => l.GetLocalVariable<int>("_AREA_LEVEL").Value > 1))
      {
        int areaLevel = area.GetLocalVariable<int>("_AREA_LEVEL").Value;

        var query = NWScript.SqlPrepareQueryCampaign(Config.database, $"INSERT INTO areaResourceStock (areaTag, mining, wood, animals) VALUES (@areaTag, @mining, @wood, @animals)" +
          $"ON CONFLICT (areaTag) DO UPDATE SET mining = @mining, wood = @wood, animals = @animals;");
        NWScript.SqlBindString(query, "@areaTag", area.Tag);
        NWScript.SqlBindInt(query, "@mining", areaLevel * 2);
        NWScript.SqlBindInt(query, "@wood", areaLevel * 2);
        NWScript.SqlBindInt(query, "@animals", areaLevel * 2);
        NWScript.SqlStep(query);
      }

      if (delay > 0.0f)
      {
        Task task3 = NwTask.Run(async () =>
        {
          await NwTask.Delay(TimeSpan.FromDays(1));
          await SpawnCollectableResources(delay);
        });
      }
    }
    private void SaveServerVault()
    {
      var query = NWScript.SqlPrepareQueryCampaign(Config.database, $"UPDATE moduleInfo SET year = @year, month = @month, day = @day, hour = @hour, minute = @minute, second = @second where rowid = 1");
      NWScript.SqlBindInt(query, "@year", NwDateTime.Now.Year);
      NWScript.SqlBindInt(query, "@month", NwDateTime.Now.Month);
      NWScript.SqlBindInt(query, "@day", NwDateTime.Now.DayInTenday);
      NWScript.SqlBindInt(query, "@hour", NwDateTime.Now.Hour);
      NWScript.SqlBindInt(query, "@minute", NwDateTime.Now.Minute);
      NWScript.SqlBindInt(query, "@second", NwDateTime.Now.Second);
      NWScript.SqlStep(query);

      HandleExpiredAuctions();

      NWScript.ExportAllCharacters();

      /*foreach (KeyValuePair<string, ScriptPerf> perfentry in ModuleSystem.scriptPerformanceMonitoring)
      {
          if (perfentry.Value.nbExecution == 0)
              continue;

          query = NWScript.SqlPrepareQueryCampaign(ModuleSystem.database, $"INSERT INTO scriptPerformance (script, nbExecutions, averageExecutionTime, cumulatedExecutionTime) VALUES (@script, @nbExecutions, @averageExecutionTime, @cumulatedExecutionTime)" +
          "ON CONFLICT (script) DO UPDATE SET nbExecutions = nbExecutions + @nbExecutions, averageExecutionTime = (cumulatedExecutionTime + @cumulatedExecutionTime) / (nbExecutions + @nbExecutions), cumulatedExecutionTime = cumulatedExecutionTime + @cumulatedExecutionTime");
          NWScript.SqlBindString(query, "@script", perfentry.Key);
          NWScript.SqlBindInt(query, "@nbExecutions", perfentry.Value.nbExecution);
          NWScript.SqlBindFloat(query, "@cumulatedExecutionTime", (float)perfentry.Value.cumulatedExecutionTime);
          NWScript.SqlBindFloat(query, "@averageExecutionTime", (float)perfentry.Value.cumulatedExecutionTime / perfentry.Value.nbExecution);
          NWScript.SqlStep(query);

          perfentry.Value.cumulatedExecutionTime = 0;
          perfentry.Value.nbExecution = 0;
      }*/

     /* foreach (KeyValuePair<string, GoldBalance> goldEntry in goldBalanceMonitoring)
      {
        query = NWScript.SqlPrepareQueryCampaign(Config.database, $"INSERT INTO goldBalance (lootedTag, nbTimesLooted, averageGold, cumulatedGold) VALUES (@lootedTag, @nbTimesLooted, @averageGold, @cumulatedGold)" +
        "ON CONFLICT (lootedTag) DO UPDATE SET nbTimesLooted = nbTimesLooted + @nbTimesLooted, averageGold = (cumulatedGold + @cumulatedGold) / (nbTimesLooted + @nbTimesLooted), cumulatedGold = cumulatedGold + @cumulatedGold");
        NWScript.SqlBindString(query, "@lootedTag", goldEntry.Key);
        NWScript.SqlBindInt(query, "@nbTimesLooted", goldEntry.Value.nbTimesLooted);
        NWScript.SqlBindInt(query, "@cumulatedGold", goldEntry.Value.cumulatedGold);
        NWScript.SqlBindInt(query, "@averageGold", goldEntry.Value.cumulatedGold / goldEntry.Value.nbTimesLooted);
        NWScript.SqlStep(query);

        goldEntry.Value.cumulatedGold = 0;
        goldEntry.Value.nbTimesLooted = 0;
      }*/

      Task DownloadDiscordUsers = NwTask.Run(async () =>
      {
        await Bot._client.DownloadUsersAsync(new List<IGuild> { { Bot._client.GetGuild(680072044364562528) } });
        return true;
      });

      Task scheduleNextSave = NwTask.Run(async () =>
      {
        await NwTask.Delay(TimeSpan.FromMinutes(10));
        SaveServerVault();
        return true;
      });      
    }
    public void RestorePlayerCorpseFromDatabase()
    {
      var query = NWScript.SqlPrepareQueryCampaign(Config.database, $"SELECT deathCorpse, areaTag, position, characterId FROM playerDeathCorpses");

      while (Convert.ToBoolean(NWScript.SqlStep(query)))
      {
        NwCreature corpse = ObjectPlugin.Deserialize(NWScript.SqlGetString(query, 0)).ToNwObject<NwCreature>();
        corpse.Location = Utils.GetLocationFromDatabase(NWScript.SqlGetString(query, 1), NWScript.SqlGetVector(query, 2), 0);
        corpse.GetLocalVariable<int>("_PC_ID").Value = NWScript.SqlGetInt(query, 3);

        foreach (NwItem item in corpse.Inventory.Items.Where(i => i.Tag != "item_pccorpse"))
          item.Destroy();

        PlayerSystem.SetupPCCorpse(corpse);
      }
    }
    public void RestorePlayerShopsFromDatabase()
    {
      // TODO : envoyer un courrier aux joueurs pour indiquer que leur shop à expiré
      // TODO : Plutôt que de détruire les shops expirées, rendre leurs inventaires accessibles à n'importe qui (ceux-ci n'étant pas protégés par Polpo)
      var deletionQuery = NWScript.SqlPrepareQueryCampaign(Config.database, $"DELETE FROM playerShops where expirationDate < @now");
      NWScript.SqlBindString(deletionQuery, "@now", DateTime.Now.ToString());
      NWScript.SqlStep(deletionQuery);

      var query = NWScript.SqlPrepareQueryCampaign(Config.database, $"SELECT shop, panel, characterId, rowid, expirationDate, areaTag, position, facing FROM playerShops");

      while (Convert.ToBoolean(NWScript.SqlStep(query)))
      {
        NwStore shop = ObjectPlugin.Deserialize(NWScript.SqlGetString(query, 0)).ToNwObject<NwStore>();
        NwPlaceable panel = ObjectPlugin.Deserialize(NWScript.SqlGetString(query, 1)).ToNwObject<NwPlaceable>();
        shop.Location = Utils.GetLocationFromDatabase(NWScript.SqlGetString(query, 5), NWScript.SqlGetVector(query, 6), NWScript.SqlGetFloat(query, 7));
        panel.Location = Utils.GetLocationFromDatabase(NWScript.SqlGetString(query, 5), NWScript.SqlGetVector(query, 6), NWScript.SqlGetFloat(query, 7));
        shop.GetLocalVariable<int>("_OWNER_ID").Value = NWScript.SqlGetInt(query, 2);
        shop.GetLocalVariable<int>("_SHOP_ID").Value = NWScript.SqlGetInt(query, 3);
        panel.GetLocalVariable<int>("_OWNER_ID").Value = NWScript.SqlGetInt(query, 2);
        panel.GetLocalVariable<int>("_SHOP_ID").Value = NWScript.SqlGetInt(query, 3);

        panel.OnUsed += PlaceableSystem.OnUsedPlayerOwnedShop;

        foreach (NwItem item in shop.Items)
        {
          ItemPlugin.SetBaseGoldPieceValue(item, item.GetLocalVariable<int>("_SET_SELL_PRICE").Value);
        }
      }
    }
    public void RestorePlayerAuctionsFromDatabase()
    {
      // TODO : envoyer un courrier aux joueurs pour indiquer que leur shop à expiré
      // TODO : Plutôt que de détruire les shops expirées, rendre leurs inventaires accessibles à n'importe qui (ceux-ci n'étant pas protégés par Polpo)
      var query = NWScript.SqlPrepareQueryCampaign(Config.database, $"SELECT shop, panel, characterId, rowid, expirationDate, highestAuction, highestAuctionner, areaTag, position, facing FROM playerAuctions where shop != 'deleted'");

      while (Convert.ToBoolean(NWScript.SqlStep(query)))
      {
        NwStore shop = ObjectPlugin.Deserialize(NWScript.SqlGetString(query, 0)).ToNwObject<NwStore>();
        NwPlaceable panel = ObjectPlugin.Deserialize(NWScript.SqlGetString(query, 1)).ToNwObject<NwPlaceable>();
        shop.Location = Utils.GetLocationFromDatabase(NWScript.SqlGetString(query, 7), NWScript.SqlGetVector(query, 8), NWScript.SqlGetFloat(query, 9));
        panel.Location = Utils.GetLocationFromDatabase(NWScript.SqlGetString(query, 7), NWScript.SqlGetVector(query, 8), NWScript.SqlGetFloat(query, 9));
        shop.GetLocalVariable<int>("_OWNER_ID").Value = NWScript.SqlGetInt(query, 2);
        shop.GetLocalVariable<int>("_SHOP_ID").Value = NWScript.SqlGetInt(query, 3);
        shop.GetLocalVariable<int>("_CURRENT_AUCTION").Value = NWScript.SqlGetInt(query, 5);
        shop.GetLocalVariable<int>("_CURRENT_AUCTIONNER").Value = NWScript.SqlGetInt(query, 6);
        panel.GetLocalVariable<int>("_OWNER_ID").Value = NWScript.SqlGetInt(query, 2);
        panel.GetLocalVariable<int>("_SHOP_ID").Value = NWScript.SqlGetInt(query, 3);

        panel.OnUsed += PlaceableSystem.OnUsedPlayerOwnedAuction;

        foreach (NwItem item in shop.Items)
          ItemPlugin.SetBaseGoldPieceValue(item, item.GetLocalVariable<int>("_CURRENT_AUCTION").Value);
      }
    }
    public void HandleExpiredAuctions()
    {
      // TODO : envoyer un courrier aux joueurs pour indiquer que leur shop à expiré
      // TODO : Plutôt que de détruire les shops expirées, rendre leurs inventaires accessibles à n'importe qui (ceux-ci n'étant pas protégés par Polpo)
      var query = NWScript.SqlPrepareQueryCampaign(Config.database, $"SELECT characterId, rowid, highestAuction, highestAuctionner, shop FROM playerAuctions WHERE expirationDate > @now");
      NWScript.SqlBindString(query, "@now", DateTime.Now.ToString());

      while (Convert.ToBoolean(NWScript.SqlStep(query)))
      {
        int buyerId = NWScript.SqlGetInt(query, 3);
        int sellerId = NWScript.SqlGetInt(query, 0);
        int auctionId = NWScript.SqlGetInt(query, 1);

        NwPlayer oSeller = NwModule.Instance.Players.FirstOrDefault(p => ObjectPlugin.GetInt(p, "characterId") == sellerId);
        NwStore store = NwModule.FindObjectsOfType<NwStore>().FirstOrDefault(p => p.GetLocalVariable<int>("_AUCTION_ID").Value == auctionId);

        if (buyerId <= 0) // pas d'acheteur
        {
          // S'il est co, on rend l'item au seller et on détruit la ligne en BDD. S'il est pas co, on attend la prochaine occurence pour lui rendre l'item
          if (oSeller != null)
          {
            NwStore tempStore = ObjectPlugin.Deserialize(NWScript.SqlGetString(query, 4)).ToNwObject<NwStore>();
            NwItem tempItem = tempStore.Items.FirstOrDefault();
            tempItem.Clone(oSeller, null, true);
            NwItem authorization = NwItem.Create("auction_clearanc", oSeller.Location);
            oSeller.AcquireItem(authorization);
            oSeller.SendServerMessage($"Aucune enchère sur votre {tempItem.Name.ColorString(API.Color.ORANGE)}. L'objet vous a donc été restitué.");

            Task delayedDeletion = NwTask.Run(async () =>
            {
              await NwTask.Delay(TimeSpan.FromSeconds(0.2));
              DeleteExpiredAuction(auctionId);
            });
          }
        }
        else
        {
          // Si highestAuction > 0 On donne les sous au seller et on lui envoie un message s'il est co. S'il n'est pas co on met à jour en bdd et on lui envoie un courrier
          int highestAuction = NWScript.SqlGetInt(query, 2);

          if(highestAuction > 0)
          {
            if (oSeller != null)
            {
              if (PlayerSystem.Players.TryGetValue(oSeller, out PlayerSystem.Player seller))
              {
                seller.bankGold += highestAuction * 95 / 100;
                oSeller.SendServerMessage($"Votre enchère vous a permis de remporter {(highestAuction * 95 / 100).ToString().ColorString(API.Color.ORANGE)}. L'or a été versé à votre banque !");
              }

              NwItem authorization = NwItem.Create("auction_clearanc", oSeller.Location);
              oSeller.AcquireItem(authorization);
            }
            else 
            {
              var buyerQuery = NWScript.SqlPrepareQueryCampaign(Config.database, $"UPDATE playerCharacters SET bankGold = bankGold + @gold where characterId = @characterId");
              NWScript.SqlBindInt(buyerQuery, "@characterId", sellerId);
              NWScript.SqlBindInt(buyerQuery, "@gold", highestAuction);
              NWScript.SqlStep(buyerQuery);

              // TODO : envoyer un courrier au vendeur
            }
          }
          // Si le buyer est co, on lui file l'item et on détruit la ligne en BDD. S'il est pas co, on met highestAuction à 0 et on attend la prochaine occurence
          NwPlayer oBuyer = NwModule.Instance.Players.FirstOrDefault(p => ObjectPlugin.GetInt(p, "characterId") == buyerId);

          if (oBuyer != null)
          {
            NwStore tempStore = ObjectPlugin.Deserialize(NWScript.SqlGetString(query, 4)).ToNwObject<NwStore>();
            NwItem tempItem = tempStore.Items.FirstOrDefault();
            tempItem.Clone(oSeller, null, true);
            oSeller.SendServerMessage($"Vous venez de remporter l'enchère sur {tempItem.Name.ColorString(API.Color.ORANGE)}. L'objet se trouve désormais dans votre inventaire.");

            Task delayedDeletion = NwTask.Run(async () =>
            {
              await NwTask.Delay(TimeSpan.FromSeconds(0.2));
              DeleteExpiredAuction(auctionId);
            });
          }
          else
          {
            Task delayedUpdate = NwTask.Run(async () =>
            {
              await NwTask.Delay(TimeSpan.FromSeconds(0.2));
              UpdateExpiredAuction(auctionId);
            });
          }
        }

        // On détruit la shop et le panel s'ils ne sont pas null
        if (store != null)
          store.Destroy();

        NwPlaceable panel = NwModule.FindObjectsOfType<NwPlaceable>().FirstOrDefault(p => p.GetLocalVariable<int>("_AUCTION_ID").Value == auctionId);
        if (panel != null)
          panel.Destroy();
      }
    }
    private void DeleteExpiredAuction(int auctionId)
    {
      var deletionQuery = NWScript.SqlPrepareQueryCampaign(Config.database, $"DELETE from playerAuctions where rowid = @rowid");
      NWScript.SqlBindInt(deletionQuery, "@rowid", auctionId);
      NWScript.SqlStep(deletionQuery);
    }
    private void UpdateExpiredAuction(int auctionId)
    {
      var deletionQuery = NWScript.SqlPrepareQueryCampaign(Config.database, $"UPDATE playerAuctions set highestAuction = 0 where rowid = @rowid");
      NWScript.SqlBindInt(deletionQuery, "@rowid", auctionId);
      NWScript.SqlStep(deletionQuery);
    }
    public void RestoreDMPersistentPlaceableFromDatabase()
    {
      var query = NWScript.SqlPrepareQueryCampaign(Config.database, $"SELECT serializedPlaceable, areaTag, position, facing FROM dm_persistant_placeable");

      while (Convert.ToBoolean(NWScript.SqlStep(query)))
        NWScript.SqlGetObject(query, 0, Utils.GetLocationFromDatabase(NWScript.SqlGetString(query, 1), NWScript.SqlGetVector(query, 2), NWScript.SqlGetFloat(query, 3)));
    }

    [ScriptHandler("before_elc")]
    private void HandleBeforeELCValidation(CallInfo callInfo)
    {
      int characterId = ObjectPlugin.GetInt(callInfo.ObjectSelf, "characterId");

      if (characterId > 0)
      {
        var query = NWScript.SqlPrepareQueryCampaign(Config.database, $"SELECT areaTag, position, facing from playerCharacters where rowid = @characterId");
        NWScript.SqlBindInt(query, "@characterId", characterId);
        NWScript.SqlStep(query);

        API.Location loc = Utils.GetLocationFromDatabase(NWScript.SqlGetString(query, 0), NWScript.SqlGetVector(query, 1), NWScript.SqlGetFloat(query, 2));
        NwWaypoint wp = NwWaypoint.Create("NW_WAYPOINT001", loc, false, $"wp_start_{NWScript.GetPCPublicCDKey(callInfo.ObjectSelf)}");
        PlayerPlugin.SetPersistentLocation(NWScript.GetPCPublicCDKey(callInfo.ObjectSelf), PlayerPlugin.GetBicFileName(callInfo.ObjectSelf), wp);
      }
    }

    [ScriptHandler("on_elc_check")]
    private void HandleELCValidation(CallInfo callInfo)
    {
      int characterId = ObjectPlugin.GetInt(callInfo.ObjectSelf, "characterId");
      if (characterId > 0)
      {
        ElcPlugin.SkipValidationFailure();
      }
      else
      {
        int validationFailureType = ElcPlugin.GetValidationFailureType();
        int validationFailureSubType = ElcPlugin.GetValidationFailureSubType();

        if (validationFailureType == ElcPlugin.NWNX_ELC_VALIDATION_FAILURE_TYPE_CHARACTER && validationFailureSubType == 15 && ((NwPlayer)callInfo.ObjectSelf).GetAbilityScore(API.Constants.Ability.Intelligence, true) < 11)
          ElcPlugin.SkipValidationFailure();
        else
          Utils.LogMessageToDMs($"ELC VALIDATION FAILURE - Player {NWScript.GetPCPlayerName(callInfo.ObjectSelf)} - Character {NWScript.GetName(callInfo.ObjectSelf)} - type : {validationFailureType} - SubType : {validationFailureSubType}");
      }
    }
  }
}
