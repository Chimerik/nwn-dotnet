using System;
using System.Collections.Generic;
using System.IO;
using NWN.NWNX;
using NWN.NWNX.Enum;

namespace NWN.Systems
{
  public static class ChatSystem
  {
    public const string CHAT_SCRIPT = "on_chat";
    public static Dictionary<string, Func<uint, int>> Register = new Dictionary<string, Func<uint, int>>
    {
      { CHAT_SCRIPT, HandleChat },
    };

    public static void Init ()
    {
      Chat.RegisterChatScript(CHAT_SCRIPT);
    }

    public class Context
    {
      public string msg { get; }
      public NWObject oSender { get; }
      public NWObject oTarget { get; }
      public ChatChannel channel { get; }

      public Context(string msg, NWObject oSender, NWObject oTarget, ChatChannel channel)
      {
        this.msg = msg;
        this.oSender = oSender;
        this.oTarget = oTarget;
        this.channel = channel;
      }
    }

    private static int HandleChat (uint oidself)
    {
      pipeline.Execute(new Context(
        msg: Chat.GetMessage(),
        oSender: Chat.GetSender(),
        oTarget: Chat.GetTarget(),
        channel: Chat.GetChannel()
      ));

      return Entrypoints.SCRIPT_HANDLED;
    }

    private static Pipeline<Context> pipeline = new Pipeline<Context>(
      new Action<Context, Action>[]
      {
        ChatSystem.ProcessWriteLogMiddleware,
        CommandSystem.ProcessChatCommandMiddleware,
        ChatSystem.ProcessSpeakValueMiddleware,
        ChatSystem.ProcessMutePMMiddleware,
        ChatSystem.ProcessPMMiddleware,
        ChatSystem.ProcessDeadPlayerMiddleware,
        ChatSystem.ProcessDMListenMiddleware,
        ChatSystem.ProcessLanguageMiddleware
      }
    );
    public static void ProcessWriteLogMiddleware(ChatSystem.Context chatContext, Action next)
    {
      string filename = String.Format("{0:yyyy-MM-dd}_{1}.txt", DateTime.Now, "chatlog");
      string path = Path.Combine(Environment.GetEnvironmentVariable("HOME") + "/ChatLog", filename);

      using (System.IO.StreamWriter file =
      new System.IO.StreamWriter(path, true))
      {
        if (!chatContext.oTarget.IsValid)
          file.WriteLine(DateTime.Now.ToShortTimeString() + " - [" + chatContext.channel + " - " + NWScript.GetName(NWScript.GetArea(chatContext.oSender)) + "] " + NWScript.GetName(chatContext.oSender, true) + " : " + chatContext.msg);
        else
          file.WriteLine(DateTime.Now.ToShortTimeString() + " - [" + chatContext.channel + "] " + NWScript.GetName(chatContext.oSender) + " To : " + NWScript.GetName(chatContext.oTarget, true) + " : " + chatContext.msg);
      }

      next();
      return;
    }
    public static void ProcessSpeakValueMiddleware(ChatSystem.Context chatContext, Action next)
    {
      if (NWScript.GetLocalString(chatContext.oSender, "_IS_LISTENING_VAR") == chatContext.oSender.Name)
      {
        NWNX.Chat.SkipMessage();
        NWScript.SetLocalString(chatContext.oSender, "_LISTENING_VAR", chatContext.msg);
        NWScript.DeleteLocalString(chatContext.oSender, "_IS_LISTENING_VAR");
        return;
      }

      next();
      return;
    }
    public static void ProcessMutePMMiddleware(ChatSystem.Context chatContext, Action next)
    {
      if (chatContext.oTarget.IsValid)
      {
        if (NWNX.Object.GetInt(chatContext.oTarget, "__BLOCK_ALL_MP") > 0 || NWNX.Object.GetInt(chatContext.oTarget, "__BLOCK_" + NWScript.GetName(chatContext.oSender, true) + "_MP") > 0)
          if (!NWScript.GetIsDM(chatContext.oTarget))
          {
            NWNX.Chat.SkipMessage();
            NWScript.SendMessageToPC(chatContext.oSender, NWScript.GetName(chatContext.oTarget) + " bloque actuellement la réception des mp. Votre message n'a pas pu être transmis");
            return;
          }
      }

      next();
      return;
    }
    public static void ProcessPMMiddleware(ChatSystem.Context chatContext, Action next)
    {
      if(chatContext.oTarget.IsValid)
      {
        NWNX.Chat.SkipMessage();
        if (NWScript.GetIsObjectValid(NWScript.GetLocalObject(chatContext.oTarget, "_POSSESSING")))
          NWNX.Chat.SendMessage((int)chatContext.channel, chatContext.msg, chatContext.oSender, NWScript.GetLocalObject(chatContext.oTarget, "_POSSESSING").AsObject());
        else
          NWNX.Chat.SendMessage((int)chatContext.channel, chatContext.msg, chatContext.oSender, chatContext.oTarget);
        return;
      }
      else if (chatContext.channel == NWNX.Enum.ChatChannel.PlayerTell || chatContext.channel == NWNX.Enum.ChatChannel.DMTell)
      {
        NWNX.Chat.SkipMessage();
        NWScript.SendMessageToPC(chatContext.oSender, "La personne à laquelle vous tentez d'envoyer un message n'est plus connectée.");
        return;
      }

      next();
      return;
    }
    public static void ProcessDeadPlayerMiddleware(ChatSystem.Context chatContext, Action next)
    {
      if (NWScript.GetIsDead(chatContext.oSender))
        NWScript.SendMessageToPC(chatContext.oSender, "N'oubliez pas que vous êtes inconscient, vous ne pouvez pas parler, mais tout juste gémir et décrire votre état");

      next();
      return;
    }

    public static void ProcessDMListenMiddleware(ChatSystem.Context chatContext, Action next)
    {
      //SYSTEME DE RECOPIE DE CHAT POUR LES DMS
      if (chatContext.channel == NWNX.Enum.ChatChannel.PlayerTalk || chatContext.channel == NWNX.Enum.ChatChannel.PlayerWhisper)
      {
        var oInviSender = NWScript.GetLocalObject(NWScript.GetModule(), "_INVISIBLE_SENDER"); // TODO : créer un _INVISIBLE_SENDER dans le module
        foreach (KeyValuePair<uint, PlayerSystem.Player> PlayerListEntry in PlayerSystem.Players)
        {
          PlayerSystem.Player oDM = PlayerListEntry.Value;
          if (NWScript.GetIsDM(oDM))
          {
            if (oDM.Listened.ContainsKey(chatContext.oSender))
            {
              if ((NWScript.GetArea(chatContext.oSender) != NWScript.GetArea(oDM)) || NWScript.GetDistanceBetween(oDM, chatContext.oSender) > NWNX.Chat.GetChatHearingDistance(oDM, chatContext.channel))
              {
                NWScript.SetName(oInviSender, NWScript.GetName(chatContext.oSender));
                var oPossessed = NWScript.GetLocalObject(oDM, "_POSSESSING");
                if (!NWScript.GetIsObjectValid(oPossessed))
                    NWNX.Chat.SendMessage((int)NWNX.Enum.ChatChannel.PlayerTell, "[COPIE - " + NWScript.GetName(NWScript.GetArea(chatContext.oSender)) + "] " + chatContext.msg, oInviSender.AsObject(), oDM);
                else
                  NWNX.Chat.SendMessage((int)NWNX.Enum.ChatChannel.PlayerTell, "[COPIE - " + NWScript.GetName(NWScript.GetArea(chatContext.oSender)) + "] " + chatContext.msg, oInviSender.AsObject(), oPossessed.AsObject());
              }
            }
          }
        }
      }

      next();
      return;
    }
    public static void ProcessLanguageMiddleware(ChatSystem.Context chatContext, Action next)
    {
      // SYSTEME DE LANGUE
      int iLangueActive = NWScript.GetLocalInt(chatContext.oSender, "_LANGUE_ACTIVE");

      if (iLangueActive != 0 && (chatContext.channel == NWNX.Enum.ChatChannel.PlayerTalk || chatContext.channel == NWNX.Enum.ChatChannel.PlayerWhisper || chatContext.channel == NWNX.Enum.ChatChannel.DMTalk || chatContext.channel == NWNX.Enum.ChatChannel.DMWhisper))
      {
        string sLanguageName = NWScript.Get2DAString("feat", "FEAT", iLangueActive);
        string sName = NWScript.GetLocalString(chatContext.oSender, "__DISGUISE_NAME");
        if (sName == "") sName = NWScript.GetName(chatContext.oSender);

        foreach (KeyValuePair<uint, PlayerSystem.Player> PlayerListEntry in PlayerSystem.Players)
        {
          if ((uint)chatContext.oSender != PlayerListEntry.Key && (uint)chatContext.oSender != NWScript.GetLocalObject(PlayerListEntry.Key, "_POSSESSING"))
          {
            uint oEavesdrop;

            if (NWScript.GetIsDM(PlayerListEntry.Key) && NWScript.GetIsObjectValid(NWScript.GetLocalObject(PlayerListEntry.Key, "_POSSESSING")))
              oEavesdrop = NWScript.GetLocalObject(PlayerListEntry.Key, "_POSSESSING");
            else
              oEavesdrop = PlayerListEntry.Key;

            if ((NWScript.GetArea(chatContext.oSender) == NWScript.GetArea(oEavesdrop)) && (NWScript.GetDistanceBetween(oEavesdrop, chatContext.oSender) < NWNX.Chat.GetChatHearingDistance(oEavesdrop.AsObject(), chatContext.channel)))
            {
              if (NWScript.GetHasFeat(iLangueActive, oEavesdrop) || NWScript.GetIsDM(PlayerListEntry.Key) || NWScript.GetIsDMPossessed(oEavesdrop))
              {
                NWNX.Chat.SkipMessage();
                NWNX.Chat.SendMessage((int)chatContext.channel, "[" + sLanguageName + "] " + chatContext.msg, chatContext.oSender, oEavesdrop.AsObject());
                NWScript.SendMessageToPC(oEavesdrop, sName + " : [" + sLanguageName + "] " + Languages.GetLangueStringConvertedHRPProtection(chatContext.msg, iLangueActive));
              }
              else
              {
                NWNX.Chat.SkipMessage();
                NWNX.Chat.SendMessage((int)chatContext.channel, Languages.GetLangueStringConvertedHRPProtection(chatContext.msg, iLangueActive), chatContext.oSender, oEavesdrop.AsObject());
              }
            }
          }
        }

        NWNX.Chat.SkipMessage();
        NWNX.Chat.SendMessage((int)chatContext.channel, "[" + sLanguageName + "] " + chatContext.msg, chatContext.oSender, chatContext.oSender);
        NWScript.SendMessageToPC(chatContext.oSender, sName + " : [" + sLanguageName + "] " + Languages.GetLangueStringConvertedHRPProtection(chatContext.msg, iLangueActive));
        return;
      }

      next();
      return;
    }
  }
}
