using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
    public static void ProcessDMListenMiddleware(Context ctx, Action next)
    {
      //SYSTEME DE RECOPIE DE CHAT POUR LES DMS
      if (ctx.channel == ChatPlugin.NWNX_CHAT_CHANNEL_PLAYER_TALK || ctx.channel == ChatPlugin.NWNX_CHAT_CHANNEL_PLAYER_WHISPER)
      {
        NwCreature oInviSender = NwModule.FindObjectsWithTag<NwCreature>("_invisible_sender").FirstOrDefault();
        foreach (NwPlayer oDM in NwModule.Instance.Players.Where(d => d.IsDM || d.IsDMPossessed || d.IsPlayerDM))
        {
          if (PlayerSystem.Players.TryGetValue(oDM, out PlayerSystem.Player dungeonMaster))
          {
            NwPlayer oPC = ctx.oSender.ToNwObject<NwPlayer>();
            if (dungeonMaster.listened.Contains(oPC))
            {
              if (oPC.Area != oDM.Area || oDM.Distance(oPC) > ChatPlugin.GetChatHearingDistance(oDM, ctx.channel))
              {
                oInviSender.Name = NWScript.GetName(ctx.oSender);

                var oPossessed = NWScript.GetLocalObject(oDM, "_POSSESSING");
                if (NWScript.GetIsObjectValid(oPossessed) != 1)
                  ChatPlugin.SendMessage(ChatPlugin.NWNX_CHAT_CHANNEL_PLAYER_TELL, "[COPIE - " + oPC.Area.Name + "] " + ctx.msg, oInviSender, oDM);
                else
                  ChatPlugin.SendMessage(ChatPlugin.NWNX_CHAT_CHANNEL_PLAYER_TELL, "[COPIE - " + oPC.Area.Name + "] " + ctx.msg, oInviSender, oPossessed);
              }
            }
          }
        }
      }

      next();
    }
    public static void ProcessLanguageMiddleware(Context ctx, Action next) // SYSTEME DE LANGUE
    {
      NwPlayer oPC = ctx.oSender.ToNwObject<NwPlayer>();
      int iLanguage = oPC.GetLocalVariable<int>("_ACTIVE_LANGUAGE").Value;
      if (iLanguage != (int)Feat.Invalid && (ctx.channel == ChatPlugin.NWNX_CHAT_CHANNEL_PLAYER_TALK || ctx.channel == ChatPlugin.NWNX_CHAT_CHANNEL_PLAYER_WHISPER || ctx.channel == ChatPlugin.NWNX_CHAT_CHANNEL_DM_TALK || ctx.channel == ChatPlugin.NWNX_CHAT_CHANNEL_DM_WHISPER))
      {
        string sLanguageName = Enum.GetName(typeof(Feat), iLanguage);
        //string sName = NWScript.GetLocalString(ctx.oSender, "__DISGUISE_NAME");
        //if (sName == "") sName = NWScript.GetName(ctx.oSender);

        foreach (NwPlayer players in NwModule.Instance.Players.Where(p => p.Area == oPC.Area && p.Distance(oPC) < ChatPlugin.GetChatHearingDistance(p, ctx.channel)))
        {
          if (oPC != players && oPC != NWScript.GetLocalObject(players, "_POSSESSING"))
          {
            NwGameObject oEavesdrop;

            if (players.IsDM && NWScript.GetIsObjectValid(NWScript.GetLocalObject(players, "_POSSESSING")) == 1)
              oEavesdrop = NWScript.GetLocalObject(players, "_POSSESSING").ToNwObject<NwGameObject>();
            else
              oEavesdrop = players;

            if (NWScript.GetHasFeat(oPC.GetLocalVariable<int>("_ACTIVE_LANGUAGE").Value, oEavesdrop) == 1 || players.IsDM || players.IsDMPossessed || players.IsPlayerDM)
            {
              ChatPlugin.SkipMessage();
              ChatPlugin.SendMessage(ctx.channel, "[" + sLanguageName + "] " + ctx.msg, ctx.oSender, oEavesdrop);
              NWScript.SendMessageToPC(oEavesdrop, oPC.Name + " : [" + sLanguageName + "] " + Languages.GetLangueStringConvertedHRPProtection(ctx.msg, (Feat)oPC.GetLocalVariable<int>("_ACTIVE_LANGUAGE").Value));
            }
            else
            {
              ChatPlugin.SkipMessage();
              ChatPlugin.SendMessage(ctx.channel, Languages.GetLangueStringConvertedHRPProtection(ctx.msg, (Feat)oPC.GetLocalVariable<int>("_ACTIVE_LANGUAGE").Value), ctx.oSender, oEavesdrop);
            }
          }
        }

        ChatPlugin.SkipMessage();
        ChatPlugin.SendMessage(ctx.channel, "[" + sLanguageName + "] " + ctx.msg, ctx.oSender, ctx.oSender);
        NWScript.SendMessageToPC(ctx.oSender, oPC.Name + " : [" + sLanguageName + "] " + Languages.GetLangueStringConvertedHRPProtection(ctx.msg, (Feat)oPC.GetLocalVariable<int>("_ACTIVE_LANGUAGE").Value));
        return;
      }

      next();
    }
  }
}
