using System;
using System.Collections.Generic;
using System.IO;
using NWN.Core;
using NWN.Core.NWNX;

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
      ChatPlugin.RegisterChatScript(CHAT_SCRIPT);
    }

    public class Context
    {
      public string msg { get; }
      public uint oSender { get; }
      public uint oTarget { get; }
      public int channel { get; }

      public Context(string msg, uint oSender, uint oTarget, int channel)
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
        msg: ChatPlugin.GetMessage(),
        oSender: ChatPlugin.GetSender(),
        oTarget: ChatPlugin.GetTarget(),
        channel: ChatPlugin.GetChannel()
      ));

      return 0;
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
        if (NWScript.GetIsObjectValid(ctx.oTarget) != 1)
          file.WriteLine(DateTime.Now.ToShortTimeString() + " - [" + ctx.channel + " - " + NWScript.GetName(NWScript.GetArea(ctx.oSender)) + "] " + NWScript.GetName(ctx.oSender, 1) + " : " + ctx.msg);
        else
          file.WriteLine(DateTime.Now.ToShortTimeString() + " - [" + ctx.channel + "] " + NWScript.GetName(ctx.oSender) + " To : " + NWScript.GetName(ctx.oTarget, 1) + " : " + ctx.msg);
      }

      next();
    }
    public static void ProcessSpeakValueMiddleware(ChatSystem.Context ctx, Action next)
    {
      if (NWScript.GetLocalString(ctx.oSender, "_IS_LISTENING_VAR") == NWScript.GetName(ctx.oSender))
      {
        ChatPlugin.SkipMessage();
        NWScript.SetLocalString(ctx.oSender, "_LISTENING_VAR", ctx.msg);
        NWScript.DeleteLocalString(ctx.oSender, "_IS_LISTENING_VAR");
        return;
      }

      next();
    }
    public static void ProcessMutePMMiddleware(ChatSystem.Context ctx, Action next)
    {
      if (NWScript.GetIsObjectValid(ctx.oTarget) == 1)
      {
        if (ObjectPlugin.GetInt(ctx.oTarget, "__BLOCK_ALL_MP") > 0 || ObjectPlugin.GetInt(ctx.oTarget, "__BLOCK_" + NWScript.GetName(ctx.oSender, 1) + "_MP") > 0)
          if (NWScript.GetIsDM(ctx.oTarget) != 1)
          {
            ChatPlugin.SkipMessage();
            NWScript.SendMessageToPC(ctx.oSender, NWScript.GetName(ctx.oTarget) + " bloque actuellement la réception des mp. Votre message n'a pas pu être transmis");
            return;
          }
      }

      next();
    }
    public static void ProcessPMMiddleware(ChatSystem.Context ctx, Action next)
    {
      if(NWScript.GetIsObjectValid(ctx.oTarget) == 1)
      {
        ChatPlugin.SkipMessage();
        if (NWScript.GetIsObjectValid(NWScript.GetLocalObject(ctx.oTarget, "_POSSESSING")) == 1)
          ChatPlugin.SendMessage((int)ctx.channel, ctx.msg, ctx.oSender, NWScript.GetLocalObject(ctx.oTarget, "_POSSESSING"));
        else
          ChatPlugin.SendMessage((int)ctx.channel, ctx.msg, ctx.oSender, ctx.oTarget);
        return;
      }
      else if (ctx.channel == ChatPlugin.NWNX_CHAT_CHANNEL_PLAYER_TELL || ctx.channel == ChatPlugin.NWNX_CHAT_CHANNEL_DM_TELL)
      {
        ChatPlugin.SkipMessage();
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
          if (ctx.channel == ChatPlugin.NWNX_CHAT_CHANNEL_PLAYER_TALK || ctx.channel == ChatPlugin.NWNX_CHAT_CHANNEL_DM_WHISPER)
            if (!ctx.msg.Contains("(") && !ctx.msg.Contains(")"))
              if (NWScript.GetDistanceBetween(ctx.oSender, NWScript.GetNearestCreature(1, 1, ctx.oSender)) < 35.0f)
                player.isAFK = false;
      }

      next();
    }
    public static void ProcessDeadPlayerMiddleware(ChatSystem.Context ctx, Action next)
    {
      if (NWScript.GetIsDead(ctx.oSender) == 1)
        NWScript.SendMessageToPC(ctx.oSender, "N'oubliez pas que vous êtes inconscient, vous ne pouvez pas parler, mais tout juste gémir et décrire votre état");

      next();
    }

    public static void ProcessDMListenMiddleware(ChatSystem.Context ctx, Action next)
    {
      //SYSTEME DE RECOPIE DE CHAT POUR LES DMS
      if (ctx.channel == ChatPlugin.NWNX_CHAT_CHANNEL_PLAYER_TALK || ctx.channel == ChatPlugin.NWNX_CHAT_CHANNEL_DM_WHISPER)
      {
        var oInviSender = NWScript.GetLocalObject(NWScript.GetModule(), "_INVISIBLE_SENDER"); // TODO : créer un _INVISIBLE_SENDER dans le module
        foreach (KeyValuePair<uint, PlayerSystem.Player> PlayerListEntry in PlayerSystem.Players)
        {
          PlayerSystem.Player oDM = PlayerListEntry.Value;
          if (NWScript.GetIsDM(oDM.oid) == 1)
          {
            if (oDM.listened.ContainsKey(ctx.oSender))
            {
              if ((NWScript.GetArea(ctx.oSender) != NWScript.GetArea(oDM.oid)) || NWScript.GetDistanceBetween(oDM.oid, ctx.oSender) > ChatPlugin.GetChatHearingDistance(oDM.oid, ctx.channel))
              {
                NWScript.SetName(oInviSender, NWScript.GetName(ctx.oSender));
                var oPossessed = NWScript.GetLocalObject(oDM.oid, "_POSSESSING");
                if (NWScript.GetIsObjectValid(oPossessed) != 1)
                    ChatPlugin.SendMessage((int)ChatPlugin.NWNX_CHAT_CHANNEL_PLAYER_TELL, "[COPIE - " + NWScript.GetName(NWScript.GetArea(ctx.oSender)) + "] " + ctx.msg, oInviSender, oDM.oid);
                else
                  ChatPlugin.SendMessage((int)ChatPlugin.NWNX_CHAT_CHANNEL_PLAYER_TELL, "[COPIE - " + NWScript.GetName(NWScript.GetArea(ctx.oSender)) + "] " + ctx.msg, oInviSender, oPossessed);
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

      if (iLangueActive != 0 && (ctx.channel == ChatPlugin.NWNX_CHAT_CHANNEL_PLAYER_TALK || ctx.channel == ChatPlugin.NWNX_CHAT_CHANNEL_DM_WHISPER || ctx.channel == ChatPlugin.NWNX_CHAT_CHANNEL_DM_TALK || ctx.channel == ChatPlugin.NWNX_CHAT_CHANNEL_DM_WHISPER))
      {
        string sLanguageName = NWScript.Get2DAString("feat", "FEAT", iLangueActive);
        string sName = NWScript.GetLocalString(ctx.oSender, "__DISGUISE_NAME");
        if (sName == "") sName = NWScript.GetName(ctx.oSender);

        foreach (KeyValuePair<uint, PlayerSystem.Player> PlayerListEntry in PlayerSystem.Players)
        {
          if ((uint)ctx.oSender != PlayerListEntry.Key && (uint)ctx.oSender != NWScript.GetLocalObject(PlayerListEntry.Key, "_POSSESSING"))
          {
            uint oEavesdrop;

            if (NWScript.GetIsDM(PlayerListEntry.Key) == 1 && NWScript.GetIsObjectValid(NWScript.GetLocalObject(PlayerListEntry.Key, "_POSSESSING")) == 1)
              oEavesdrop = NWScript.GetLocalObject(PlayerListEntry.Key, "_POSSESSING");
            else
              oEavesdrop = PlayerListEntry.Key;

            if ((NWScript.GetArea(ctx.oSender) == NWScript.GetArea(oEavesdrop)) && (NWScript.GetDistanceBetween(oEavesdrop, ctx.oSender) < ChatPlugin.GetChatHearingDistance(oEavesdrop, ctx.channel)))
            {
              if (NWScript.GetHasFeat(iLangueActive, oEavesdrop) == 1 || NWScript.GetIsDM(PlayerListEntry.Key) == 1 || NWScript.GetIsDMPossessed(oEavesdrop) == 1)
              {
                ChatPlugin.SkipMessage();
                ChatPlugin.SendMessage((int)ctx.channel, "[" + sLanguageName + "] " + ctx.msg, ctx.oSender, oEavesdrop);
                NWScript.SendMessageToPC(oEavesdrop, sName + " : [" + sLanguageName + "] " + Languages.GetLangueStringConvertedHRPProtection(ctx.msg, (Feat)iLangueActive));
              }
              else
              {
                ChatPlugin.SkipMessage();
                ChatPlugin.SendMessage((int)ctx.channel, Languages.GetLangueStringConvertedHRPProtection(ctx.msg, (Feat)iLangueActive), ctx.oSender, oEavesdrop);
              }
            }
          }
        }

        ChatPlugin.SkipMessage();
        ChatPlugin.SendMessage((int)ctx.channel, "[" + sLanguageName + "] " + ctx.msg, ctx.oSender, ctx.oSender);
        NWScript.SendMessageToPC(ctx.oSender, sName + " : [" + sLanguageName + "] " + Languages.GetLangueStringConvertedHRPProtection(ctx.msg, (Feat)iLangueActive));
        return;
      }

      next();
    }
  }
}
