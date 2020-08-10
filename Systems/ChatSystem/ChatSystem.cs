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
        ChatSystem.ProcessAFKDetectionMiddleware,
        ChatSystem.ProcessDeadPlayerMiddleware,
        ChatSystem.ProcessDMListenMiddleware,
        ChatSystem.ProcessLanguageMiddleware
      }
    );
    public static void ProcessWriteLogMiddleware(ChatSystem.Context ctx, Action next)
    {
      string filename = String.Format("{0:yyyy-MM-dd}_{1}.txt", DateTime.Now, "chatlog");
      string path = Path.Combine(Environment.GetEnvironmentVariable("HOME") + "/ChatLog", filename);

      using (System.IO.StreamWriter file =
      new System.IO.StreamWriter(path, true))
      {
        if (!ctx.oTarget.IsValid)
          file.WriteLine(DateTime.Now.ToShortTimeString() + " - [" + ctx.channel + " - " + NWScript.GetName(NWScript.GetArea(ctx.oSender)) + "] " + NWScript.GetName(ctx.oSender, true) + " : " + ctx.msg);
        else
          file.WriteLine(DateTime.Now.ToShortTimeString() + " - [" + ctx.channel + "] " + NWScript.GetName(ctx.oSender) + " To : " + NWScript.GetName(ctx.oTarget, true) + " : " + ctx.msg);
      }

      next();
    }
    public static void ProcessSpeakValueMiddleware(ChatSystem.Context ctx, Action next)
    {
      if (NWScript.GetLocalString(ctx.oSender, "_IS_LISTENING_VAR") == ctx.oSender.Name)
      {
        NWNX.Chat.SkipMessage();
        NWScript.SetLocalString(ctx.oSender, "_LISTENING_VAR", ctx.msg);
        NWScript.DeleteLocalString(ctx.oSender, "_IS_LISTENING_VAR");
        return;
      }

      next();
    }
    public static void ProcessMutePMMiddleware(ChatSystem.Context ctx, Action next)
    {
      if (ctx.oTarget.IsValid)
      {
        if (NWNX.Object.GetInt(ctx.oTarget, "__BLOCK_ALL_MP") > 0 || NWNX.Object.GetInt(ctx.oTarget, "__BLOCK_" + NWScript.GetName(ctx.oSender, true) + "_MP") > 0)
          if (!NWScript.GetIsDM(ctx.oTarget))
          {
            NWNX.Chat.SkipMessage();
            NWScript.SendMessageToPC(ctx.oSender, NWScript.GetName(ctx.oTarget) + " bloque actuellement la réception des mp. Votre message n'a pas pu être transmis");
            return;
          }
      }

      next();
    }
    public static void ProcessPMMiddleware(ChatSystem.Context ctx, Action next)
    {
      if(ctx.oTarget.IsValid)
      {
        NWNX.Chat.SkipMessage();
        if (NWScript.GetIsObjectValid(NWScript.GetLocalObject(ctx.oTarget, "_POSSESSING")))
          NWNX.Chat.SendMessage((int)ctx.channel, ctx.msg, ctx.oSender, NWScript.GetLocalObject(ctx.oTarget, "_POSSESSING").AsObject());
        else
          NWNX.Chat.SendMessage((int)ctx.channel, ctx.msg, ctx.oSender, ctx.oTarget);
        return;
      }
      else if (ctx.channel == NWNX.Enum.ChatChannel.PlayerTell || ctx.channel == NWNX.Enum.ChatChannel.DMTell)
      {
        NWNX.Chat.SkipMessage();
        NWScript.SendMessageToPC(ctx.oSender, "La personne à laquelle vous tentez d'envoyer un message n'est plus connectée.");
        return;
      }

      next();
    }
    public static void ProcessAFKDetectionMiddleware(ChatSystem.Context ctx, Action next)
    {
      PlayerSystem.Player player;
      if (PlayerSystem.Players.TryGetValue(ctx.oSender, out player))
      {
        if(player.isAFK)
          if (ctx.channel == NWNX.Enum.ChatChannel.PlayerTalk || ctx.channel == NWNX.Enum.ChatChannel.PlayerWhisper)
            if (!ctx.msg.Contains("(") && !ctx.msg.Contains(")"))
              if (NWScript.GetDistanceBetween(ctx.oSender, NWScript.GetNearestCreature(1, 1, ctx.oSender)) < 35.0f)
                player.isAFK = false;
      }

      next();
    }
    public static void ProcessDeadPlayerMiddleware(ChatSystem.Context ctx, Action next)
    {
      if (NWScript.GetIsDead(ctx.oSender))
        NWScript.SendMessageToPC(ctx.oSender, "N'oubliez pas que vous êtes inconscient, vous ne pouvez pas parler, mais tout juste gémir et décrire votre état");

      next();
    }

    public static void ProcessDMListenMiddleware(ChatSystem.Context ctx, Action next)
    {
      //SYSTEME DE RECOPIE DE CHAT POUR LES DMS
      if (ctx.channel == NWNX.Enum.ChatChannel.PlayerTalk || ctx.channel == NWNX.Enum.ChatChannel.PlayerWhisper)
      {
        var oInviSender = NWScript.GetLocalObject(NWScript.GetModule(), "_INVISIBLE_SENDER"); // TODO : créer un _INVISIBLE_SENDER dans le module
        foreach (KeyValuePair<uint, PlayerSystem.Player> PlayerListEntry in PlayerSystem.Players)
        {
          PlayerSystem.Player oDM = PlayerListEntry.Value;
          if (NWScript.GetIsDM(oDM))
          {
            if (oDM.listened.ContainsKey(ctx.oSender))
            {
              if ((NWScript.GetArea(ctx.oSender) != NWScript.GetArea(oDM)) || NWScript.GetDistanceBetween(oDM, ctx.oSender) > NWNX.Chat.GetChatHearingDistance(oDM, ctx.channel))
              {
                NWScript.SetName(oInviSender, NWScript.GetName(ctx.oSender));
                var oPossessed = NWScript.GetLocalObject(oDM, "_POSSESSING");
                if (!NWScript.GetIsObjectValid(oPossessed))
                    NWNX.Chat.SendMessage((int)NWNX.Enum.ChatChannel.PlayerTell, "[COPIE - " + NWScript.GetName(NWScript.GetArea(ctx.oSender)) + "] " + ctx.msg, oInviSender.AsObject(), oDM);
                else
                  NWNX.Chat.SendMessage((int)NWNX.Enum.ChatChannel.PlayerTell, "[COPIE - " + NWScript.GetName(NWScript.GetArea(ctx.oSender)) + "] " + ctx.msg, oInviSender.AsObject(), oPossessed.AsObject());
              }
            }
          }
        }
      }

      next();
    }
    public static void ProcessLanguageMiddleware(ChatSystem.Context ctx, Action next)
    {
      // SYSTEME DE LANGUE
      int iLangueActive = NWScript.GetLocalInt(ctx.oSender, "_LANGUE_ACTIVE");

      if (iLangueActive != 0 && (ctx.channel == NWNX.Enum.ChatChannel.PlayerTalk || ctx.channel == NWNX.Enum.ChatChannel.PlayerWhisper || ctx.channel == NWNX.Enum.ChatChannel.DMTalk || ctx.channel == NWNX.Enum.ChatChannel.DMWhisper))
      {
        string sLanguageName = NWScript.Get2DAString("feat", "FEAT", iLangueActive);
        string sName = NWScript.GetLocalString(ctx.oSender, "__DISGUISE_NAME");
        if (sName == "") sName = NWScript.GetName(ctx.oSender);

        foreach (KeyValuePair<uint, PlayerSystem.Player> PlayerListEntry in PlayerSystem.Players)
        {
          if ((uint)ctx.oSender != PlayerListEntry.Key && (uint)ctx.oSender != NWScript.GetLocalObject(PlayerListEntry.Key, "_POSSESSING"))
          {
            uint oEavesdrop;

            if (NWScript.GetIsDM(PlayerListEntry.Key) && NWScript.GetIsObjectValid(NWScript.GetLocalObject(PlayerListEntry.Key, "_POSSESSING")))
              oEavesdrop = NWScript.GetLocalObject(PlayerListEntry.Key, "_POSSESSING");
            else
              oEavesdrop = PlayerListEntry.Key;

            if ((NWScript.GetArea(ctx.oSender) == NWScript.GetArea(oEavesdrop)) && (NWScript.GetDistanceBetween(oEavesdrop, ctx.oSender) < NWNX.Chat.GetChatHearingDistance(oEavesdrop.AsObject(), ctx.channel)))
            {
              if (NWScript.GetHasFeat(iLangueActive, oEavesdrop) || NWScript.GetIsDM(PlayerListEntry.Key) || NWScript.GetIsDMPossessed(oEavesdrop))
              {
                NWNX.Chat.SkipMessage();
                NWNX.Chat.SendMessage((int)ctx.channel, "[" + sLanguageName + "] " + ctx.msg, ctx.oSender, oEavesdrop.AsObject());
                NWScript.SendMessageToPC(oEavesdrop, sName + " : [" + sLanguageName + "] " + Languages.GetLangueStringConvertedHRPProtection(ctx.msg, iLangueActive));
              }
              else
              {
                NWNX.Chat.SkipMessage();
                NWNX.Chat.SendMessage((int)ctx.channel, Languages.GetLangueStringConvertedHRPProtection(ctx.msg, iLangueActive), ctx.oSender, oEavesdrop.AsObject());
              }
            }
          }
        }

        NWNX.Chat.SkipMessage();
        NWNX.Chat.SendMessage((int)ctx.channel, "[" + sLanguageName + "] " + ctx.msg, ctx.oSender, ctx.oSender);
        NWScript.SendMessageToPC(ctx.oSender, sName + " : [" + sLanguageName + "] " + Languages.GetLangueStringConvertedHRPProtection(ctx.msg, iLangueActive));
        return;
      }

      next();
    }
  }
}
