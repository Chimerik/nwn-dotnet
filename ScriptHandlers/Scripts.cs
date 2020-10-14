using NWN.NWNX;
using NWN.Systems;
using System;
using System.Collections.Generic;
using System.Linq;
using NWN.Enums;

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

      NWScript.SetEventScript(NWScript.GetModule(), (int)EventScript.Module_OnPlayerTarget, "on_pc_target");

      NWNX.Events.SubscribeEvent("NWNX_ON_CLIENT_DISCONNECT_BEFORE", "player_exit_before");
      NWNX.Events.ToggleDispatchListMode("NWNX_ON_CLIENT_DISCONNECT_BEFORE", "player_exit_before", 1);

      NWNX.Events.SubscribeEvent("NWNX_ON_ADD_ASSOCIATE_AFTER", "summon_add_after");
      NWNX.Events.ToggleDispatchListMode("NWNX_ON_ADD_ASSOCIATE_AFTER", "summon_add_after", 1);
      NWNX.Events.SubscribeEvent("NWNX_ON_REMOVE_ASSOCIATE_AFTER", "summon_remove_after");
      NWNX.Events.ToggleDispatchListMode("NWNX_ON_REMOVE_ASSOCIATE_AFTER", "summon_remove_after", 1);

      NWNX.Events.SubscribeEvent(NWNX.Events.ON_INPUT_KEYBOARD_BEFORE, PlayerSystem.ON_PC_KEYSTROKE_SCRIPT);
      NWNX.Events.ToggleDispatchListMode(NWNX.Events.ON_INPUT_KEYBOARD_BEFORE, PlayerSystem.ON_PC_KEYSTROKE_SCRIPT, 1);

      Events.SubscribeEvent(Events.ON_INPUT_KEYBOARD_BEFORE, PlayerSystem.ON_PC_KEYSTROKE_SCRIPT);
      Events.ToggleDispatchListMode(Events.ON_INPUT_KEYBOARD_BEFORE, PlayerSystem.ON_PC_KEYSTROKE_SCRIPT, 1);

      NWNX.Events.SubscribeEvent("NWNX_ON_INPUT_KEYBOARD_AFTER", "event_mv_plc");
      NWNX.Events.ToggleDispatchListMode("NWNX_ON_INPUT_KEYBOARD_AFTER", "event_mv_plc", 1);

      NWNX.Events.SubscribeEvent("NWNX_ON_BROADCAST_CAST_SPELL_AFTER", "event_spellbroadcast_after");
      NWNX.Events.ToggleDispatchListMode("NWNX_ON_BROADCAST_CAST_SPELL_AFTER", "event_spellbroadcast_after", 1);

      NWNX.Events.SubscribeEvent("NWNX_ON_ITEM_EQUIP_BEFORE", "event_equip_items_before");
      NWNX.Events.ToggleDispatchListMode("NWNX_ON_ITEM_EQUIP_BEFORE", "event_equip_items_before", 1);
      NWNX.Events.SubscribeEvent("NWNX_ON_ITEM_UNEQUIP_BEFORE", "event_unequip_items_before");
      NWNX.Events.ToggleDispatchListMode("NWNX_ON_ITEM_UNEQUIP_BEFORE", "event_unequip_items_before", 1);
      NWNX.Events.SubscribeEvent("NWNX_ON_VALIDATE_ITEM_EQUIP_BEFORE", "event_validate_equip_items_before");
      NWNX.Events.ToggleDispatchListMode("NWNX_ON_VALIDATE_ITEM_EQUIP_BEFORE", "event_validate_equip_items_before", 1);
      NWNX.Events.SubscribeEvent("NWNX_ON_VALIDATE_USE_ITEM_BEFORE", "event_validate_equip_items_before");
      NWNX.Events.ToggleDispatchListMode("NWNX_ON_VALIDATE_USE_ITEM_BEFORE", "event_validate_equip_items_before", 1);

      NWNX.Events.SubscribeEvent("NWNX_ON_SERVER_CHARACTER_SAVE_BEFORE", "event_player_save_before");
      NWNX.Events.SubscribeEvent("NWNX_ON_CLIENT_EXPORT_CHARACTER_BEFORE", "event_player_save_before");

      NWNX.Events.SubscribeEvent("NWNX_ON_DM_POSSESS_FULL_POWER_BEFORE", "event_dm_possess_before");
      NWNX.Events.SubscribeEvent("NWNX_ON_DM_POSSESS_BEFORE", "event_dm_possess_before");
      NWNX.Events.SubscribeEvent("NWNX_ON_DM_SPAWN_OBJECT_AFTER", "event_dm_spawn_object_after");
      NWNX.Events.SubscribeEvent("NWNX_ON_DM_JUMP_TARGET_TO_POINT_AFTER", "event_dm_jump_target_after");

      NWNX.Events.SubscribeEvent("NWNX_ON_START_COMBAT_ROUND_AFTER", "event_start_combat_after");
      NWNX.Events.ToggleDispatchListMode("NWNX_ON_START_COMBAT_ROUND_AFTER", "event_start_combat_after", 1);
      NWNX.Events.SubscribeEvent("NWNX_ON_COMBAT_MODE_OFF", "event_combatmode");
      NWNX.Events.ToggleDispatchListMode("NWNX_ON_COMBAT_MODE_OFF", "event_combatmode", 1);
      NWNX.Events.SubscribeEvent("NWNX_ON_USE_SKILL_BEFORE", "event_skillused");
      NWNX.Events.ToggleDispatchListMode("NWNX_ON_USE_SKILL_BEFORE", "event_skillused", 1);

      NWNX.Events.SubscribeEvent("NWNX_ON_DO_LISTEN_DETECTION_AFTER", "event_detection_after");
      NWNX.Events.ToggleDispatchListMode("NWNX_ON_DO_LISTEN_DETECTION_AFTER", "event_detection_after", 1);

      NWNX.Events.SubscribeEvent("NWNX_ON_INPUT_ATTACK_OBJECT_BEFORE", "event_auto_spell");
      NWNX.Events.ToggleDispatchListMode("NWNX_ON_INPUT_ATTACK_OBJECT_BEFORE", "event_auto_spell", 1);
      NWNX.Events.SubscribeEvent("NWNX_ON_INPUT_FORCE_MOVE_TO_OBJECT_BEFORE", "event_auto_spell");
      NWNX.Events.ToggleDispatchListMode("NWNX_ON_INPUT_FORCE_MOVE_TO_OBJECT_BEFORE", "event_auto_spell", 1);
      NWNX.Events.SubscribeEvent("NWNX_ON_INPUT_CAST_SPELL_BEFORE", "_onspellcast");
      NWNX.Events.ToggleDispatchListMode("NWNX_ON_INPUT_CAST_SPELL_BEFORE", "_onspellcast", 1);
      NWNX.Events.SubscribeEvent("NWNX_ON_INPUT_KEYBOARD_BEFORE", "event_auto_spell");
      NWNX.Events.ToggleDispatchListMode("NWNX_ON_INPUT_KEYBOARD_BEFORE", "event_auto_spell", 1);
      NWNX.Events.SubscribeEvent("NWNX_ON_INPUT_WALK_TO_WAYPOINT_BEFORE", "event_auto_spell");
      NWNX.Events.ToggleDispatchListMode("NWNX_ON_INPUT_WALK_TO_WAYPOINT_BEFORE", "event_auto_spell", 1);

      NWNX.Events.SubscribeEvent("NWNX_ON_USE_FEAT_BEFORE", "event_feat_used");
      NWNX.Events.ToggleDispatchListMode("NWNX_ON_USE_FEAT_BEFORE", "event_feat_used", 1);

      NWNX.Events.SubscribeEvent("NWNX_ON_EFFECT_REMOVED_AFTER", "event_effects");
      NWNX.Events.ToggleDispatchListMode("NWNX_ON_EFFECT_REMOVED_AFTER", "event_effects", 1);

      NWNX.Events.SubscribeEvent("NWNX_ON_PARTY_ACCEPT_INVITATION_AFTER", "event_party_accept_after");
      NWNX.Events.SubscribeEvent("NWNX_ON_PARTY_LEAVE_BEFORE", "event_party_leave_before");
      NWNX.Events.SubscribeEvent("NWNX_ON_PARTY_LEAVE_AFTER", "event_party_leave_after");
      NWNX.Events.SubscribeEvent("NWNX_ON_PARTY_KICK_BEFORE", "event_party_leave_before");
      NWNX.Events.SubscribeEvent("NWNX_ON_PARTY_KICK_AFTER", "event_party_kick_after");

      NWNX.Events.SubscribeEvent("NWNX_ON_TIMING_BAR_CANCEL_BEFORE", "event_mining_cycle_cancel_before");
      NWNX.Events.ToggleDispatchListMode("NWNX_ON_TIMING_BAR_CANCEL_BEFORE", "event_mining_cycle_cancel_before", 1);
      NWNX.Events.SubscribeEvent("NWNX_ON_CLIENT_DISCONNECT_BEFORE", "event_mining_cycle_cancel_before");
      NWNX.Events.ToggleDispatchListMode("NWNX_ON_CLIENT_DISCONNECT_BEFORE", "event_mining_cycle_cancel_before", 1);
      NWNX.Events.SubscribeEvent("NWNX_ON_ITEM_EQUIP_BEFORE", "event_mining_cycle_cancel_before");
      NWNX.Events.ToggleDispatchListMode("NWNX_ON_ITEM_EQUIP_BEFORE", "event_mining_cycle_cancel_before", 1);
      NWNX.Events.SubscribeEvent("NWNX_ON_ITEM_UNEQUIP_BEFORE", "event_mining_cycle_cancel_before");
      NWNX.Events.ToggleDispatchListMode("NWNX_ON_ITEM_UNEQUIP_BEFORE", "event_mining_cycle_cancel_before", 1);
      NWNX.Events.SubscribeEvent("NWNX_ON_START_COMBAT_ROUND_AFTER", "event_mining_cycle_cancel_before");
      NWNX.Events.ToggleDispatchListMode("NWNX_ON_START_COMBAT_ROUND_AFTER", "event_mining_cycle_cancel_before", 1);
      NWNX.Events.SubscribeEvent("NWNX_ON_INPUT_CAST_SPELL_BEFORE", "event_mining_cycle_cancel_before");
      NWNX.Events.ToggleDispatchListMode("NWNX_ON_INPUT_CAST_SPELL_BEFORE", "event_mining_cycle_cancel_before", 1);

      NWNX.Events.SubscribeEvent("NWNX_ON_INVENTORY_ADD_ITEM_BEFORE", "event_refinery_add_item_before");
      NWNX.Events.ToggleDispatchListMode("NWNX_ON_INVENTORY_ADD_ITEM_BEFORE", "event_refinery_add_item_before", 1);

      NWNX.Events.SubscribeEvent("NWNX_ON_INVENTORY_ADD_ITEM_AFTER", "event_pccorpse_add_item_after");
      NWNX.Events.ToggleDispatchListMode("NWNX_ON_INVENTORY_ADD_ITEM_AFTER", "event_pccorpse_add_item_after", 1);
      NWNX.Events.SubscribeEvent("NWNX_ON_INVENTORY_REMOVE_ITEM_AFTER", "event_pccorpse_remove_item_after");
      NWNX.Events.ToggleDispatchListMode("NWNX_ON_INVENTORY_REMOVE_ITEM_AFTER", "event_pccorpse_remove_item_after", 1);

      NWPlaceable refinery = NWScript.GetObjectByTag("refinery", 0).AsPlaceable();

      int i = 1;
      while(refinery.IsValid)
      {
        NWNX.Events.AddObjectToDispatchList("NWNX_ON_INVENTORY_ADD_ITEM_BEFORE", "event_refinery_add_item_before", refinery);
        i++;
        refinery = NWScript.GetObjectByTag("refinery", i).AsPlaceable();
      }

      NWNX.Events.SubscribeEvent("NWNX_ON_EXAMINE_OBJECT_BEFORE", "event_examine_before");
      NWNX.Events.SubscribeEvent("NWNX_ON_EXAMINE_OBJECT_AFTER", "event_examine_after");

      //NWNX.Events.SubscribeEvent("CDE_POTAGER", "event_potager");

      //Garden.Init();

      CollectSystem.InitiateOres();

      NWScript.DelayCommand(600.0f, () => SaveServerVault());

      // TODO : Restore Death corpses from DB

      return Entrypoints.SCRIPT_NOT_HANDLED;
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
      var tag = NWScript.GetTag(oItem);

      Func<uint, uint, int> handler;
      if (ActivateItemHandlers.Register.TryGetValue(tag, out handler))
      {
        try
        {
          return handler.Invoke(oItem, oActivator);
        }
        catch (Exception e)
        {
          Utils.LogException(e);
        }
      }

      return Entrypoints.SCRIPT_NOT_HANDLED;
    }

    private static int EventPotager(uint oidSelf)
    {
      Garden oGarden;
      if (Garden.Potagers.TryGetValue(oidSelf.AsPlaceable().Locals.Int.Get("id"), out oGarden))
      {
        oGarden.PlanterFruit(NWNX.Events.GetEventData("FRUIT_NAME"), NWNX.Events.GetEventData("FRUIT_TAG"));
      }

      return Entrypoints.SCRIPT_HANDLED;
    }

    private static int EventEffects(uint oidSelf)
    {
      string current_event = NWNX.Events.GetCurrentEvent();
      int effectType = int.Parse(NWNX.Events.GetEventData("TYPE"));
      int effectIntParam1 = int.Parse(NWNX.Events.GetEventData("INT_PARAM_1"));

      if (current_event == "NWNX_ON_EFFECT_REMOVED_AFTER")
      {
        if (NWNX.Events.GetEventData("CUSTOM_TAG") == "lycan_curse")
        {
          PlayerSystem.Player player;
          if (PlayerSystem.Players.TryGetValue(oidSelf, out player))
          {
            player.RemoveLycanCurse();
          }
        }
        else if (effectType == (int)EffectTypeEngine.AbilityIncrease && effectIntParam1 == (int)Ability.Strength)
        {
          if (NWScript.GetMovementRate(oidSelf) != (int)MovementRate.Immobile)
            if (NWScript.GetWeight(oidSelf) >= int.Parse(NWScript.Get2DAString("encumbrance", "Heavy", oidSelf.AsCreature().Ability[Ability.Strength].Total)))
              NWNX.Creature.SetMovementRate(oidSelf, MovementRate.Immobile);
        }
        else if (effectType == (int)EffectTypeEngine.AbilityDecrease && effectIntParam1 == (int)Ability.Strength)
        {
          if (NWScript.GetMovementRate(oidSelf) == (int)MovementRate.Immobile)
            if (NWScript.GetWeight(oidSelf) <= int.Parse(NWScript.Get2DAString("encumbrance", "Heavy", oidSelf.AsCreature().Ability[Ability.Strength].Total)))
              NWNX.Creature.SetMovementRate(oidSelf, MovementRate.Default);
        }
      }
      else if (current_event == "NWNX_ON_EFFECT_APPLIED_AFTER")
      {
        if(effectType == (int)EffectTypeEngine.AbilityIncrease && effectIntParam1 == (int)Ability.Strength)
        {
          if (NWScript.GetMovementRate(oidSelf) == (int)MovementRate.Immobile)
            if (NWScript.GetWeight(oidSelf) <= int.Parse(NWScript.Get2DAString("encumbrance", "Heavy", oidSelf.AsCreature().Ability[Ability.Strength].Total)))
              NWNX.Creature.SetMovementRate(oidSelf, MovementRate.Default);
        }
        else if (effectType == (int)EffectTypeEngine.AbilityDecrease && effectIntParam1 == (int)Ability.Strength)
        {
          if (NWScript.GetMovementRate(oidSelf) != (int)MovementRate.Immobile)
            if (NWScript.GetWeight(oidSelf) >= int.Parse(NWScript.Get2DAString("encumbrance", "Heavy", oidSelf.AsCreature().Ability[Ability.Strength].Total)))
              NWNX.Creature.SetMovementRate(oidSelf, MovementRate.Immobile);
        }
      }

      return Entrypoints.SCRIPT_HANDLED;
    }
  }
}
