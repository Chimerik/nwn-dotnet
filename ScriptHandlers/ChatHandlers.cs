using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NWN.Enums;
using NWN.NWNX;
using NWN.NWNX.Enum;
using NWN.Systems;

namespace NWN.ScriptHandlers
{
  static public class ChatHandlers
  {
    public static Dictionary<string, Func<string, NWPlayer, uint, NWNX.Enum.ChatChannel, int>> Register = new Dictionary<string, Func<string, NWPlayer, uint, NWNX.Enum.ChatChannel, int>>
    {
            { "walk", HandleWalkCommand },
            { "testblockdotnet", HandleTestBlockCommand },
            { "frostattack", HandleFrostAttackCommand },
            { "reveal", HandleRevealCommand },
            { "dispel_aoe", HandleDispelAoeCommand },
            { "dispel", HandleDispelCommand },
            { "dissip", HandleDispelCommand },
            { "invi", HandleInviCommand },
            { "hostile", HandleHostileCommand },
            { "description", HandleDescriptionCommand },
            { "publickey", HandlePublicKeyCommand },
            { "mute", HandleMuteCommand },
            { "casque", HandleCasqueCommand },
            { "cape", HandleCapeCommand },
            { "touch", HandleTouchCommand },
            { "reboot", HandleRebootCommand },
            { "listen", HandleListenCommand },
            { "persist", HandlePersistCommand },
            { "elfe", HandleElfeCommand },
            { "test", HandleTestCommand },
    };

    private static int HandleWalkCommand(string sChatReceived, NWPlayer oChatSender, uint oChatTarget, NWNX.Enum.ChatChannel iChannel)
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
    private static int HandleTestBlockCommand(string sChatReceived, NWPlayer oChatSender, uint oChatTarget, NWNX.Enum.ChatChannel iChannel)
    {
      Chat.SkipMessage();
      PlayerSystem.Player oPC;
      if (PlayerSystem.Players.TryGetValue(oChatSender, out oPC))
      {
        oPC.BoulderBlock();
      }
      return Entrypoints.SCRIPT_HANDLED;
    }

    private static int HandleFrostAttackCommand(string sChatReceived, NWPlayer oChatSender, uint oChatTarget, NWNX.Enum.ChatChannel iChannel)
    {
      PlayerSystem.Player oPC;
      if (PlayerSystem.Players.TryGetValue(oChatSender, out oPC))
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

      return Entrypoints.SCRIPT_HANDLED;
    }
    private static int HandleElfeCommand(string sChatReceived, NWPlayer oChatSender, uint oChatTarget, NWNX.Enum.ChatChannel iChannel)
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
    private static int HandleTestCommand(string sChatReceived, NWPlayer oChatSender, uint oChatTarget, NWNX.Enum.ChatChannel iChannel)
    {
      Chat.SkipMessage();
      string[] sArray = sChatReceived.Split('*', '*');
      string sTranslated = "";
      int i = 0;

      foreach(string s in sArray)
      {
        if (i % 2 == 0)
          sTranslated += "(elf) " + s;
        else
          sTranslated += $"* {s} *";

        i++;
      }
      NWScript.SendMessageToPC(oChatSender, "Translation : " + sTranslated);

      return Entrypoints.SCRIPT_HANDLED;
    }
    private static int HandleRevealCommand(string sChatReceived, NWPlayer oChatSender, uint oChatTarget, NWNX.Enum.ChatChannel iChannel)
    {
      Chat.SkipMessage();
      if (!NWScript.GetIsObjectValid(oChatTarget))
        NWNX.Reveal.RevealToParty(oChatSender, 1, DetectionMethod.Seen);
      else
        NWNX.Reveal.RevealTo(oChatSender, oChatTarget.AsObject(), DetectionMethod.Seen);

      return Entrypoints.SCRIPT_HANDLED;
    }
    private static int HandleDispelAoeCommand(string sChatReceived, NWPlayer oChatSender, uint oChatTarget, NWNX.Enum.ChatChannel iChannel)
    {
      Chat.SkipMessage();
      var oArea = NWScript.GetArea(oChatSender);
      var oAoE = NWScript.GetFirstObjectInArea(oArea);

      while (NWScript.GetIsObjectValid(oAoE))
      {
        if (NWScript.GetAreaOfEffectCreator(oAoE) == oChatSender)
        {
          NWScript.DestroyObject(oAoE);
        }
        oAoE = NWScript.GetNextObjectInArea(oArea);
      }

      return Entrypoints.SCRIPT_HANDLED;
    }
    private static int HandleDispelCommand(string sChatReceived, NWPlayer oChatSender, uint oChatTarget, NWNX.Enum.ChatChannel iChannel)
    {
      Chat.SkipMessage();
      if (NWScript.GetIsObjectValid(oChatTarget))
      {
        foreach(Effect e in oChatTarget.AsObject().Effects)
        {
          if (NWScript.GetEffectCreator(e) == oChatSender && NWScript.GetEffectTag(e) == "")
            NWScript.RemoveEffect(oChatTarget, e);
        }
      }
      else
      {
        foreach (Effect e in oChatSender.Effects)
        {
          if (NWScript.GetEffectCreator(e) == oChatSender && NWScript.GetEffectTag(e) == "")
            NWScript.RemoveEffect(oChatSender, e);
        }
      }

      return Entrypoints.SCRIPT_HANDLED;
    }
    private static int HandleInviCommand(string sChatReceived, NWPlayer oChatSender, uint oChatTarget, NWNX.Enum.ChatChannel iChannel)
    {
      Chat.SkipMessage();

      NWScript.SendMessageToPC(oChatSender, "Vous dissipez tout sort d'invisibilité sur vous.");
      Spells.RemoveAnySpellEffects(Spell.ImprovedInvisibilit, oChatSender);
      Spells.RemoveAnySpellEffects(Spell.Invisibilit, oChatSender);
      Spells.RemoveAnySpellEffects(Spell.InvisibilitySpher, oChatSender);

      return Entrypoints.SCRIPT_HANDLED;
    }
    private static int HandleHostileCommand(string sChatReceived, NWPlayer oChatSender, uint oChatTarget, NWNX.Enum.ChatChannel iChannel)
    {
      Chat.SkipMessage();

      int iCount = 1;
      var oPC = NWScript.GetNearestCreature(1, 1, oChatSender, iCount);
      
      float fDistance;
      if (float.TryParse(sChatReceived, out fDistance))
      {

      }
      else
        fDistance = 20.0f;

      while (NWScript.GetIsObjectValid(oPC))
      {
        if (NWScript.GetDistanceBetween(oPC, oChatSender) > fDistance)
          break;

        if (!Utils.IsPartyMember(oChatSender, oPC))
               NWScript.SetPCDislike(oChatSender, oPC);

             iCount++;
             oPC = NWScript.GetNearestCreature(1, 1, oChatSender, iCount);
           }
      return Entrypoints.SCRIPT_HANDLED;
    }
    private static int HandleDescriptionCommand(string sChatReceived, NWPlayer oChatSender, uint oChatTarget, NWNX.Enum.ChatChannel iChannel)
    {
      Chat.SkipMessage();
      
      if (sChatReceived.Contains("+np"))
      {
        NWScript.SetDescription(oChatSender, NWScript.GetDescription(oChatSender) + "\n" + sChatReceived.Replace("+np", "").Trim());
        return Entrypoints.SCRIPT_HANDLED;
      }

      if (sChatReceived.Contains("+ap"))
      {
        NWScript.SetDescription(oChatSender, NWScript.GetDescription(oChatSender) + sChatReceived.Replace("+ap", "").Trim());
        return Entrypoints.SCRIPT_HANDLED;
      }

      NWScript.SetDescription(oChatSender, sChatReceived.Trim());

      return Entrypoints.SCRIPT_HANDLED;
    }
    private static int HandlePublicKeyCommand(string sChatReceived, NWPlayer oChatSender, uint oChatTarget, NWNX.Enum.ChatChannel iChannel)
    {
      Chat.SkipMessage();
      NWScript.SendMessageToPC(oChatSender, "Votre clef publique est : " + NWScript.GetPCPublicCDKey(oChatSender));
      return Entrypoints.SCRIPT_HANDLED;
    }
    private static int HandleMuteCommand(string sChatReceived, NWPlayer oChatSender, uint oChatTarget, NWNX.Enum.ChatChannel iChannel)
    {
      Chat.SkipMessage();

      if(NWScript.GetIsObjectValid(oChatTarget))
      {
        string sTargetName = NWScript.GetName(oChatTarget);
        if (NWNX.Object.GetInt(oChatSender, "__BLOCK_" + sTargetName + "_MP") == 0)
        {
          NWNX.Object.SetInt(oChatSender, "__BLOCK_" + sTargetName + "_MP", 1, true);
          NWScript.SendMessageToPC(oChatSender, "Vous bloquez désormais tous les mps de " + sTargetName + ". Cette commande ne fonctionne pas sur les Dms.");
        }
        else
        {
          NWNX.Object.DeleteInt(oChatSender, "__BLOCK_" + sTargetName + "_MP");
          NWScript.SendMessageToPC(oChatSender, "Vous ne bloquez plus les mps de " + sTargetName);
        }
      }
      else
      {
        if (NWNX.Object.GetInt(oChatSender, "__BLOCK_ALL_MP") == 0)
        {
          NWNX.Object.SetInt(oChatSender, "__BLOCK_ALL_MP", 1, true);
          NWScript.SendMessageToPC(oChatSender, "Vous bloquez désormais tous les mps. Vous recevrez toujours les mps des DMs.");
        }
        else
        {
          NWNX.Object.DeleteInt(oChatSender, "__BLOCK_ALL_MP");
          NWScript.SendMessageToPC(oChatSender, "Vous ne bloquez plus les mps.");
        }
      }

      return Entrypoints.SCRIPT_HANDLED;
    }
    private static int HandleCasqueCommand(string sChatReceived, NWPlayer oChatSender, uint oChatTarget, NWNX.Enum.ChatChannel iChannel)
    {
      Chat.SkipMessage();

      var oHelmet = NWScript.GetItemInSlot(Enums.InventorySlot.Head, oChatSender);

      if (NWScript.GetIsObjectValid(oHelmet))
      {
        if (NWScript.GetHiddenWhenEquipped(oHelmet) == 0)
          NWScript.SetHiddenWhenEquipped(oHelmet, 1);
        else
          NWScript.SetHiddenWhenEquipped(oHelmet, 0);
      }
      else  
        NWScript.FloatingTextStringOnCreature("Vous ne portez pas de casque !", oChatSender, false);

      return Entrypoints.SCRIPT_HANDLED;
    }
    private static int HandleCapeCommand(string sChatReceived, NWPlayer oChatSender, uint oChatTarget, NWNX.Enum.ChatChannel iChannel)
    {
      Chat.SkipMessage();

      var oCloak = NWScript.GetItemInSlot(Enums.InventorySlot.Cloak, oChatSender);

      if (NWScript.GetIsObjectValid(oCloak))
      {
        if (NWScript.GetHiddenWhenEquipped(oCloak) == 0)
          NWScript.SetHiddenWhenEquipped(oCloak, 1);
        else
          NWScript.SetHiddenWhenEquipped(oCloak, 0);
      }
      else
        NWScript.FloatingTextStringOnCreature("Vous ne portez pas de cape !", oChatSender, false);

      return Entrypoints.SCRIPT_HANDLED;
    }
    private static int HandleTouchCommand(string sChatReceived, NWPlayer oChatSender, uint oChatTarget, NWNX.Enum.ChatChannel iChannel)
    {
      Chat.SkipMessage();

      if(!Spells.GetHasEffect(NWScript.GetEffectType(NWScript.EffectCutsceneGhost()), oChatSender))
        NWScript.ApplyEffectToObject(DurationType.Permanent, NWScript.SupernaturalEffect(NWScript.EffectCutsceneGhost()), oChatSender);
      else
        Spells.RemoveEffectOfType(NWScript.GetEffectType(NWScript.EffectCutsceneGhost()), oChatSender);

      return Entrypoints.SCRIPT_HANDLED;
    }
    private static int HandleRebootCommand(string sChatReceived, NWPlayer oChatSender, uint oChatTarget, NWNX.Enum.ChatChannel iChannel)
    {
      Chat.SkipMessage();

      if (NWScript.GetIsDM(oChatSender))
      {
        NWScript.ExportAllCharacters();

        foreach (KeyValuePair<uint, PlayerSystem.Player> PlayerListEntry in PlayerSystem.Players)
        {
          NWScript.FloatingTextStringOnCreature("Attention - Le serveur va redémarrer dans 30 secondes.", PlayerListEntry.Key, false);
          Utils.RebootTimer(PlayerListEntry.Key, 30);
        }

        NWNX.Administrator.SetPlayerPassword("REBOOT");
        NWScript.AssignCommand(NWScript.GetModule(), () => NWScript.DelayCommand(30.0f, () => Utils.BootAllPC()));
        NWScript.AssignCommand(NWScript.GetModule(), () => NWScript.DelayCommand(35.0f, () => NWNX.Administrator.ShutdownServer()));
      }

      return Entrypoints.SCRIPT_HANDLED;
    }
    private static int HandleListenCommand(string sChatReceived, NWPlayer oChatSender, uint oChatTarget, NWNX.Enum.ChatChannel iChannel)
    {
      Chat.SkipMessage();

      if (NWScript.GetIsDM(oChatSender))
      {
        PlayerSystem.Player oDM;
        if (PlayerSystem.Players.TryGetValue(oChatSender, out oDM))
        {
          if (!NWScript.GetIsObjectValid(oChatTarget))
          {
            if (oDM.Listened.Count > 0)
              oDM.Listened.Clear();
            else
            {
              foreach (KeyValuePair<uint, PlayerSystem.Player> PlayerListEntry in PlayerSystem.Players)
              {
                if (!NWScript.GetIsDM(PlayerListEntry.Key))
                  oDM.Listened.Add(PlayerListEntry.Key, PlayerListEntry.Value);
              }
            }
          }
          else
          {
            if (oDM.Listened.ContainsKey(oChatTarget))
              oDM.Listened.Remove(oChatTarget);
            else
            {
              PlayerSystem.Player oPC;
              if (PlayerSystem.Players.TryGetValue(oChatTarget, out oPC))
                oDM.Listened.Add(oChatTarget, oPC);
            }
          }
        }  
      }

      return Entrypoints.SCRIPT_HANDLED;
    }
    private static int HandlePersistCommand(string sChatReceived, NWPlayer oChatSender, uint oChatTarget, NWNX.Enum.ChatChannel iChannel)
    {
      Chat.SkipMessage();

      if (NWScript.GetIsDM(oChatSender))
      {
        if (NWNX.Object.GetInt(oChatSender, "_SPAWN_PERSIST") != 0)
        {
          NWNX.Object.DeleteInt(oChatSender, "_SPAWN_PERSIST");
          NWScript.SendMessageToPC(oChatSender, "Persistance des placeables créés par DM désactivée.");
        }
        else
        {
          NWNX.Object.SetInt(oChatSender, "_SPAWN_PERSIST", 1, true);
          NWScript.SendMessageToPC(oChatSender, "Persistance des placeables créés par DM activée.");
        }
      }

      return Entrypoints.SCRIPT_HANDLED;
    }
  } 
}
