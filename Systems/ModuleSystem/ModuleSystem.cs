﻿using Discord;
using Google.Cloud.Translation.V2;
using Microsoft.Data.Sqlite;
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
    public static readonly TranslationClient googleTranslationClient = TranslationClient.Create();
    public static Dictionary<string, GoldBalance> goldBalanceMonitoring = new Dictionary<string, GoldBalance>();
    public ModuleSystem(NativeEventService eventService)
    {
      eventService.Subscribe<NwModule, ModuleEvents.OnModuleStart>(NwModule.Instance, OnModuleStart);
      eventService.Subscribe<NwModule, ModuleEvents.OnModuleLoad>(NwModule.Instance, OnModuleLoad);
    }
    private void OnModuleStart(ModuleEvents.OnModuleStart onModuleStart)
    {
      ;
    }
    private void OnModuleLoad(ModuleEvents.OnModuleLoad onModuleLoad)
    {
      //DoAsyncStuff();
      LoadDiscordBot();
      //Console.WriteLine($" enum name : {Feat.LegendaryPeltReprocessing5.ToDescription()}", Color.PINK);

      NwModule.Instance.GetLocalVariable<string>("X2_S_UD_SPELLSCRIPT").Value = "spellhook";

      CreateDatabase();
      SetModuleTime();
      InitializeEvents();
      FeatSystem.InitializeFeatModifiers();

      SaveServerVault();

      RestorePlayerCorpseFromDatabase();
      RestoreDMPersistentPlaceableFromDatabase();

      float resourceRespawnTime;
      if (DateTime.Now.Hour < 5)
        resourceRespawnTime = (float)(TimeSpan.Parse("05:00:00") - DateTime.Now.TimeOfDay).TotalSeconds;
      else
        resourceRespawnTime = (float)((DateTime.Now.AddDays(1).Date).AddHours(5) - DateTime.Now).TotalSeconds;

      Task spawnResources = NwTask.Run(async () =>
      {
        await NwTask.Delay(TimeSpan.FromSeconds(resourceRespawnTime));
        await SpawnCollectableResources(resourceRespawnTime);
        return true;
      });
    }
    private async void DoAsyncStuff()
    {
      await NwTask.WaitUntilValueChanged(() => NwModule.Instance.Players.Count());
      NwModule.Instance.Players.First().SendServerMessage("Message async. C'est chouette, non ?");
    }
    private async void LoadDiscordBot()
    {
      await Bot.MainAsync();
      if (Config.env == Config.Env.Prod)
        await (Bot._client.GetChannel(786218144296468481) as IMessageChannel).SendMessageAsync($"Module en ligne !");
    }

    private void CreateDatabase()
    {
      using (var connection = new SqliteConnection($"{Config.db_path}"))
      {
        connection.Open();

        var command = connection.CreateCommand();
        command.CommandText =
        @"
                CREATE TABLE IF NOT EXISTS moduleInfo('year' INTEGER NOT NULL, 'month' INTEGER NOT NULL, 'day' INTEGER NOT NULL,
                'hour' INTEGER NOT NULL, 'minute' INTEGER NOT NULL, 'second' INTEGER NOT NULL)
                ";
        command.ExecuteNonQuery();

        command = connection.CreateCommand();
        command.CommandText =
        @"
                CREATE TABLE IF NOT EXISTS PlayerAccounts('accountName' TEXT NOT NULL, 'cdKey' TEXT, 'bonusRolePlay' INTEGER NOT NULL,
                'discordId' INTEGER, 'rank' TEXT)
                ";
        command.ExecuteNonQuery();

        command = connection.CreateCommand();
        command.CommandText =
        @"
                CREATE TABLE IF NOT EXISTS playerCharacters('accountId' INTEGER NOT NULL, 'characterName' TEXT NOT NULL,
                'dateLastSaved' TEXT NOT NULL, 'currentSkillType' INTEGER NOT NULL, 'currentSkillJob' INTEGER NOT NULL,
                'currentCraftJobRemainingTime' REAL, 'currentCraftJob' INTEGER NOT NULL, 'currentCraftObject' TEXT NOT NULL,
                currentCraftJobMaterial TEXT, 'frostAttackOn' INTEGER NOT NULL, areaTag TEXT, position TEXT, facing REAL,
                currentHP INTEGER, bankGold INTEGER, menuOriginTop INTEGER, menuOriginLeft INTEGER, storage TEXT)
                ";
        command.ExecuteNonQuery();

        command = connection.CreateCommand();
        command.CommandText =
        @"
                CREATE TABLE IF NOT EXISTS playerLearnableSkills('characterId' INTEGER NOT NULL, 'skillId' INTEGER NOT NULL,
                'skillPoints' INTEGER NOT NULL, 'trained' INTEGER, UNIQUE (characterId, skillId))
                ";
        command.ExecuteNonQuery();

        command = connection.CreateCommand();
        command.CommandText =
        @"
                CREATE TABLE IF NOT EXISTS playerLearnableSpells('characterId' INTEGER NOT NULL, 'skillId' INTEGER NOT NULL,
                'skillPoints' INTEGER NOT NULL, 'trained' INTEGER, UNIQUE (characterId, skillId))
                ";
        command.ExecuteNonQuery();

        command = connection.CreateCommand();
        command.CommandText =
        @"
                CREATE TABLE IF NOT EXISTS playerMaterialStorage('characterId' INTEGER NOT NULL, 'materialName' TEXT NOT NULL,
                'materialStock' INTEGER, UNIQUE (characterId, materialName))
                ";
        command.ExecuteNonQuery();

        command = connection.CreateCommand();
        command.CommandText =
        @"
                CREATE TABLE IF NOT EXISTS playerDeathCorpses('characterId' INTEGER NOT NULL, 'deathCorpse' TEXT NOT NULL,
                'areaTag' TEXT NOT NULL, 'position' TEXT NOT NULL)
                ";
        command.ExecuteNonQuery();

        command = connection.CreateCommand();
        command.CommandText =
        @"
                CREATE TABLE IF NOT EXISTS loot_containers('chestTag' TEXT NOT NULL, 'accountID' INTEGER NOT NULL,
                'serializedChest' TEXT NOT NULL, 'position' TEXT NOT NULL, 'facing' REAL NOT NULL, PRIMARY KEY(chestTag))
                ";
        command.ExecuteNonQuery();

        command = connection.CreateCommand();
        command.CommandText =
        @"
                CREATE TABLE IF NOT EXISTS dm_persistant_placeable('accountID' INTEGER NOT NULL, 'serializedPlaceable' TEXT NOT NULL,
                'areaTag' TEXT NOT NULL, 'position' TEXT NOT NULL, 'facing' REAL NOT NULL)
                ";
        command.ExecuteNonQuery();

        command = connection.CreateCommand();
        command.CommandText =
        @"
                CREATE TABLE IF NOT EXISTS playerMapPins('characterId' INTEGER NOT NULL, 'mapPinId' INTEGER NOT NULL,
                'areaTag' TEXT NOT NULL, 'x' REAL NOT NULL, 'y' REAL NOT NULL, 'note' TEXT, UNIQUE (characterId, mapPinId))
                ";
        command.ExecuteNonQuery();

        command = connection.CreateCommand();
        command.CommandText =
        @"
                CREATE TABLE IF NOT EXISTS playerDescriptions('characterId' INTEGER NOT NULL, 'descriptionName' TEXT NOT NULL,
                'description' TEXT NOT NULL, UNIQUE (characterId, descriptionName))
                ";
        command.ExecuteNonQuery();

        command = connection.CreateCommand();
        command.CommandText =
        @"
                CREATE TABLE IF NOT EXISTS areaResourceStock('areaTag' TEXT NOT NULL, 'mining' INTEGER, 'wood' INTEGER,
                'animals' INTEGER, PRIMARY KEY(areaTag))
                ";
        command.ExecuteNonQuery();

        command = connection.CreateCommand();
        command.CommandText =
        @"
                CREATE TABLE IF NOT EXISTS scriptPerformance('script' TEXT NOT NULL, 'nbExecutions' INTEGER NOT NULL,
                'averageExecutionTime' REAL NOT NULL, 'cumulatedExecutionTime' REAL NOT NULL, PRIMARY KEY(script))
                ";
        command.ExecuteNonQuery();

        command = connection.CreateCommand();
        command.CommandText =
        @"
                CREATE TABLE IF NOT EXISTS goldBalance('lootedTag' TEXT NOT NULL, 'nbTimesLooted' INTEGER NOT NULL,
                'averageGold' INT NOT NULL, 'cumulatedGold' INT NOT NULL, PRIMARY KEY(lootedTag))
                ";
        command.ExecuteNonQuery();
      }
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
      EventsPlugin.SubscribeEvent("NWNX_ON_STORE_REQUEST_SELL_BEFORE", "b_store_sell");

      EventsPlugin.SubscribeEvent("NWNX_ON_MAP_PIN_ADD_PIN_AFTER", "map_pin_added");
      EventsPlugin.SubscribeEvent("NWNX_ON_MAP_PIN_CHANGE_PIN_AFTER", "map_pin_changed");
      EventsPlugin.SubscribeEvent("NWNX_ON_MAP_PIN_DESTROY_PIN_AFTER", "mappin_destroyed");

      EventsPlugin.SubscribeEvent("NWNX_ON_INPUT_EMOTE_BEFORE", "on_input_emote");

      //EventsPlugin.SubscribeEvent("NWNX_ON_HAS_FEAT_AFTER", "event_has_feat");
    }
    private void SetModuleTime()
    {
      using (var connection = new SqliteConnection($"{Config.db_path}"))
      {
        connection.Open();

        var command = connection.CreateCommand();
        command.CommandText =
        @"
                SELECT year, month, day, hour, minute, second from moduleInfo where rowid = 1
                ";

        using (var reader = command.ExecuteReader())
        {
          if (reader.Read())
          {
            NwDateTime.Now = new NwDateTime(reader.GetInt32(0), reader.GetInt32(1), reader.GetInt32(2), reader.GetInt32(3), reader.GetInt32(4), reader.GetInt32(5));
          }
          else
          {
            command = connection.CreateCommand();
            command.CommandText =
            @"
                        INSERT INTO moduleInfo (year, month, day, hour, minute, second) VALUES (@year, @month, @day, @hour, @minute, @second)
                        ";

            command.Parameters.AddWithValue("$year", NwDateTime.Now.Year);
            command.Parameters.AddWithValue("$month", NwDateTime.Now.Month);
            command.Parameters.AddWithValue("$day", NwDateTime.Now.DayInMonth);
            command.Parameters.AddWithValue("$hour", NwDateTime.Now.Hour);
            command.Parameters.AddWithValue("$minute", NwDateTime.Now.Minute);
            command.Parameters.AddWithValue("$second", NwDateTime.Now.Second);
            command.ExecuteNonQuery();
          }
        }
      }
    }
    public static async Task SpawnCollectableResources(float delay)
    {
      foreach (NwPlaceable ressourcePoint in NwModule.FindObjectsWithTag<NwPlaceable>(new string[] { "ore_spawn_wp", "wood_spawn_wp" }).Where(l => l.Area.GetLocalVariable<int>("_AREA_LEVEL").Value > 1))
      {
        int areaLevel = ressourcePoint.Area.GetLocalVariable<int>("_AREA_LEVEL").Value;
        if (NwRandom.Roll(NWN.Utils.random, 100) >= (areaLevel * 20) - 20)
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
        }
      }

      foreach (NwArea area in NwModule.Instance.Areas.Where(l => l.GetLocalVariable<int>("_AREA_LEVEL").Value > 1))
      {
        int areaLevel = area.GetLocalVariable<int>("_AREA_LEVEL").Value;

        using (var connection = new SqliteConnection($"{Config.db_path}"))
        {
          connection.Open();

          var command = connection.CreateCommand();
          command.CommandText =
          @"
                    INSERT INTO areaResourceStock (areaTag, mining, wood, animals) VALUES (@areaTag, @mining, @wood, @animals)
                    ON CONFLICT (areaTag) DO UPDATE SET mining = @mining, wood = @wood, animals = @animals;
                    ";
          command.Parameters.AddWithValue("areaTag", area.Tag);
          command.Parameters.AddWithValue("mining", areaLevel * 2);
          command.Parameters.AddWithValue("wood", areaLevel * 2);
          command.Parameters.AddWithValue("animals", areaLevel * 2);
          command.ExecuteNonQuery();
        }
      }

      if (delay > 0.0f)
      {
        await NwTask.Delay(TimeSpan.FromDays(1));
        await SpawnCollectableResources(delay);
      }
    }
    private async void SaveServerVault()
    {
      await NwTask.Delay(TimeSpan.FromMinutes(10));

      var query = NWScript.SqlPrepareQueryCampaign(Config.database, $"UPDATE moduleInfo SET year = @year, month = @month, day = @day, hour = @hour, minute = @minute, second = @second where rowid = 1");
      NWScript.SqlBindInt(query, "@year", NWScript.GetCalendarYear());
      NWScript.SqlBindInt(query, "@month", NWScript.GetCalendarMonth());
      NWScript.SqlBindInt(query, "@day", NWScript.GetCalendarDay());
      NWScript.SqlBindInt(query, "@hour", NWScript.GetTimeHour());
      NWScript.SqlBindInt(query, "@minute", NWScript.GetTimeMinute());
      NWScript.SqlBindInt(query, "@second", NWScript.GetTimeSecond());
      NWScript.SqlStep(query);

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

      foreach (KeyValuePair<string, GoldBalance> goldEntry in goldBalanceMonitoring)
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
      }

      Task DownloadDiscordUsers = NwTask.Run(async () =>
      {
        await Bot._client.DownloadUsersAsync(new List<IGuild> { { Bot._client.GetGuild(680072044364562528) } });
        return true;
      });

      SaveServerVault();
    }
    public void RestorePlayerCorpseFromDatabase()
    {
      var query = NWScript.SqlPrepareQueryCampaign(Config.database, $"SELECT deathCorpse, areaTag, position, characterId FROM playerDeathCorpses");

      while (Convert.ToBoolean(NWScript.SqlStep(query)))
      {
        NwPlaceable corpse = NWScript.SqlGetObject(query, 0, NWN.Utils.GetLocationFromDatabase(NWScript.SqlGetString(query, 1), NWScript.SqlGetVector(query, 2), 0)).ToNwObject<NwPlaceable>();
        corpse.GetLocalVariable<int>("_PC_ID").Value = NWScript.SqlGetInt(query, 3);

        foreach (NwItem item in corpse.Items.Where(i => i.Tag != "item_pccorpse"))
          item.Destroy();

        Task setupCorpse = NwTask.Run(async () =>
        {
          await NwTask.Delay(TimeSpan.FromSeconds(3));
          PlayerSystem.SetupPCCorpse(corpse);
          PlayerSystem.SetupPCCorpse(corpse);
          return true;
        });
      }
    }
    public void RestoreDMPersistentPlaceableFromDatabase()
    {
      var query = NWScript.SqlPrepareQueryCampaign(Config.database, $"SELECT serializedPlaceable, areaTag, position, facing FROM dm_persistant_placeable");

      while (Convert.ToBoolean(NWScript.SqlStep(query)))
        NWScript.SqlGetObject(query, 0, NWN.Utils.GetLocationFromDatabase(NWScript.SqlGetString(query, 1), NWScript.SqlGetVector(query, 2), NWScript.SqlGetFloat(query, 3)));
    }
  }
}
