using NWN.Systems;
using System;
using System.Collections.Generic;
using System.Linq;
using NWN.Core;
using NWN.Core.NWNX;

namespace NWN.ScriptHandlers
{
  public static class Scripts
  {
    public static Dictionary<string, Func<uint, int>> Register = new Dictionary<string, Func<uint, int>>
    {
      { "event_moduleload", HandleModuleLoad },
      { "x2_mod_def_act", HandleActivateItem },
      //  { "event_mouse_clic", EventMouseClick },
      { "event_potager", EventPotager },
      { "_event_effects", EventEffects },
    }.Concat(Systems.LootSystem.Register)
     .Concat(Systems.PlayerSystem.Register)
     .Concat(Systems.ChatSystem.Register)
     .Concat(Systems.SpellSystem.Register)
     .Concat(Systems.ItemSystem.Register)
     .Concat(PlaceableSystem.Register)
     .Concat(Systems.CollectSystem.Register)
     .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

    private static int HandleModuleLoad(uint oidSelf)
    {
      try
      {
        LootSystem.InitChestArea();
      }
      catch (Exception e)
      {
        Utils.LogException(e);
      }

      ChatSystem.Init();
      
      NWScript.SetEventScript(NWScript.GetModule(), NWScript.EVENT_SCRIPT_MODULE_ON_PLAYER_TARGET, "on_pc_target");

      EventsPlugin.SubscribeEvent("NWNX_ON_CLIENT_DISCONNECT_BEFORE", "player_exit_before");
      EventsPlugin.ToggleDispatchListMode("NWNX_ON_CLIENT_DISCONNECT_BEFORE", "player_exit_before", 1);

      EventsPlugin.SubscribeEvent("NWNX_ON_ADD_ASSOCIATE_AFTER", "summon_add_after");
      EventsPlugin.ToggleDispatchListMode("NWNX_ON_ADD_ASSOCIATE_AFTER", "summon_add_after", 1);
      EventsPlugin.SubscribeEvent("NWNX_ON_REMOVE_ASSOCIATE_AFTER", "summon_remove_after");
      EventsPlugin.ToggleDispatchListMode("NWNX_ON_REMOVE_ASSOCIATE_AFTER", "summon_remove_after", 1);

      EventsPlugin.SubscribeEvent("NWNX.ON_INPUT_KEYBOARD_BEFORE", PlayerSystem.ON_PC_KEYSTROKE_SCRIPT);
      EventsPlugin.ToggleDispatchListMode("NWNX.ON_INPUT_KEYBOARD_BEFORE", PlayerSystem.ON_PC_KEYSTROKE_SCRIPT, 1);

      EventsPlugin.SubscribeEvent("NWNX.ON_INPUT_KEYBOARD_BEFORE", PlayerSystem.ON_PC_KEYSTROKE_SCRIPT);
      EventsPlugin.ToggleDispatchListMode("NWNX.ON_INPUT_KEYBOARD_BEFORE", PlayerSystem.ON_PC_KEYSTROKE_SCRIPT, 1);

      EventsPlugin.SubscribeEvent("NWNX_ON_INPUT_KEYBOARD_AFTER", "event_mv_plc");
      EventsPlugin.ToggleDispatchListMode("NWNX_ON_INPUT_KEYBOARD_AFTER", "event_mv_plc", 1);

      EventsPlugin.SubscribeEvent("NWNX_ON_BROADCAST_CAST_SPELL_AFTER", "event_spellbroadcast_after");
      EventsPlugin.ToggleDispatchListMode("NWNX_ON_BROADCAST_CAST_SPELL_AFTER", "event_spellbroadcast_after", 1);

      EventsPlugin.SubscribeEvent("NWNX_ON_ITEM_EQUIP_BEFORE", "event_equip_items_before");
      EventsPlugin.ToggleDispatchListMode("NWNX_ON_ITEM_EQUIP_BEFORE", "event_equip_items_before", 1);
      EventsPlugin.SubscribeEvent("NWNX_ON_ITEM_UNEQUIP_BEFORE", "event_unequip_items_before");
      EventsPlugin.ToggleDispatchListMode("NWNX_ON_ITEM_UNEQUIP_BEFORE", "event_unequip_items_before", 1);
      EventsPlugin.SubscribeEvent("NWNX_ON_VALIDATE_ITEM_EQUIP_BEFORE", "event_validate_equip_items_before");
      EventsPlugin.ToggleDispatchListMode("NWNX_ON_VALIDATE_ITEM_EQUIP_BEFORE", "event_validate_equip_items_before", 1);
      EventsPlugin.SubscribeEvent("NWNX_ON_VALIDATE_USE_ITEM_BEFORE", "event_validate_equip_items_before");
      EventsPlugin.ToggleDispatchListMode("NWNX_ON_VALIDATE_USE_ITEM_BEFORE", "event_validate_equip_items_before", 1);

      EventsPlugin.SubscribeEvent("NWNX_ON_SERVER_CHARACTER_SAVE_BEFORE", "event_player_save_before");
      EventsPlugin.SubscribeEvent("NWNX_ON_CLIENT_EXPORT_CHARACTER_BEFORE", "event_player_save_before");
      EventsPlugin.SubscribeEvent("NWNX_ON_SERVER_CHARACTER_SAVE_AFTER", "event_player_save_after");
      EventsPlugin.SubscribeEvent("NWNX_ON_SERVER_CHARACTER_SAVE_AFTER", "event_player_save_after");

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

      EventsPlugin.SubscribeEvent("NWNX_ON_INPUT_ATTACK_OBJECT_BEFORE", "event_auto_spell");
      EventsPlugin.ToggleDispatchListMode("NWNX_ON_INPUT_ATTACK_OBJECT_BEFORE", "event_auto_spell", 1);
      EventsPlugin.SubscribeEvent("NWNX_ON_INPUT_FORCE_MOVE_TO_OBJECT_BEFORE", "event_auto_spell");
      EventsPlugin.ToggleDispatchListMode("NWNX_ON_INPUT_FORCE_MOVE_TO_OBJECT_BEFORE", "event_auto_spell", 1);
      EventsPlugin.SubscribeEvent("NWNX_ON_INPUT_CAST_SPELL_BEFORE", "_onspellcast");
      EventsPlugin.ToggleDispatchListMode("NWNX_ON_INPUT_CAST_SPELL_BEFORE", "_onspellcast", 1);
      EventsPlugin.SubscribeEvent("NWNX_ON_INPUT_KEYBOARD_BEFORE", "event_auto_spell");
      EventsPlugin.ToggleDispatchListMode("NWNX_ON_INPUT_KEYBOARD_BEFORE", "event_auto_spell", 1);
      EventsPlugin.SubscribeEvent("NWNX_ON_INPUT_WALK_TO_WAYPOINT_BEFORE", "event_auto_spell");
      EventsPlugin.ToggleDispatchListMode("NWNX_ON_INPUT_WALK_TO_WAYPOINT_BEFORE", "event_auto_spell", 1);

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
      
      var refinery = NWScript.GetObjectByTag("refinery", 0);

      int i = 1;
      while(NWScript.GetIsObjectValid(refinery) == 1)
      {
        EventsPlugin.AddObjectToDispatchList("NWNX_ON_INVENTORY_ADD_ITEM_BEFORE", "event_refinery_add_item_before", refinery);
        i++;
        refinery = NWScript.GetObjectByTag("refinery", i);
      }

      EventsPlugin.SubscribeEvent("NWNX_ON_EXAMINE_OBJECT_BEFORE", "event_examine_before");
      EventsPlugin.SubscribeEvent("NWNX_ON_EXAMINE_OBJECT_AFTER", "event_examine_after");

      //EventsPlugin.SubscribeEvent("CDE_POTAGER", "event_potager");

      //Garden.Init();

      CollectSystem.InitiateOres();

      NWScript.DelayCommand(600.0f, () => SaveServerVault());

      RestorePlayerCorpseFromDatabase();

      return -1;
    }

    private static void SaveServerVault()
    {
      NWScript.ExportAllCharacters();
      NWScript.DelayCommand(600.0f, () => SaveServerVault());
    }

    private static int HandleActivateItem(uint oidSelf)
    {
      var oItem = NWScript.GetItemActivated();
      var oActivator = NWScript.GetItemActivator();
      var oTarget = NWScript.GetItemActivatedTarget();
      var tag = NWScript.GetTag(oItem);

      Func<uint, uint, uint, int> handler;
      if (ActivateItemHandlers.Register.TryGetValue(tag, out handler))
      {
        try
        {
          return handler.Invoke(oItem, oActivator, oTarget);
        }
        catch (Exception e)
        {
          Utils.LogException(e);
        }
      }

      return 0;
    }

    private static int EventPotager(uint oidSelf)
    {
      Garden oGarden;
      if (Garden.Potagers.TryGetValue(NWScript.GetLocalInt(oidSelf, "id"), out oGarden))
      {
        oGarden.PlanterFruit(EventsPlugin.GetEventData("FRUIT_NAME"), EventsPlugin.GetEventData("FRUIT_TAG"));
      }

      return 0;
    }

    private static int EventEffects(uint oidSelf)
    {
      string current_event = EventsPlugin.GetCurrentEvent();
      int effectType = int.Parse(EventsPlugin.GetEventData("TYPE"));
      int effectIntParam1 = int.Parse(EventsPlugin.GetEventData("INT_PARAM_1"));

      if (current_event == "NWNX_ON_EFFECT_REMOVED_AFTER")
      {
        if (EventsPlugin.GetEventData("CUSTOM_TAG") == "lycan_curse")
        {
          PlayerSystem.Player player;
          if (PlayerSystem.Players.TryGetValue(oidSelf, out player))
          {
            player.RemoveLycanCurse();
          }
        }
        else if (effectType == NWScript.EFFECT_TYPE_ABILITY_INCREASE && effectIntParam1 == NWScript.ABILITY_STRENGTH)
        {
          if (NWScript.GetMovementRate(oidSelf) != CreaturePlugin.NWNX_CREATURE_MOVEMENT_RATE_IMMOBILE)
            if (NWScript.GetWeight(oidSelf) >= int.Parse(NWScript.Get2DAString("encumbrance", "Heavy", NWScript.GetAbilityScore(oidSelf, NWScript.ABILITY_STRENGTH))))
              CreaturePlugin.SetMovementRate(oidSelf, CreaturePlugin.NWNX_CREATURE_MOVEMENT_RATE_IMMOBILE);
        }
        else if (effectType == NWScript.EFFECT_TYPE_ABILITY_DECREASE && effectIntParam1 == NWScript.ABILITY_STRENGTH)
        {
          if (NWScript.GetMovementRate(oidSelf) == CreaturePlugin.NWNX_CREATURE_MOVEMENT_RATE_IMMOBILE)
            if (NWScript.GetWeight(oidSelf) <= int.Parse(NWScript.Get2DAString("encumbrance", "Heavy", NWScript.GetAbilityScore(oidSelf, NWScript.ABILITY_STRENGTH))))
              CreaturePlugin.SetMovementRate(oidSelf, CreaturePlugin.NWNX_CREATURE_MOVEMENT_RATE_DEFAULT);
        }
      }
      else if (current_event == "NWNX_ON_EFFECT_APPLIED_AFTER")
      {
        if(effectType == NWScript.EFFECT_TYPE_ABILITY_INCREASE && effectIntParam1 == NWScript.ABILITY_STRENGTH)
        {
          if (NWScript.GetMovementRate(oidSelf) == CreaturePlugin.NWNX_CREATURE_MOVEMENT_RATE_IMMOBILE)
            if (NWScript.GetWeight(oidSelf) <= int.Parse(NWScript.Get2DAString("encumbrance", "Heavy", NWScript.GetAbilityScore(oidSelf, NWScript.ABILITY_STRENGTH))))
              CreaturePlugin.SetMovementRate(oidSelf, CreaturePlugin.NWNX_CREATURE_MOVEMENT_RATE_DEFAULT);
        }
        else if (effectType == NWScript.EFFECT_TYPE_ABILITY_DECREASE && effectIntParam1 == NWScript.GetAbilityScore(oidSelf, NWScript.ABILITY_STRENGTH))
        {
          if (NWScript.GetMovementRate(oidSelf) != CreaturePlugin.NWNX_CREATURE_MOVEMENT_RATE_IMMOBILE)
            if (NWScript.GetWeight(oidSelf) >= int.Parse(NWScript.Get2DAString("encumbrance", "Heavy", NWScript.GetAbilityScore(oidSelf, NWScript.ABILITY_STRENGTH))))
              CreaturePlugin.SetMovementRate(oidSelf, CreaturePlugin.NWNX_CREATURE_MOVEMENT_RATE_IMMOBILE);
        }
      }

      return 0;
    }
    public static void RestorePlayerCorpseFromDatabase()
    {
      var query = NWScript.SqlPrepareQueryCampaign("AoaDatabase", $"SELECT deathCorpse, areaTag, position FROM playerDeathCorpses");

      while (Convert.ToBoolean(NWScript.SqlStep(query)))
        NWScript.SqlGetObject(query, 0, Utils.GetLocationFromDatabase(NWScript.SqlGetString(query, 1), NWScript.SqlGetVector(query, 2), 0));
    }
  }
}
