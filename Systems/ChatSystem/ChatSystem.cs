using System;
using System.Collections.Generic;
using System.IO;
using NWN.API;
using NWN.API.Events;
using NWN.Core;
using NWN.Core.NWNX;
using NWN.Services;

namespace NWN.Systems
{
  [ServiceBinding(typeof(ChatSystem))]
  public class ChatSystem
  {
    public ChatSystem(NativeEventService eventService)
    {
      eventService.Subscribe<NwModule, ModuleEvents.OnModuleLoad>(NwModule.Instance, OnModuleLoad);
    }
    private void OnModuleLoad(ModuleEvents.OnModuleLoad onModuleLoad)
    {
      ChatPlugin.RegisterChatScript("on_chat");
    }

    [ScriptHandler("on_chat")]
    private void OnNWNXChatEvent(CallInfo callInfo)
    {
      pipeline.Execute(new Context(
        msg: ChatPlugin.GetMessage(),
        oSender: ChatPlugin.GetSender(),
        oTarget: ChatPlugin.GetTarget(),
        channel: ChatPlugin.GetChannel()
      ));
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

    private static Pipeline<Context> pipeline = new Pipeline<Context>(
      new Action<Context, Action>[]
      {
            ChatSystem.ProcessWriteLogMiddleware,
            CommandSystem.ProcessChatCommandMiddleware,
            ChatSystem.ProcessMutePMMiddleware,
            ChatSystem.ProcessPMMiddleware,
            ChatSystem.ProcessAFKDetectionMiddleware,
            ChatSystem.ProcessDeadPlayerMiddleware,
            ChatSystem.ProcessDMListenMiddleware,
            ChatSystem.ProcessLanguageMiddleware
      }
    );
    public static void ProcessWriteLogMiddleware(Context ctx, Action next)
    {
      if (ctx.channel != ChatPlugin.NWNX_CHAT_CHANNEL_SERVER_MSG && ctx.oSender.ToNwObjectSafe<NwPlayer>() != null)
      {
        if (ctx.oTarget.ToNwObjectSafe<NwPlayer>() != null)
        {
          string filename = String.Format("{0:yyyy-MM-dd}_{1}.txt", DateTime.Now, "chatlog");
          string path = Path.Combine(Environment.GetEnvironmentVariable("HOME") + "/ChatLog", filename);

          using (StreamWriter file =
          new StreamWriter(path, true))
            file.WriteLineAsync(DateTime.Now.ToShortTimeString() + " - [" + ctx.channel + " - " + NWScript.GetName(NWScript.GetArea(ctx.oSender)) + "] " + NWScript.GetName(ctx.oSender, 1) + " : " + ctx.msg);
        }
        else
        {
          string filename = $"{NWScript.GetPCPlayerName(ctx.oTarget)}_{NWScript.GetPCPlayerName(ctx.oSender)}.txt";
          string path = Path.Combine(Environment.GetEnvironmentVariable("HOME") + "/ChatLog", filename);

          if (!File.Exists(path))
          {
            filename = $"{NWScript.GetPCPlayerName(ctx.oSender)}_{NWScript.GetPCPlayerName(ctx.oTarget)}.txt";
            path = Path.Combine(Environment.GetEnvironmentVariable("HOME") + "/ChatLog", filename);
          }
          using (StreamWriter file =
          new StreamWriter(path, true))
            file.WriteLineAsync(DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString() + " -  " + NWScript.GetName(ctx.oSender) + " To : " + NWScript.GetName(ctx.oTarget, 1) + " : " + ctx.msg);
        }

        next();
      }
    }
    public static void ProcessMutePMMiddleware(Context ctx, Action next)
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
    public static void ProcessPMMiddleware(Context ctx, Action next)
    {
      if (NWScript.GetIsObjectValid(ctx.oTarget) == 1)
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
        if (player.isAFK)
          if (ctx.channel == ChatPlugin.NWNX_CHAT_CHANNEL_PLAYER_TALK || ctx.channel == ChatPlugin.NWNX_CHAT_CHANNEL_PLAYER_WHISPER)
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

    public static void ProcessDMListenMiddleware(Context ctx, Action next)
    {
      //SYSTEME DE RECOPIE DE CHAT POUR LES DMS
      if (ctx.channel == ChatPlugin.NWNX_CHAT_CHANNEL_PLAYER_TALK || ctx.channel == ChatPlugin.NWNX_CHAT_CHANNEL_PLAYER_WHISPER)
      {
        var oInviSender = NWScript.GetObjectByTag("_invisible_sender"); // TODO : créer un _INVISIBLE_SENDER dans le module
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
    public static void ProcessLanguageMiddleware(Context ctx, Action next) // SYSTEME DE LANGUE
    {
      if (PlayerSystem.Players.TryGetValue(ctx.oSender, out PlayerSystem.Player player))
      {
        if (player.oid.GetLocalVariable<int>("_ACTIVE_LANGUAGE").Value != (int)Feat.Invalid && (ctx.channel == ChatPlugin.NWNX_CHAT_CHANNEL_PLAYER_TALK || ctx.channel == ChatPlugin.NWNX_CHAT_CHANNEL_PLAYER_WHISPER || ctx.channel == ChatPlugin.NWNX_CHAT_CHANNEL_DM_TALK || ctx.channel == ChatPlugin.NWNX_CHAT_CHANNEL_DM_WHISPER))
        {
          string sLanguageName = Enum.GetName(typeof(Feat), player.oid.GetLocalVariable<int>("_ACTIVE_LANGUAGE").Value);
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
                if (NWScript.GetHasFeat(player.oid.GetLocalVariable<int>("_ACTIVE_LANGUAGE").Value, oEavesdrop) == 1 || NWScript.GetIsDM(PlayerListEntry.Key) == 1 || NWScript.GetIsDMPossessed(oEavesdrop) == 1)
                {
                  ChatPlugin.SkipMessage();
                  ChatPlugin.SendMessage((int)ctx.channel, "[" + sLanguageName + "] " + ctx.msg, ctx.oSender, oEavesdrop);
                  NWScript.SendMessageToPC(oEavesdrop, sName + " : [" + sLanguageName + "] " + Languages.GetLangueStringConvertedHRPProtection(ctx.msg, (Feat)player.oid.GetLocalVariable<int>("_ACTIVE_LANGUAGE").Value));
                }
                else
                {
                  ChatPlugin.SkipMessage();
                  ChatPlugin.SendMessage((int)ctx.channel, Languages.GetLangueStringConvertedHRPProtection(ctx.msg, (Feat)player.oid.GetLocalVariable<int>("_ACTIVE_LANGUAGE").Value), ctx.oSender, oEavesdrop);
                }
              }
            }
          }

          ChatPlugin.SkipMessage();
          ChatPlugin.SendMessage((int)ctx.channel, "[" + sLanguageName + "] " + ctx.msg, ctx.oSender, ctx.oSender);
          NWScript.SendMessageToPC(ctx.oSender, sName + " : [" + sLanguageName + "] " + Languages.GetLangueStringConvertedHRPProtection(ctx.msg, (Feat)player.oid.GetLocalVariable<int>("_ACTIVE_LANGUAGE").Value));
          return;
        }

        next();
      }
    }
  }
}
