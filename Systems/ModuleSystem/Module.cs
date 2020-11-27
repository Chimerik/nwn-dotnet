using System;
using System.Collections.Generic;
using NWN.Core;
using NWN.Core.NWNX;

namespace NWN.Systems
{
  class Module
  {
    public uint oid { get; }
    public List<string> botAsyncCommandList { get; set; }
    public static Dictionary<string, Area> areaDictionnary = new Dictionary<string, Area>();
    public static string currentScript = "";
    public Module(uint oid)
    {
      this.oid = oid;
      NWScript.SetLocalString(oid, "X2_S_UD_SPELLSCRIPT", "spellhook");
      this.botAsyncCommandList = new List<string>();
      Bot.MainAsync();
      this.CreateDatabase();
      ChatSystem.Init();
      try
      {
        LootSystem.InitChestArea();
        InitModuleChestSpawn();
      }
      catch (Exception e)
      {
        Utils.LogException(e);
      }

      this.InitializeEvents();
      this.InitializeFeatModifiers();

      CollectSystem.InitiateOres();

      NWScript.DelayCommand(600.0f, () => SaveServerVault());

      RestorePlayerCorpseFromDatabase();
      RestoreDMPersistentPlaceableFromDatabase();
    }
    private void CreateDatabase()
    {
      var query = NWScript.SqlPrepareQueryCampaign(ModuleSystem.database, "CREATE TABLE IF NOT EXISTS PlayerAccounts('accountName' TEXT NOT NULL, 'bonusRolePlay' INTEGER NOT NULL)");
      NWScript.SqlStep(query);

      query = NWScript.SqlPrepareQueryCampaign(ModuleSystem.database, "CREATE TABLE IF NOT EXISTS playerCharacters('accountId' INTEGER NOT NULL, 'characterName' TEXT NOT NULL, 'dateLastSaved' TEXT NOT NULL, 'currentSkillJob' INTEGER NOT NULL, 'currentCraftJobRemainingTime' REAL, 'currentCraftJob' INTEGER NOT NULL, 'currentCraftObject' TEXT NOT NULL, currentCraftJobMaterial TEXT, 'frostAttackOn' INTEGER NOT NULL, areaTag TEXT, position TEXT, facing REAL, currentHP INTEGER, bankGold INTEGER, menuOriginTop INTEGER, menuOriginLeft INTEGER)");
      NWScript.SqlStep(query);

      query = NWScript.SqlPrepareQueryCampaign(ModuleSystem.database, "CREATE TABLE IF NOT EXISTS playerLearnableSkills('characterId' INTEGER NOT NULL, 'skillId' INTEGER NOT NULL, 'skillPoints' INTEGER NOT NULL, 'trained' INTEGER)");
      NWScript.SqlStep(query);

      query = NWScript.SqlPrepareQueryCampaign(ModuleSystem.database, "CREATE TABLE IF NOT EXISTS playerMaterialStorage('characterId' INTEGER NOT NULL, 'Veldspar' INTEGER, 'Scordite' INTEGER, 'Pyroxeres' INTEGER, 'Tritanium' INTEGER, 'Pyerite' INTEGER, 'Mexallon' INTEGER, 'Noxcium' INTEGER, PRIMARY KEY(characterId))");
      NWScript.SqlStep(query);

      query = NWScript.SqlPrepareQueryCampaign(ModuleSystem.database, "CREATE TABLE IF NOT EXISTS playerDeathCorpses('characterId' INTEGER NOT NULL, 'deathCorpse' TEXT NOT NULL, 'areaTag' TEXT NOT NULL, 'position' TEXT NOT NULL)");
      NWScript.SqlStep(query);

      query = NWScript.SqlPrepareQueryCampaign(ModuleSystem.database, $"CREATE TABLE IF NOT EXISTS loot_containers('chestTag' TEXT NOT NULL, 'accountID' INTEGER NOT NULL, 'serializedChest' TEXT NOT NULL, 'position' TEXT NOT NULL, 'facing' REAL NOT NULL, PRIMARY KEY(chestTag))");
      NWScript.SqlStep(query);

      query = NWScript.SqlPrepareQueryCampaign(ModuleSystem.database, $"CREATE TABLE IF NOT EXISTS dm_persistant_placeable('accountID' INTEGER NOT NULL, 'serializedPlaceable' TEXT NOT NULL, 'areaTag' TEXT NOT NULL, 'position' TEXT NOT NULL, 'facing' REAL NOT NULL)");
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

      EventsPlugin.SubscribeEvent("NWNX_ON_TIMING_BAR_CANCEL_BEFORE", "event_mining_cycle_cancel_before");
      EventsPlugin.ToggleDispatchListMode("NWNX_ON_TIMING_BAR_CANCEL_BEFORE", "event_mining_cycle_cancel_before", 1);
      EventsPlugin.SubscribeEvent("NWNX_ON_CLIENT_DISCONNECT_BEFORE", "event_mining_cycle_cancel_before");
      EventsPlugin.ToggleDispatchListMode("NWNX_ON_CLIENT_DISCONNECT_BEFORE", "event_mining_cycle_cancel_before", 1);
      EventsPlugin.SubscribeEvent("NWNX_ON_ITEM_EQUIP_BEFORE", "event_mining_cycle_cancel_before");
      EventsPlugin.ToggleDispatchListMode("NWNX_ON_ITEM_EQUIP_BEFORE", "event_mining_cycle_cancel_before", 1);
      EventsPlugin.SubscribeEvent("NWNX_ON_ITEM_UNEQUIP_BEFORE", "event_mining_cycle_cancel_before");
      EventsPlugin.ToggleDispatchListMode("NWNX_ON_ITEM_UNEQUIP_BEFORE", "event_mining_cycle_cancel_before", 1);
      EventsPlugin.SubscribeEvent("NWNX_ON_START_COMBAT_ROUND_AFTER", "event_mining_cycle_cancel_before");
      EventsPlugin.ToggleDispatchListMode("NWNX_ON_START_COMBAT_ROUND_AFTER", "event_mining_cycle_cancel_before", 1);
      EventsPlugin.SubscribeEvent("NWNX_ON_INPUT_CAST_SPELL_BEFORE", "event_mining_cycle_cancel_before");
      EventsPlugin.ToggleDispatchListMode("NWNX_ON_INPUT_CAST_SPELL_BEFORE", "event_mining_cycle_cancel_before", 1);

      EventsPlugin.SubscribeEvent("NWNX_ON_INVENTORY_ADD_ITEM_BEFORE", "event_refinery_add_item_before");
      EventsPlugin.ToggleDispatchListMode("NWNX_ON_INVENTORY_ADD_ITEM_BEFORE", "event_refinery_add_item_before", 1);

      EventsPlugin.SubscribeEvent("NWNX_ON_INVENTORY_REMOVE_ITEM_AFTER", "event_inventory_pccorpse_removed_after");
      EventsPlugin.ToggleDispatchListMode("NWNX_ON_INVENTORY_REMOVE_ITEM_AFTER", "event_inventory_pccorpse_removed_after", 1);
      EventsPlugin.SubscribeEvent("NWNX_ON_INVENTORY_REMOVE_ITEM_AFTER", "event_pccorpse_remove_item_after");
      EventsPlugin.ToggleDispatchListMode("NWNX_ON_INVENTORY_REMOVE_ITEM_AFTER", "event_pccorpse_remove_item_after", 1);

      EventsPlugin.SubscribeEvent("NWNX_ON_SERVER_SEND_AREA_AFTER", "event_after_area_enter");
      EventsPlugin.SubscribeEvent("NWNX_ON_SERVER_SEND_AREA_BEFORE", "event_before_area_exit");
      EventsPlugin.SubscribeEvent("NWNX_ON_CLIENT_DISCONNECT_BEFORE", "event_before_area_exit");

      EventsPlugin.SubscribeEvent("NWNX_ON_JOURNAL_OPEN_AFTER", "event_on_journal_open");
      EventsPlugin.SubscribeEvent("NWNX_ON_JOURNAL_CLOSE_AFTER", "event_on_journal_close");

      var refinery = NWScript.GetObjectByTag("refinery", 0);

      int i = 1;
      while (NWScript.GetIsObjectValid(refinery) == 1)
      {
        EventsPlugin.AddObjectToDispatchList("NWNX_ON_INVENTORY_ADD_ITEM_BEFORE", "event_refinery_add_item_before", refinery);
        i++;
        refinery = NWScript.GetObjectByTag("refinery", i);
      }

      EventsPlugin.SubscribeEvent("NWNX_ON_STORE_REQUEST_BUY_BEFORE", "before_store_buy");
      EventsPlugin.SubscribeEvent("NWNX_ON_STORE_REQUEST_SELL_BEFORE", "before_store_sell");
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
          FeatPlugin.SetFeatModifier(feat, FeatPlugin.NWNX_FEAT_MODIFIER_ABILITY, ability, value);
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
          skillFeat.iModifier = value;
          SkillranksPlugin.SetSkillFeat(skillFeat, 1);
          value++;
          feat++;
        }
      }

      value = 1;  
      for (int attackBonusfeat = (int)Feat.ImprovedAttackBonus; attackBonusfeat < (int)Feat.ImprovedAttackBonus + 5; attackBonusfeat++)
      {
        FeatPlugin.SetFeatModifier(attackBonusfeat, FeatPlugin.NWNX_FEAT_MODIFIER_AB, value);
        value++;
      }

      feat = (int)Feat.ImprovedSpellSlot0_1;
      value = 1;
      for (int spellLevel = 0; spellLevel < 10; spellLevel++)
      {
        value = 1;
        while (value < 11)
        {
          FeatPlugin.SetFeatModifier(feat, 22, 43, spellLevel, value); // 22 = NWNX_FEAT_MODIFIER_BONUSSPELL, 43 = class aventurier
          value++;
          feat++;
        }
      }

      feat = (int)Feat.ImprovedSavingThrowAll;
      value = 1;
      for (int savingThrow = NWScript.SAVING_THROW_ALL; savingThrow < NWScript.SAVING_THROW_WILL; savingThrow++)
      {
        value = 1;
        while (value < 6)
        {
          FeatPlugin.SetFeatModifier(feat, FeatPlugin.NWNX_FEAT_MODIFIER_SAVE, savingThrow, value);
          value++;
          feat++;
        }
      }
    }

    private static void SaveServerVault()
    {
      NWScript.ExportAllCharacters();
      NWScript.DelayCommand(600.0f, () => SaveServerVault());
    }
    public void RestorePlayerCorpseFromDatabase()
    {
      var query = NWScript.SqlPrepareQueryCampaign(ModuleSystem.database, $"SELECT deathCorpse, areaTag, position FROM playerDeathCorpses");

      while (Convert.ToBoolean(NWScript.SqlStep(query)))
        NWScript.SqlGetObject(query, 0, Utils.GetLocationFromDatabase(NWScript.SqlGetString(query, 1), NWScript.SqlGetVector(query, 2), 0));
    }
    public void RestoreDMPersistentPlaceableFromDatabase()
    {
      var query = NWScript.SqlPrepareQueryCampaign(ModuleSystem.database, $"SELECT serializedPlaceable, areaTag, position, facing FROM dm_persistant_placeable");

      while (Convert.ToBoolean(NWScript.SqlStep(query)))
        NWScript.SqlGetObject(query, 0, Utils.GetLocationFromDatabase(NWScript.SqlGetString(query, 1), NWScript.SqlGetVector(query, 2), NWScript.SqlGetFloat(query, 3)));
    }
    private void InitModuleChestSpawn()
    {
      var oArea = NWScript.GetFirstArea();

      while (Convert.ToBoolean(NWScript.GetIsObjectValid(oArea)))
      {
        Module.areaDictionnary.Add(NWScript.GetObjectUUID(oArea), new Area(oArea));
        oArea = NWScript.GetNextArea();
      }
    }
    public string PreparingModuleForAsyncReboot()
    {
      this.botAsyncCommandList.Add("reboot");
      return "Reboot effectif dans 30 secondes.";
    }
  }
}
