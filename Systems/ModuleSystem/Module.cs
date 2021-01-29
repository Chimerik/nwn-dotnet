using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using NWN.Core;
using NWN.Core.NWNX;
using static NWN.Systems.Craft.Collect.Config;

namespace NWN.Systems
{
  class Module
  {
    public uint oid { get; }
    public List<string> botAsyncCommandList { get; set; }
    public static string currentScript = "";
    public Module(uint oid)
    {
      this.oid = oid;
      NWScript.SetLocalString(oid, "X2_S_UD_SPELLSCRIPT", "spellhook");
      this.botAsyncCommandList = new List<string>();
      Bot.MainAsync();
      this.CreateDatabase();
      this.SetModuleTime();
      ChatSystem.Init();
      try
      {
        LootSystem.InitChestArea();
        InitAreas();
      }
      catch (Exception e)
      {
        Utils.LogException(e);
      }

      this.InitializeEvents();
      this.InitializeFeatModifiers();

      NWScript.DelayCommand(600.0f, () => SaveServerVault());

      RestorePlayerCorpseFromDatabase();
      RestoreDMPersistentPlaceableFromDatabase();

      float resourceRespawnTime;
      if (DateTime.Now.Hour < 5)
        resourceRespawnTime = (float)(TimeSpan.Parse("05:00:00") - DateTime.Now.TimeOfDay).TotalSeconds;
      else
        resourceRespawnTime = (float)((DateTime.Now.AddDays(1).Date).AddHours(5) - DateTime.Now).TotalSeconds;

      NWScript.DelayCommand(resourceRespawnTime, () => SpawnCollectableResources(resourceRespawnTime));

      if (Config.env == Config.Env.Prod)
        NWScript.DelayCommand(5.0f, () => (Bot._client.GetChannel(786218144296468481) as IMessageChannel).SendMessageAsync($"Module en ligne !"));
    }

    private void CreateDatabase()
    {
      var query = NWScript.SqlPrepareQueryCampaign(ModuleSystem.database, "CREATE TABLE IF NOT EXISTS moduleInfo('year' INTEGER NOT NULL, 'month' INTEGER NOT NULL, 'day' INTEGER NOT NULL, 'hour' INTEGER NOT NULL, 'minute' INTEGER NOT NULL, 'second' INTEGER NOT NULL)");
      NWScript.SqlStep(query);

      query = NWScript.SqlPrepareQueryCampaign(ModuleSystem.database, "CREATE TABLE IF NOT EXISTS PlayerAccounts('accountName' TEXT NOT NULL, 'cdKey' TEXT, 'bonusRolePlay' INTEGER NOT NULL, 'discordId' INTEGER, 'rank' TEXT)");
      NWScript.SqlStep(query);

      query = NWScript.SqlPrepareQueryCampaign(ModuleSystem.database, "CREATE TABLE IF NOT EXISTS playerCharacters('accountId' INTEGER NOT NULL, 'characterName' TEXT NOT NULL, 'dateLastSaved' TEXT NOT NULL, 'currentSkillType' INTEGER NOT NULL, 'currentSkillJob' INTEGER NOT NULL, 'currentCraftJobRemainingTime' REAL, 'currentCraftJob' INTEGER NOT NULL, 'currentCraftObject' TEXT NOT NULL, currentCraftJobMaterial TEXT, 'frostAttackOn' INTEGER NOT NULL, areaTag TEXT, position TEXT, facing REAL, currentHP INTEGER, bankGold INTEGER, menuOriginTop INTEGER, menuOriginLeft INTEGER, storage TEXT)");
      NWScript.SqlStep(query);

      query = NWScript.SqlPrepareQueryCampaign(ModuleSystem.database, "CREATE TABLE IF NOT EXISTS playerLearnableSkills('characterId' INTEGER NOT NULL, 'skillId' INTEGER NOT NULL, 'skillPoints' INTEGER NOT NULL, 'trained' INTEGER, UNIQUE (characterId, skillId))");
      NWScript.SqlStep(query);

      query = NWScript.SqlPrepareQueryCampaign(ModuleSystem.database, "CREATE TABLE IF NOT EXISTS playerLearnableSpells('characterId' INTEGER NOT NULL, 'skillId' INTEGER NOT NULL, 'skillPoints' INTEGER NOT NULL, 'trained' INTEGER, UNIQUE (characterId, skillId))");
      NWScript.SqlStep(query);

      query = NWScript.SqlPrepareQueryCampaign(ModuleSystem.database, "CREATE TABLE IF NOT EXISTS playerMaterialStorage('characterId' INTEGER NOT NULL, 'materialName' TEXT NOT NULL, 'materialStock' INTEGER, UNIQUE (characterId, materialName))");
      NWScript.SqlStep(query);

      query = NWScript.SqlPrepareQueryCampaign(ModuleSystem.database, "CREATE TABLE IF NOT EXISTS playerDeathCorpses('characterId' INTEGER NOT NULL, 'deathCorpse' TEXT NOT NULL, 'areaTag' TEXT NOT NULL, 'position' TEXT NOT NULL)");
      NWScript.SqlStep(query);

      query = NWScript.SqlPrepareQueryCampaign(ModuleSystem.database, $"CREATE TABLE IF NOT EXISTS loot_containers('chestTag' TEXT NOT NULL, 'accountID' INTEGER NOT NULL, 'serializedChest' TEXT NOT NULL, 'position' TEXT NOT NULL, 'facing' REAL NOT NULL, PRIMARY KEY(chestTag))");
      NWScript.SqlStep(query);

      query = NWScript.SqlPrepareQueryCampaign(ModuleSystem.database, $"CREATE TABLE IF NOT EXISTS dm_persistant_placeable('accountID' INTEGER NOT NULL, 'serializedPlaceable' TEXT NOT NULL, 'areaTag' TEXT NOT NULL, 'position' TEXT NOT NULL, 'facing' REAL NOT NULL)");
      NWScript.SqlStep(query);

      query = NWScript.SqlPrepareQueryCampaign(ModuleSystem.database, "CREATE TABLE IF NOT EXISTS playerMapPins('characterId' INTEGER NOT NULL, 'mapPinId' INTEGER NOT NULL, 'areaTag' TEXT NOT NULL, 'x' REAL NOT NULL, 'y' REAL NOT NULL, 'note' TEXT, UNIQUE (characterId, mapPinId))");
      NWScript.SqlStep(query);

      query = NWScript.SqlPrepareQueryCampaign(ModuleSystem.database, "CREATE TABLE IF NOT EXISTS playerDescriptions('characterId' INTEGER NOT NULL, 'descriptionName' TEXT NOT NULL, 'description' TEXT NOT NULL, UNIQUE (characterId, descriptionName))");
      NWScript.SqlStep(query);

      query = NWScript.SqlPrepareQueryCampaign(ModuleSystem.database, "CREATE TABLE IF NOT EXISTS areaResourceStock('areaTag' TEXT NOT NULL, 'mining' INTEGER, 'wood' INTEGER, 'animals' INTEGER, PRIMARY KEY(areaTag))");
      NWScript.SqlStep(query);

      query = NWScript.SqlPrepareQueryCampaign(ModuleSystem.database, $"CREATE TABLE IF NOT EXISTS scriptPerformance('script' TEXT NOT NULL, 'nbExecutions' INTEGER NOT NULL, 'averageExecutionTime' REAL NOT NULL, 'cumulatedExecutionTime' REAL NOT NULL, PRIMARY KEY(script))");
      NWScript.SqlStep(query);

      query = NWScript.SqlPrepareQueryCampaign(ModuleSystem.database, $"CREATE TABLE IF NOT EXISTS goldBalance('lootedTag' TEXT NOT NULL, 'nbTimesLooted' INTEGER NOT NULL, 'averageGold' INT NOT NULL, 'cumulatedGold' INT NOT NULL, PRIMARY KEY(lootedTag))");
      NWScript.SqlStep(query);
    }
    private void InitializeEvents()
    {
      NWScript.SetEventScript(this.oid, NWScript.EVENT_SCRIPT_MODULE_ON_PLAYER_TARGET, "on_pc_target");

      EventsPlugin.SubscribeEvent("NWNX_ON_CLIENT_DISCONNECT_BEFORE", "player_exit_before");
      EventsPlugin.ToggleDispatchListMode("NWNX_ON_CLIENT_DISCONNECT_BEFORE", "player_exit_before", 1);

      EventsPlugin.SubscribeEvent("NWNX_ON_ADD_ASSOCIATE_AFTER", "summon_add_after");
      EventsPlugin.ToggleDispatchListMode("NWNX_ON_ADD_ASSOCIATE_AFTER", "summon_add_after", 1);
      EventsPlugin.SubscribeEvent("NWNX_ON_REMOVE_ASSOCIATE_AFTER", "summon_remove_after");
      EventsPlugin.ToggleDispatchListMode("NWNX_ON_REMOVE_ASSOCIATE_AFTER", "summon_remove_after", 1);

      EventsPlugin.SubscribeEvent("NWNX_ON_BROADCAST_CAST_SPELL_AFTER", "event_spellbroadcast_after");
      EventsPlugin.ToggleDispatchListMode("NWNX_ON_BROADCAST_CAST_SPELL_AFTER", "event_spellbroadcast_after", 1);
      EventsPlugin.SubscribeEvent("NWNX_ON_CAST_SPELL_BEFORE", "_onspellcast_before");
      EventsPlugin.ToggleDispatchListMode("NWNX_ON_CAST_SPELL_BEFORE", "_onspellcast_before", 1);
      EventsPlugin.SubscribeEvent("NWNX_ON_CAST_SPELL_AFTER", "_onspellcast_after");
      EventsPlugin.ToggleDispatchListMode("NWNX_ON_CAST_SPELL_AFTER", "_onspellcast_after", 1);
      //EventsPlugin.SubscribeEvent("NWNX_ON_SPELL_INTERRUPTED_AFTER", "_onspellinterrupted_after");
      //EventsPlugin.ToggleDispatchListMode("NWNX_ON_SPELL_INTERRUPTED_AFTER", "_onspellinterrupted_after", 1);

      EventsPlugin.SubscribeEvent("NWNX_ON_ITEM_SCROLL_LEARN_BEFORE", "event_learn_scroll_before");

      EventsPlugin.SubscribeEvent("NWNX_ON_INPUT_TOGGLE_PAUSE_BEFORE", "event_spacebar_down");
      EventsPlugin.ToggleDispatchListMode("NWNX_ON_INPUT_TOGGLE_PAUSE_BEFORE", "event_spacebar_down", 1);

      EventsPlugin.SubscribeEvent("NWNX_ON_ITEM_EQUIP_BEFORE", "event_equip_items_before");
      EventsPlugin.ToggleDispatchListMode("NWNX_ON_ITEM_EQUIP_BEFORE", "event_equip_items_before", 1);
      EventsPlugin.SubscribeEvent("NWNX_ON_ITEM_UNEQUIP_BEFORE", "event_unequip_items_before");
      EventsPlugin.ToggleDispatchListMode("NWNX_ON_ITEM_UNEQUIP_BEFORE", "event_unequip_items_before", 1);
      EventsPlugin.SubscribeEvent("NWNX_ON_VALIDATE_ITEM_EQUIP_BEFORE", "event_validate_equip_items_before");
      EventsPlugin.ToggleDispatchListMode("NWNX_ON_VALIDATE_ITEM_EQUIP_BEFORE", "event_validate_equip_items_before", 1);
      EventsPlugin.SubscribeEvent("NWNX_ON_VALIDATE_USE_ITEM_BEFORE", "event_validate_equip_items_before");
      EventsPlugin.ToggleDispatchListMode("NWNX_ON_VALIDATE_USE_ITEM_BEFORE", "event_validate_equip_items_before", 1);
      EventsPlugin.SubscribeEvent("NWNX_ON_USE_ITEM_BEFORE", "event_use_item_before");
      EventsPlugin.ToggleDispatchListMode("NWNX_ON_USE_ITEM_BEFORE", "event_use_item_before", 1);

      EventsPlugin.SubscribeEvent("NWNX_ON_EXAMINE_OBJECT_BEFORE", "event_examine_before");
      EventsPlugin.SubscribeEvent("NWNX_ON_EXAMINE_OBJECT_AFTER", "event_examine_after");

      EventsPlugin.SubscribeEvent("NWNX_ON_SERVER_CHARACTER_SAVE_BEFORE", "event_player_save_before");
      EventsPlugin.SubscribeEvent("NWNX_ON_CLIENT_EXPORT_CHARACTER_BEFORE", "event_player_save_before");
      EventsPlugin.SubscribeEvent("NWNX_ON_SERVER_CHARACTER_SAVE_AFTER", "event_player_save_after");
      EventsPlugin.SubscribeEvent("NWNX_ON_CLIENT_EXPORT_CHARACTER_AFTER", "event_player_save_after");

      EventsPlugin.SubscribeEvent("NWNX_ON_DM_POSSESS_FULL_POWER_BEFORE", "event_dm_possess_before");
      EventsPlugin.SubscribeEvent("NWNX_ON_DM_POSSESS_BEFORE", "event_dm_possess_before");
      EventsPlugin.SubscribeEvent("NWNX_ON_DM_SPAWN_OBJECT_AFTER", "event_dm_spawn_object_after");
      EventsPlugin.SubscribeEvent("NWNX_ON_DM_JUMP_TARGET_TO_POINT_AFTER", "event_dm_jump_target_after");

      EventsPlugin.SubscribeEvent("NWNX_ON_START_COMBAT_ROUND_AFTER", "event_start_combat_after");
      EventsPlugin.ToggleDispatchListMode("NWNX_ON_START_COMBAT_ROUND_AFTER", "event_start_combat_after", 1);
      EventsPlugin.SubscribeEvent("NWNX_ON_COMBAT_MODE_OFF", "event_combatmode");
      EventsPlugin.ToggleDispatchListMode("NWNX_ON_COMBAT_MODE_OFF", "event_combatmode", 1);
      EventsPlugin.SubscribeEvent("NWNX_ON_USE_SKILL_BEFORE", "event_skillused");
      EventsPlugin.ToggleDispatchListMode("NWNX_ON_USE_SKILL_BEFORE", "event_skillused", 1);

      EventsPlugin.SubscribeEvent("NWNX_ON_DO_LISTEN_DETECTION_AFTER", "event_detection_after");
      EventsPlugin.ToggleDispatchListMode("NWNX_ON_DO_LISTEN_DETECTION_AFTER", "event_detection_after", 1);

      EventsPlugin.SubscribeEvent("NWNX_ON_USE_FEAT_BEFORE", "event_feat_used");
      EventsPlugin.ToggleDispatchListMode("NWNX_ON_USE_FEAT_BEFORE", "event_feat_used", 1);

      EventsPlugin.SubscribeEvent("NWNX_ON_EFFECT_REMOVED_AFTER", "event_effects");
      EventsPlugin.ToggleDispatchListMode("NWNX_ON_EFFECT_REMOVED_AFTER", "event_effects", 1);

      EventsPlugin.SubscribeEvent("NWNX_ON_PARTY_ACCEPT_INVITATION_AFTER", "event_party_accept_after");
      EventsPlugin.SubscribeEvent("NWNX_ON_PARTY_LEAVE_BEFORE", "event_party_leave_before");
      EventsPlugin.SubscribeEvent("NWNX_ON_PARTY_LEAVE_AFTER", "event_party_leave_after");
      EventsPlugin.SubscribeEvent("NWNX_ON_PARTY_KICK_BEFORE", "event_party_leave_before");
      EventsPlugin.SubscribeEvent("NWNX_ON_PARTY_KICK_AFTER", "event_party_kick_after");

      EventsPlugin.SubscribeEvent("NWNX_ON_TIMING_BAR_CANCEL_BEFORE", "event_collect_cycle_cancel");
      EventsPlugin.ToggleDispatchListMode("NWNX_ON_TIMING_BAR_CANCEL_BEFORE", "event_collect_cycle_cancel", 1);
      EventsPlugin.SubscribeEvent("NWNX_ON_CLIENT_DISCONNECT_BEFORE", "event_collect_cycle_cancel");
      EventsPlugin.ToggleDispatchListMode("NWNX_ON_CLIENT_DISCONNECT_BEFORE", "event_collect_cycle_cancel", 1);
      EventsPlugin.SubscribeEvent("NWNX_ON_ITEM_EQUIP_BEFORE", "event_collect_cycle_cancel");
      EventsPlugin.ToggleDispatchListMode("NWNX_ON_ITEM_EQUIP_BEFORE", "event_collect_cycle_cancel", 1);
      EventsPlugin.SubscribeEvent("NWNX_ON_ITEM_UNEQUIP_BEFORE", "event_collect_cycle_cancel");
      EventsPlugin.ToggleDispatchListMode("NWNX_ON_ITEM_UNEQUIP_BEFORE", "event_collect_cycle_cancel", 1);
      EventsPlugin.SubscribeEvent("NWNX_ON_START_COMBAT_ROUND_AFTER", "event_collect_cycle_cancel");
      EventsPlugin.ToggleDispatchListMode("NWNX_ON_START_COMBAT_ROUND_AFTER", "event_collect_cycle_cancel", 1);
      EventsPlugin.SubscribeEvent("NWNX_ON_INPUT_CAST_SPELL_BEFORE", "event_collect_cycle_cancel");
      EventsPlugin.ToggleDispatchListMode("NWNX_ON_INPUT_CAST_SPELL_BEFORE", "event_collect_cycle_cancel", 1);

      EventsPlugin.SubscribeEvent("NWNX_ON_SERVER_SEND_AREA_AFTER", "event_after_area_enter");
      EventsPlugin.SubscribeEvent("NWNX_ON_SERVER_SEND_AREA_BEFORE", "event_before_area_exit");
      EventsPlugin.SubscribeEvent("NWNX_ON_CLIENT_DISCONNECT_BEFORE", "event_before_area_exit");

      EventsPlugin.SubscribeEvent("NWNX_ON_JOURNAL_OPEN_AFTER", "event_on_journal_open");
      EventsPlugin.SubscribeEvent("NWNX_ON_JOURNAL_CLOSE_AFTER", "event_on_journal_close");

      EventsPlugin.SubscribeEvent("NWNX_ON_STORE_REQUEST_BUY_BEFORE", "before_store_buy");
      EventsPlugin.SubscribeEvent("NWNX_ON_STORE_REQUEST_SELL_BEFORE", "before_store_sell");

      EventsPlugin.SubscribeEvent("NWNX_ON_SET_NPC_FACTION_REPUTATION_BEFORE", "before_reputation_change");

      EventsPlugin.SubscribeEvent("NWNX_ON_MAP_PIN_ADD_PIN_AFTER", "map_pin_added");
      EventsPlugin.SubscribeEvent("NWNX_ON_MAP_PIN_CHANGE_PIN_AFTER", "map_pin_changed");
      EventsPlugin.SubscribeEvent("NWNX_ON_MAP_PIN_DESTROY_PIN_AFTER", "map_pin_destroyed");

      //EventsPlugin.SubscribeEvent("NWNX_ON_HAS_FEAT_AFTER", "event_has_feat");
    }
    private void InitializeFeatModifiers()
    {
      int feat = (int)Feat.ImprovedStrength;
      int value = 1;
      for (int ability = NWScript.ABILITY_STRENGTH; ability <= NWScript.ABILITY_CHARISMA; ability++)
      {
        value = 1;
        while(value < 6)
        {
          FeatPlugin.SetFeatModifier(feat, FeatPlugin.NWNX_FEAT_MODIFIER_ABILITY, ability, 1);
          value++;
          feat++;
        }
      }

      feat = (int)Feat.ImprovedAnimalEmpathy;
      for (int skill = NWScript.SKILL_ANIMAL_EMPATHY; skill <= NWScript.SKILL_INTIMIDATE; skill++)
      {

        if (skill == NWScript.SKILL_PERSUADE || skill == NWScript.SKILL_APPRAISE || skill == NWScript.SKILL_CRAFT_TRAP)
          continue;

        value = 1;
        while (value < 6)
        {
          SkillFeat skillFeat = new SkillFeat();
          skillFeat.iFeat = feat;
          skillFeat.iSkill = skill;
          skillFeat.iModifier = 1;
          SkillranksPlugin.SetSkillFeat(skillFeat, 1);
          value++;
          feat++;
        }
      }

      value = 1;  
      for (int attackBonusfeat = (int)Feat.ImprovedAttackBonus; attackBonusfeat < (int)Feat.ImprovedAttackBonus + 5; attackBonusfeat++)
      {
        FeatPlugin.SetFeatModifier(attackBonusfeat, FeatPlugin.NWNX_FEAT_MODIFIER_AB, 1);
        value++;
      }

      feat = (int)Feat.ImprovedSpellSlot0_1;
      for (int spellLevel = 0; spellLevel < 10; spellLevel++)
      {
        value = 1;
        while (value < 11)
        {
          FeatPlugin.SetFeatModifier(feat, 22, 43, spellLevel, 1); // 22 = NWNX_FEAT_MODIFIER_BONUSSPELL, 43 = class aventurier
          value++;
          feat++;
        }
      }

      feat = (int)Feat.ImprovedSavingThrowAll;
      for (int savingThrow = NWScript.SAVING_THROW_ALL; savingThrow < NWScript.SAVING_THROW_WILL; savingThrow++)
      {
        value = 1;
        while (value < 6)
        {
          FeatPlugin.SetFeatModifier(feat, FeatPlugin.NWNX_FEAT_MODIFIER_SAVE, savingThrow, 1);
          value++;
          feat++;
        }
      }
    }

    private static void SaveServerVault()
    {
      var query = NWScript.SqlPrepareQueryCampaign(ModuleSystem.database, $"UPDATE moduleInfo SET year = @year, month = @month, day = @day, hour = @hour, minute = @minute, second = @second where rowid = 1");
      NWScript.SqlBindInt(query, "@year", NWScript.GetCalendarYear());
      NWScript.SqlBindInt(query, "@month", NWScript.GetCalendarMonth());
      NWScript.SqlBindInt(query, "@day", NWScript.GetCalendarDay());
      NWScript.SqlBindInt(query, "@hour", NWScript.GetTimeHour());
      NWScript.SqlBindInt(query, "@minute", NWScript.GetTimeMinute());
      NWScript.SqlBindInt(query, "@second", NWScript.GetTimeSecond());
      NWScript.SqlStep(query);

      NWScript.ExportAllCharacters();

      foreach (KeyValuePair<string, ScriptPerf> perfentry in ModuleSystem.scriptPerformanceMonitoring)
      {
        query = NWScript.SqlPrepareQueryCampaign(ModuleSystem.database, $"INSERT INTO scriptPerformance (script, nbExecutions, averageExecutionTime, cumulatedExecutionTime) VALUES (@script, @nbExecutions, @averageExecutionTime, @cumulatedExecutionTime)" +
        "ON CONFLICT (script) DO UPDATE SET nbExecutions = nbExecutions + @nbExecutions, averageExecutionTime = (cumulatedExecutionTime + @cumulatedExecutionTime) / (nbExecutions + @nbExecutions), cumulatedExecutionTime = cumulatedExecutionTime + @cumulatedExecutionTime");
        NWScript.SqlBindString(query, "@script", perfentry.Key);
        NWScript.SqlBindInt(query, "@nbExecutions", perfentry.Value.nbExecution);
        NWScript.SqlBindFloat(query, "@cumulatedExecutionTime", (float)perfentry.Value.cumulatedExecutionTime);
        NWScript.SqlBindFloat(query, "@averageExecutionTime", (float)perfentry.Value.cumulatedExecutionTime / perfentry.Value.nbExecution);
        NWScript.SqlStep(query);

        perfentry.Value.cumulatedExecutionTime = 0;
        perfentry.Value.nbExecution = 0;
      }

      foreach (KeyValuePair<string, GoldBalance> goldEntry in ModuleSystem.goldBalanceMonitoring)
      {
        query = NWScript.SqlPrepareQueryCampaign(ModuleSystem.database, $"INSERT INTO goldBalance (lootedTag, nbTimesLooted, averageGold, cumulatedGold) VALUES (@lootedTag, @nbTimesLooted, @averageGold, @cumulatedGold)" +
        "ON CONFLICT (lootedTag) DO UPDATE SET nbTimesLooted = nbTimesLooted + @nbTimesLooted, averageGold = (cumulatedGold + @cumulatedGold) / (nbTimesLooted + @nbTimesLooted), cumulatedGold = cumulatedGold + @cumulatedGold");
        NWScript.SqlBindString(query, "@lootedTag", goldEntry.Key);
        NWScript.SqlBindInt(query, "@nbTimesLooted", goldEntry.Value.nbTimesLooted);
        NWScript.SqlBindInt(query, "@cumulatedGold", goldEntry.Value.cumulatedGold);
        NWScript.SqlBindInt(query, "@averageGold", goldEntry.Value.cumulatedGold / goldEntry.Value.nbTimesLooted);
        NWScript.SqlStep(query);

        goldEntry.Value.cumulatedGold = 0;
        goldEntry.Value.nbTimesLooted = 0;
      }

      Bot._client.DownloadUsersAsync(new List<IGuild> { { Bot._client.GetGuild(680072044364562528) } });
      NWScript.DelayCommand(600.0f, () => SaveServerVault());
    }
    public void RestorePlayerCorpseFromDatabase()
    {
      var query = NWScript.SqlPrepareQueryCampaign(ModuleSystem.database, $"SELECT deathCorpse, areaTag, position, characterId FROM playerDeathCorpses");

      while (Convert.ToBoolean(NWScript.SqlStep(query)))
      {
        uint corpse = NWScript.SqlGetObject(query, 0, Utils.GetLocationFromDatabase(NWScript.SqlGetString(query, 1), NWScript.SqlGetVector(query, 2), 0));
        NWScript.SetLocalInt(corpse, "_PC_ID", NWScript.SqlGetInt(query, 3));

        var oObj = NWScript.GetFirstItemInInventory(corpse);

        while (Convert.ToBoolean(NWScript.GetIsObjectValid(oObj)))
        {
          if (NWScript.GetTag(oObj) != "item_pccorpse")
            NWScript.DestroyObject(oObj);
          oObj = NWScript.GetNextItemInInventory(corpse);
        }

        NWScript.DelayCommand(3.0f, () => PlayerSystem.SetupPCCorpse(corpse));
      }
    }
    public void RestoreDMPersistentPlaceableFromDatabase()
    {
      var query = NWScript.SqlPrepareQueryCampaign(ModuleSystem.database, $"SELECT serializedPlaceable, areaTag, position, facing FROM dm_persistant_placeable");

      while (Convert.ToBoolean(NWScript.SqlStep(query)))
        NWScript.SqlGetObject(query, 0, Utils.GetLocationFromDatabase(NWScript.SqlGetString(query, 1), NWScript.SqlGetVector(query, 2), NWScript.SqlGetFloat(query, 3)));
    }
    private void InitAreas()
    {
      var oArea = NWScript.GetFirstArea();

      while (Convert.ToBoolean(NWScript.GetIsObjectValid(oArea)))
      {
        AreaSystem.CreateArea(oArea);
        oArea = NWScript.GetNextArea();
      }
    }
    public async Task PreparingModuleForAsyncReboot(SocketCommandContext context)
    {
      if (Utils.GetPlayerStaffRankFromDiscord(context.User.Id) == "admin")
      {
        this.botAsyncCommandList.Add("reboot");
        await context.Channel.SendMessageAsync("Reboot effectif dans 30 secondes.");
        return;
      }

      await context.Channel.SendMessageAsync("Noooon, vous n'êtes pas la maaaaaître ! Le maaaaître est bien plus poli, d'habitude !");
      return;
    }
    public async Task PreparingModuleForAsyncRefill(SocketCommandContext context)
    {
      if (Utils.GetPlayerStaffRankFromDiscord(context.User.Id) == "admin")
      {
        this.botAsyncCommandList.Add("refill");
        await context.Channel.SendMessageAsync("Refill en cours.");
        return;
      }

      await context.Channel.SendMessageAsync("Noooon, vous n'êtes pas la maaaaaître ! Le maaaaître est bien plus poli, d'habitude !");
      return;
    }
    public async Task PreparingModuleForAsyncSay(SocketCommandContext context, string sPCName, string text)
    {
      int result = Utils.CheckPlayerCredentialsFromDiscord(context, sPCName);
      if (result > 0)
      {
        this.botAsyncCommandList.Add($"say_{result}_{text}");
        await context.Channel.SendMessageAsync("Texte en cours de relais vers votre personnage.");
        return;
      }

      await context.Channel.SendMessageAsync("Le personnage indiqué n'existe pas, n'est pas connecté ou n'a pas été enregistré avec votre code Discord et votre clef cd.");
    }
    private void SetModuleTime()
    {
      var query = NWScript.SqlPrepareQueryCampaign(ModuleSystem.database, $"SELECT year, month, day, hour, minute, second from moduleInfo where rowid = 1");
      
      if(Convert.ToBoolean(NWScript.SqlStep(query)))
      {
        NWScript.SetCalendar(NWScript.SqlGetInt(query, 0), NWScript.SqlGetInt(query, 1), NWScript.SqlGetInt(query, 2));
        NWScript.SetTime(NWScript.SqlGetInt(query, 3), NWScript.SqlGetInt(query, 4), NWScript.SqlGetInt(query, 5), 0);
      }
      else
      {
        query = NWScript.SqlPrepareQueryCampaign(ModuleSystem.database, $"INSERT INTO moduleInfo (year, month, day, hour, minute, second) VALUES (@year, @month, @day, @hour, @minute, @second)");
        NWScript.SqlBindInt(query, "@year", NWScript.GetCalendarYear());
        NWScript.SqlBindInt(query, "@month", NWScript.GetCalendarMonth());
        NWScript.SqlBindInt(query, "@day", NWScript.GetCalendarDay());
        NWScript.SqlBindInt(query, "@hour", NWScript.GetTimeHour());
        NWScript.SqlBindInt(query, "@minute", NWScript.GetTimeMinute());
        NWScript.SqlBindInt(query, "@second", NWScript.GetTimeSecond());
        NWScript.SqlStep(query);
      }      
    }
    public void SpawnCollectableResources(float delay)
    {
      uint resourcePoint = NWScript.GetObjectByTag("ore_spawn_wp");
      int i = 0;

      while(Convert.ToBoolean(NWScript.GetIsObjectValid(resourcePoint)))
      {
        if (AreaSystem.areaDictionnary.TryGetValue(NWScript.GetObjectUUID(NWScript.GetArea(resourcePoint)), out Area area))
        {
          if (area.level > 1)
          {
            if (Utils.random.Next(1, 101) >= (area.level * 20) - 20)
            {
              uint newRock = NWScript.CreateObject(NWScript.OBJECT_TYPE_PLACEABLE, "mineable_rock", NWScript.GetLocation(resourcePoint));
              NWScript.SetName(newRock, Enum.GetName(typeof(OreType), GetRandomOreSpawnFromAreaLevel(area.level)));
              NWScript.SetLocalInt(newRock, "_ORE_AMOUNT", 50 * Utils.random.Next(1, 101));
              NWScript.DestroyObject(resourcePoint);
            }
          }
          else
            NWScript.DelayCommand(5.0f, () => Utils.LogMessageToDMs($"[Resource Spawn System] - Attention, un point de ressource a été déposé dans une zone de niveau < 2 : {area.name} - {area.tag}"));
        }
          
        i++;
        resourcePoint = NWScript.GetObjectByTag("ore_spawn_wp", i);
      }

      resourcePoint = NWScript.GetObjectByTag("wood_spawn_wp");
      i = 0;

      while (Convert.ToBoolean(NWScript.GetIsObjectValid(resourcePoint)))
      {
        if (AreaSystem.areaDictionnary.TryGetValue(NWScript.GetObjectUUID(NWScript.GetArea(resourcePoint)), out Area area))
        {
          if (area.level > 1)
          {
            if (Utils.random.Next(1, 101) >= (area.level * 20) - 20)
            {
              uint newRock = NWScript.CreateObject(NWScript.OBJECT_TYPE_PLACEABLE, "mineable_tree", NWScript.GetLocation(resourcePoint));
              NWScript.SetName(newRock, Enum.GetName(typeof(WoodType), GetRandomWoodSpawnFromAreaLevel(area.level)));
              NWScript.SetLocalInt(newRock, "_ORE_AMOUNT", 50 * Utils.random.Next(1, 101));
              NWScript.DestroyObject(resourcePoint);
            }
          }
          else
            NWScript.DelayCommand(5.0f, () => Utils.LogMessageToDMs($"[Resource Spawn System] - Attention, un point de ressource a été déposé dans une zone de niveau < 2 : {area.name} - {area.tag}"));
        }

        i++;
        resourcePoint = NWScript.GetObjectByTag("wood_spawn_wp", i);
      }

      foreach(Area area in AreaSystem.areaDictionnary.Values)
      {
        var query = NWScript.SqlPrepareQueryCampaign(ModuleSystem.database, $"INSERT INTO areaResourceStock (areaTag, mining, wood, animals) VALUES (@areaTag, @mining, @wood, @animals)" +
        "ON CONFLICT (areaTag) DO UPDATE SET mining = @mining, wood = @wood, animals = @animals");
        NWScript.SqlBindString(query, "@areaTag", area.tag);
        NWScript.SqlBindInt(query, "@mining", area.level * 2);
        NWScript.SqlBindInt(query, "@wood", area.level * 2);
        NWScript.SqlBindInt(query, "@animals", area.level * 2);
        NWScript.SqlStep(query);
      }

      if (delay > 0.0f)
        NWScript.DelayCommand(86400.0f, () => SpawnCollectableResources(delay));  //24 h plus tard
    }
  }
}
