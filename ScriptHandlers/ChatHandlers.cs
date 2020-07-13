using System;
using System.Collections.Generic;
using System.IO;
using NWN.Enums;
using NWN.NWNX;
using NWN.Systems;
using NWN.Systems.PostString;

namespace NWN.ScriptHandlers
{
  static public class ChatHandlers
  {
    public static Dictionary<string, Func<string, NWPlayer, uint, ChatChannel, int>> Register = new Dictionary<string, Func<string, NWPlayer, uint, ChatChannel, int>>
    {
            { "walk", HandleWalkCommand },
            { "testmenu", HandleTestMenuCommand },
            { "testblockdotnet", HandleTestBlockCommand },
            { "frostattack", HandleFrostAttackCommand },
            { "elfe", HandleElfeCommand },
    };

    private static int HandleWalkCommand(string sChatReceived, NWPlayer oChatSender, uint oChatTarget, ChatChannel iChannel)
    {
      Chat.SkipMessage();

      if (NWNX.Object.GetInt(oChatSender, "_ALWAYS_WALK") == 0)
      {
        NWNX.Player.SetAlwaysWalk(oChatSender, true);
        NWNX.Object.SetInt(oChatSender, "_ALWAYS_WALK", 1, true);
        oChatSender.SendMessage("Vous avez activé le mode marche.");
      }
      else
      {
        NWNX.Player.SetAlwaysWalk(oChatSender, false);
        NWNX.Object.DeleteInt(oChatSender, "_ALWAYS_WALK");
        oChatSender.SendMessage("Vous avez désactivé le mode marche.");
      }

      return Entrypoints.SCRIPT_HANDLED;
    }
    private static int HandleTestMenuCommand(string sChatReceived, NWPlayer oChatSender, uint oChatTarget, ChatChannel iChannel)
    {
      Chat.SkipMessage();
      NWScript.SetLocalInt(oChatSender, "_MENU_ON", 1);
      PostString.Menu_UpdateGUI(oChatSender);
      PostString.Menu_DrawStaticGUI(oChatSender);
      return Entrypoints.SCRIPT_HANDLED;
    }
    private static int HandleTestBlockCommand(string sChatReceived, NWPlayer oChatSender, uint oChatTarget, ChatChannel iChannel)
    {
      Chat.SkipMessage();
      PlayerSystem.Player oPC;
      if (PlayerSystem.Players.TryGetValue(oChatSender, out oPC))
      {
        oPC.BlockPlayer();
      }
      return Entrypoints.SCRIPT_HANDLED;
    }

    private static int HandleFrostAttackCommand(string sChatReceived, NWPlayer oChatSender, uint oChatTarget, ChatChannel iChannel)
    {
      PlayerSystem.Player oPC;
      if (PlayerSystem.Players.TryGetValue(oChatSender, out oPC))
      {
        if (sChatReceived.StartsWith("!frostattack"))
        {
          Chat.SkipMessage();

          if (oChatSender.HasSpell(Spell.RayOfFrost))
          {
            if (NWNX.Object.GetInt(oChatSender, "_FROST_ATTACK") == 0)
            {
              NWNX.Object.SetInt(oChatSender, "_FROST_ATTACK", 1, true);
              NWNX.Events.AddObjectToDispatchList("NWNX_ON_INPUT_ATTACK_OBJECT_BEFORE", "event_auto_spell", oChatSender);
              NWNX.Events.AddObjectToDispatchList("NWNX_ON_INPUT_FORCE_MOVE_TO_OBJECT_BEFORE", "event_auto_spell", oChatSender);
              NWNX.Events.AddObjectToDispatchList("NWNX_ON_INPUT_CAST_SPELL_BEFORE", "_onspellcast", oChatSender);
              NWNX.Events.AddObjectToDispatchList("NWNX_ON_INPUT_KEYBOARD_BEFORE", "event_auto_spell", oChatSender);
              NWNX.Events.AddObjectToDispatchList("NWNX_ON_INPUT_WALK_TO_WAYPOINT_BEFORE", "event_auto_spell", oChatSender);
              oPC.SendMessage("Vous activez le mode d'attaque par rayon de froid");
            }
            else
            {
              NWNX.Object.DeleteInt(oChatSender, "_FROST_ATTACK");
              NWNX.Events.RemoveObjectFromDispatchList("NWNX_ON_INPUT_ATTACK_OBJECT_BEFORE", "event_auto_spell", oChatSender);
              NWNX.Events.RemoveObjectFromDispatchList("NWNX_ON_INPUT_FORCE_MOVE_TO_OBJECT_BEFORE", "event_auto_spell", oChatSender);
              NWNX.Events.RemoveObjectFromDispatchList("NWNX_ON_INPUT_CAST_SPELL_BEFORE", "_onspellcast", oChatSender);
              NWNX.Events.RemoveObjectFromDispatchList("NWNX_ON_INPUT_KEYBOARD_BEFORE", "event_auto_spell", oChatSender);
              NWNX.Events.RemoveObjectFromDispatchList("NWNX_ON_INPUT_WALK_TO_WAYPOINT_BEFORE", "event_auto_spell", oChatSender);
              oPC.SendMessage("Vous désactivez le mode d'attaque par rayon de froid");
            }
          }
          else
            oPC.SendMessage("Il vous faut pouvoir lancer le sort rayon de froid pour activer ce mode.");

          
        }
      }

      return Entrypoints.SCRIPT_HANDLED;
    }
    private static int HandleElfeCommand(string sChatReceived, NWPlayer oChatSender, uint oChatTarget, ChatChannel iChannel)
    {
      Chat.SkipMessage();

      NWScript.SetTextureOverride("ife_foc_move", "icon_elf", oChatSender);
      /*if(NWScript.GetHasFeat(1116, oChatSender))
      {

      }
      else
        NWScript.SendMessageToPC(oChatSender, "Vous ne connaissez pas l'elfique.");
      */
      return Entrypoints.SCRIPT_HANDLED;
    }
  } 
}
