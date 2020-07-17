using NWN.NWNX;
using NWN.Systems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

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
     .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

    private static int HandleModuleLoad(uint oidSelf)
    {
      try
      {
        Systems.LootSystem.InitChestArea();
      }
      catch (Exception e)
      {
        Utils.LogException(e);
      }

      Systems.ChatSystem.Init();

      NWNX.Events.SubscribeEvent("NWNX_ON_CLIENT_DISCONNECT_BEFORE", "connexion");

      NWNX.Events.SubscribeEvent("NWNX_ON_ADD_ASSOCIATE_AFTER", "summon");
      NWNX.Events.ToggleDispatchListMode("NWNX_ON_ADD_ASSOCIATE_AFTER", "summon", 1);
      NWNX.Events.SubscribeEvent("NWNX_ON_REMOVE_ASSOCIATE_AFTER", "summon");
      NWNX.Events.ToggleDispatchListMode("NWNX_ON_REMOVE_ASSOCIATE_AFTER", "summon", 1);

      NWNX.Events.SubscribeEvent(NWNX.Events.ON_INPUT_KEYBOARD_BEFORE, Systems.PlayerSystem.ON_PC_KEYSTROKE_SCRIPT);
      NWNX.Events.ToggleDispatchListMode(NWNX.Events.ON_INPUT_KEYBOARD_BEFORE, Systems.PlayerSystem.ON_PC_KEYSTROKE_SCRIPT, 1);

      Events.SubscribeEvent(Events.ON_INPUT_KEYBOARD_BEFORE, Systems.PlayerSystem.ON_PC_KEYSTROKE_SCRIPT);
      Events.ToggleDispatchListMode(Events.ON_INPUT_KEYBOARD_BEFORE, Systems.PlayerSystem.ON_PC_KEYSTROKE_SCRIPT, 1);

      NWNX.Events.SubscribeEvent("NWNX_ON_INPUT_KEYBOARD_AFTER", "event_mv_plc");
      NWNX.Events.ToggleDispatchListMode("NWNX_ON_INPUT_KEYBOARD_AFTER", "event_mv_plc", 1);

      NWNX.Events.SubscribeEvent("NWNX_ON_CAST_SPELL_BEFORE", "event_spellcast");
      NWNX.Events.ToggleDispatchListMode("NWNX_ON_CAST_SPELL_BEFORE", "event_spellcast", 1);

      NWNX.Events.SubscribeEvent("NWNX_ON_ITEM_EQUIP_BEFORE", "event_items");
      NWNX.Events.ToggleDispatchListMode("NWNX_ON_ITEM_EQUIP_BEFORE", "event_items", 1);
      NWNX.Events.SubscribeEvent("NWNX_ON_ITEM_UNEQUIP_BEFORE", "event_items");
      NWNX.Events.ToggleDispatchListMode("NWNX_ON_ITEM_UNEQUIP_BEFORE", "event_items", 1);

      NWNX.Events.SubscribeEvent("NWNX_ON_SERVER_CHARACTER_SAVE_BEFORE", "event_dm_actions");
      NWNX.Events.ToggleDispatchListMode("NWNX_ON_SERVER_CHARACTER_SAVE_BEFORE", "event_dm_actions", 1);
      NWNX.Events.SubscribeEvent("NWNX_ON_CLIENT_EXPORT_CHARACTER_BEFORE", "event_dm_actions");
      NWNX.Events.ToggleDispatchListMode("NWNX_ON_CLIENT_EXPORT_CHARACTER_BEFORE", "event_dm_actions", 1);

      NWNX.Events.SubscribeEvent("NWNX_ON_DM_POSSESS_FULL_POWER_BEFORE", "event_dm_actions");
      NWNX.Events.ToggleDispatchListMode("NWNX_ON_DM_POSSESS_FULL_POWER_BEFORE", "event_dm_actions", 1);
      NWNX.Events.SubscribeEvent("NWNX_ON_DM_POSSESS_BEFORE", "event_dm_actions");
      NWNX.Events.ToggleDispatchListMode("NWNX_ON_DM_POSSESS_BEFORE", "event_dm_actions", 1);
      NWNX.Events.SubscribeEvent("NWNX_ON_DM_SPAWN_OBJECT_AFTER", "event_dm_actions");
      NWNX.Events.ToggleDispatchListMode("NWNX_ON_DM_SPAWN_OBJECT_AFTER", "event_dm_actions", 1);

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

      NWNX.Events.SubscribeEvent("CDE_POTAGER", "event_potager");

      //Garden.Init();

      return Entrypoints.SCRIPT_NOT_HANDLED;
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
      }

      return Entrypoints.SCRIPT_HANDLED;
    }
  }
}
