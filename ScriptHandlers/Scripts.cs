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
      { "cs_chatlistener", HandleChat }, // TODO a supprimer
      //  { "event_mouse_clic", EventMouseClick },
      { "event_potager", EventPotager },
      { "_event_effects", EventEffects },
    }.Concat(Systems.LootSystem.Register)
     .Concat(Systems.PlayerSystem.Register)
     .Concat(Systems.ChatSystem.Register)
     .Concat(Systems.SpellSystem.Register)
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
      Systems.CommandSystem.Init();

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

    private static int HandleChat(uint oidSelf)
    {
      // TODO : A supprimer
      // Repartir les fonctionnalités du chat dans le ChatSystem
      // Et les fonctionnalités du systeme de commande dans le CommandSystem
      var oChatSender = ((uint)Chat.GetSender()).AsPlayer();

      if (!oChatSender.IsPC)
        return Entrypoints.SCRIPT_HANDLED;

      var sChatReceived = Chat.GetMessage();
      var oChatTarget = Chat.GetTarget();
      var iChannel = Chat.GetChannel();
      var sCommand = "";
      if (sChatReceived.StartsWith("/"))
        sCommand = sChatReceived.Split('/', ' ')[1]; // Si le chat reçu est une commande, on récupère la commande

      string filename = String.Format("{0:yyyy-MM-dd}_{1}.txt", DateTime.Now, "chatlog");
      string path = Path.Combine(Environment.GetEnvironmentVariable("HOME") + "/ChatLog", filename);

      using (System.IO.StreamWriter file =
      new System.IO.StreamWriter(path, true))
      {
        if (!NWScript.GetIsObjectValid(oChatTarget))
          file.WriteLine(DateTime.Now.ToShortTimeString() + " - [" + iChannel + " - " + NWScript.GetName(NWScript.GetArea(oChatSender)) + "] " + NWScript.GetName(oChatSender, true) + " : " + sChatReceived);
        else
          file.WriteLine(DateTime.Now.ToShortTimeString() + " - [" + iChannel + "] " + NWScript.GetName(oChatSender) + " To : " + NWScript.GetName(oChatTarget, true) + " : " + sChatReceived);
      }

      sChatReceived = sChatReceived.Replace("/" + sCommand + " ", "");

      if (sCommand.Length > 0)
      {
        Func<string, NWPlayer, uint, NWNX.Enum.ChatChannel, int> handler;
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
      else
      { // Me sert afin de capter une valeur donnée en chat par le PJ. Mieux vaut sans doute passer par le système de menu désormais pour entrer une valeur
        if (NWScript.GetLocalString(oChatSender, "_IS_LISTENING_VAR") == NWScript.GetName(oChatSender))
        {
          NWNX.Chat.SkipMessage();
          NWScript.SetLocalString(oChatSender, "_LISTENING_VAR", sChatReceived);
          NWScript.DeleteLocalString(oChatSender, "_IS_LISTENING_VAR");
          return Entrypoints.SCRIPT_HANDLED;
        }
        else if (NWScript.GetIsDead(oChatSender))
          NWScript.SendMessageToPC(oChatSender, "N'oubliez pas que vous êtes inconscient, vous ne pouvez pas parler, mais tout juste gémir et décrire votre état");

        // MUTE MP
        if (NWScript.GetIsObjectValid(oChatTarget))
        {
          if (NWNX.Object.GetInt(oChatTarget, "__BLOCK_ALL_MP") > 0 || NWNX.Object.GetInt(oChatTarget, "__BLOCK_" + NWScript.GetName(oChatSender, true) + "_MP") > 0)
            if (!NWScript.GetIsDM(oChatSender))
            {
              NWNX.Chat.SkipMessage();
              NWScript.SendMessageToPC(oChatSender, NWScript.GetName(oChatTarget) + " bloque actuellement la réception des mp. Votre message n'a pas pu être transmis");
              return Entrypoints.SCRIPT_HANDLED;
            }
        }

        //SYSTEME DE RECOPIE DE CHAT POUR LES DMS
        if (iChannel == NWNX.Enum.ChatChannel.PlayerTalk || iChannel == NWNX.Enum.ChatChannel.PlayerWhisper)
        {
          var oInviSender = NWScript.GetLocalObject(NWScript.GetModule(), "_INVISIBLE_SENDER");
          foreach (KeyValuePair<uint, PlayerSystem.Player> PlayerListEntry in PlayerSystem.Players)
          {
            PlayerSystem.Player oDM = PlayerListEntry.Value;
            if (NWScript.GetIsDM(oDM))
            {
              if (oDM.Listened.ContainsKey(oChatSender))
              {
                if ((NWScript.GetArea(oChatSender) != NWScript.GetArea(oDM)) || NWScript.GetDistanceBetween(oDM, oChatSender) > NWNX.Chat.GetChatHearingDistance(oDM, iChannel))
                {
                  NWScript.SetName(oInviSender, NWScript.GetName(oChatSender));
                  var oPossessed = NWScript.GetLocalObject(oDM, "_POSSESSING");
                  if (!NWScript.GetIsObjectValid(oPossessed))
                  {
                    if (iChannel == NWNX.Enum.ChatChannel.PlayerTalk)
                      NWNX.Chat.SendMessage((int)NWNX.Enum.ChatChannel.PlayerTell, "[COPIE - " + NWScript.GetName(NWScript.GetArea(oChatSender)) + "] " + sChatReceived, oInviSender.AsObject(), oDM);
                    else
                      NWNX.Chat.SendMessage((int)NWNX.Enum.ChatChannel.PlayerTell, "[COPIE - " + NWScript.GetName(NWScript.GetArea(oChatSender)) + "] " + sChatReceived, oInviSender.AsObject(), oDM);
                  }
                  else if (iChannel == NWNX.Enum.ChatChannel.PlayerTalk)
                    NWNX.Chat.SendMessage((int)NWNX.Enum.ChatChannel.PlayerTell, "[COPIE - " + NWScript.GetName(NWScript.GetArea(oChatSender)) + "] " + sChatReceived, oInviSender.AsObject(), oPossessed.AsObject());
                  else
                    NWNX.Chat.SendMessage((int)NWNX.Enum.ChatChannel.PlayerTell, "[COPIE - " + NWScript.GetName(NWScript.GetArea(oChatSender)) + "] " + sChatReceived, oInviSender.AsObject(), oPossessed.AsObject());
                }
              }
            }
          }
        }

        // SYSTEME DE LANGUE
        int iLangueActive = NWScript.GetLocalInt(oChatSender, "_LANGUE_ACTIVE");

        if (iLangueActive != 0)
        {
          if (iChannel == NWNX.Enum.ChatChannel.PlayerTalk || iChannel == NWNX.Enum.ChatChannel.PlayerWhisper || iChannel == NWNX.Enum.ChatChannel.DMTalk || iChannel == NWNX.Enum.ChatChannel.DMWhisper)
          {
            string sLanguageName = NWScript.Get2DAString("feat", "FEAT", iLangueActive);
            string sName = NWScript.GetLocalString(oChatSender, "__DISGUISE_NAME");
            if (sName == "") sName = NWScript.GetName(oChatSender);

            foreach (KeyValuePair<uint, PlayerSystem.Player> PlayerListEntry in PlayerSystem.Players)
            {
              if ((uint)oChatSender != PlayerListEntry.Key && (uint)oChatSender != NWScript.GetLocalObject(PlayerListEntry.Key, "_POSSESSING"))
              {
                uint oEavesdrop;

                if (NWScript.GetIsDM(PlayerListEntry.Key) && NWScript.GetIsObjectValid(NWScript.GetLocalObject(PlayerListEntry.Key, "_POSSESSING")))
                  oEavesdrop = NWScript.GetLocalObject(PlayerListEntry.Key, "_POSSESSING");
                else
                  oEavesdrop = PlayerListEntry.Key;

                if ((NWScript.GetArea(oChatSender) == NWScript.GetArea(oEavesdrop)) && (NWScript.GetDistanceBetween(oEavesdrop, oChatSender) < NWNX.Chat.GetChatHearingDistance(oEavesdrop.AsObject(), iChannel)))
                {
                  if (NWScript.GetHasFeat(iLangueActive, oEavesdrop) || NWScript.GetIsDM(PlayerListEntry.Key) || NWScript.GetIsDMPossessed(oEavesdrop))
                  {
                    NWNX.Chat.SkipMessage();
                    NWNX.Chat.SendMessage((int)iChannel, "[" + sLanguageName + "] " + sChatReceived, oChatSender, oEavesdrop.AsObject());
                    NWScript.SendMessageToPC(oEavesdrop, sName + " : [" + sLanguageName + "] " + Languages.GetLangueStringConvertedHRPProtection(sChatReceived, iLangueActive));
                  }
                  else
                  {
                    NWNX.Chat.SkipMessage();

                    NWNX.Chat.SendMessage((int)iChannel, Languages.GetLangueStringConvertedHRPProtection(sChatReceived, iLangueActive), oChatSender, oEavesdrop.AsObject());
                  }
                }
              }
            }

            NWNX.Chat.SkipMessage();
            NWNX.Chat.SendMessage((int)iChannel, "[" + sLanguageName + "] " + sChatReceived, oChatSender, oChatSender);
            NWScript.SendMessageToPC(oChatSender, sName + " : [" + sLanguageName + "] " + Languages.GetLangueStringConvertedHRPProtection(sChatReceived, iLangueActive));
          }
          else
          {
            NWNX.Chat.SkipMessage();
            if ((iChannel == NWNX.Enum.ChatChannel.PlayerTell || iChannel == NWNX.Enum.ChatChannel.DMTell) && !NWScript.GetIsObjectValid(oChatTarget))
              NWScript.SendMessageToPC(oChatSender, "La personne à laquelle vous tentez d'envoyer un message n'est plus connectée.");
            else
              NWNX.Chat.SendMessage((int)iChannel, sChatReceived, oChatSender, oChatTarget);
          }
        }
        else
        {
          NWNX.Chat.SkipMessage();
          if ((iChannel == NWNX.Enum.ChatChannel.PlayerTell || iChannel == NWNX.Enum.ChatChannel.DMTell) && !NWScript.GetIsObjectValid(oChatTarget))
            NWScript.SendMessageToPC(oChatSender, "La personne à laquelle vous tentez d'envoyer un message n'est plus connectée.");
          else
            NWNX.Chat.SendMessage((int)iChannel, sChatReceived, oChatSender, oChatTarget);
        }

        if (NWScript.GetIsObjectValid(oChatTarget) && NWScript.GetIsObjectValid(NWScript.GetLocalObject(oChatTarget, "_POSSESSING")))
        {
          NWNX.Chat.SkipMessage();
          NWNX.Chat.SendMessage((int)iChannel, sChatReceived, oChatSender, NWScript.GetLocalObject(oChatTarget, "_POSSESSING").AsObject());
        }
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
