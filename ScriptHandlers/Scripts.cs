using NWN.Enums;
using NWN.NWNX;
using NWN.Systems;
using System;
using System.Collections.Generic;
using System.Linq;
using NWN.Systems.PostString;

namespace NWN.ScriptHandlers
{
  public static class Scripts
  {
    public static Dictionary<string, Func<uint, int>> Register = new Dictionary<string, Func<uint, int>>
        {
            { "_onload", HandleModuleLoad },
            { "x2_mod_def_act", HandleActivateItem },
            { "cs_chatlistener", HandleChat },
          //  { "event_mouse_clic", EventMouseClick },
            { "event_potager", EventPotager },
            { "_event_effects", EventEffects },
        }.Concat(Systems.LootSystem.Register)
     .Concat(Systems.PlayerSystem.Register)
     .Concat(Systems.SpellSystem.Register)
     .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

    private static int HandleModuleLoad(uint oidSelf)
    {
      //Systems.LootSystem.InitChestArea();

      NWNX.Events.SubscribeEvent(NWNX.Events.ON_INPUT_KEYBOARD_BEFORE, Systems.PlayerSystem.ON_PC_KEYSTROKE_SCRIPT);
      NWNX.Events.ToggleDispatchListMode(NWNX.Events.ON_INPUT_KEYBOARD_BEFORE, Systems.PlayerSystem.ON_PC_KEYSTROKE_SCRIPT, 1);

      NWNX.Events.SubscribeEvent("NWNX_ON_INPUT_KEYBOARD_AFTER", "event_mv_plc");
      NWNX.Events.ToggleDispatchListMode("NWNX_ON_INPUT_KEYBOARD_AFTER", "event_mv_plc", 1);

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

      Garden.Init();

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

    private static int HandleChat(uint oidSelf)
    {
      var sChatReceived = Chat.GetMessage();
      var oChatSender = ((uint)Chat.GetSender()).AsPlayer();

      if (!oChatSender.IsPC)
        return Entrypoints.SCRIPT_NOT_HANDLED;

      var oChatTarget = Chat.GetTarget();
      var iChannel = (ChatChannel)Chat.GetChannel();
      var sCommand = sChatReceived.Split('/', ' ')[1];
      sChatReceived.Replace("/" + sCommand + " ", "");

      if(sCommand.Length > 0)
      {
        Func<string, NWPlayer, uint, ChatChannel, int> handler;
        if (ChatHandlers.Register.TryGetValue(sCommand, out handler))
        {
          try
          {
            return handler.Invoke(sChatReceived, oChatSender, oChatTarget, iChannel);
          }
          catch (Exception e)
          {
            Utils.LogException(e);
          }
        }
      }

      return Entrypoints.SCRIPT_NOT_HANDLED;
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
