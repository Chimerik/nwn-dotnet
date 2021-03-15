using System;
using System.IO;
using System.Linq;
using NLog;
using NWN.API;
using NWN.API.Constants;
using NWN.Core;
using NWN.Core.NWNX;
using NWN.Services;
using Action = System.Action;

namespace NWN.Systems
{
  [ServiceBinding(typeof(ChatSystem))]
  public class ChatSystem
  {
    public static readonly Logger Log = LogManager.GetCurrentClassLogger();
    public ChatSystem()
    {
      ChatPlugin.RegisterChatScript("on_chat");
    }

    [ScriptHandler("on_chat")]
    private void OnNWNXChatEvent(CallInfo callInfo)
    {
      if (ChatPlugin.GetChannel() == ChatPlugin.NWNX_CHAT_CHANNEL_SERVER_MSG)
        return;

      NwPlayer sender = ChatPlugin.GetSender().ToNwObjectSafe<NwPlayer>();

      if (sender == null)
        return;

      NwPlayer target = ChatPlugin.GetTarget().ToNwObjectSafe<NwPlayer>();

      pipeline.Execute(new Context(
        msg: ChatPlugin.GetMessage(),
        oSender: sender,
        oTarget: target,
        channel: ChatPlugin.GetChannel()
      ));
    }

    public class Context
    {
      public string msg { get; }
      public NwPlayer oSender { get; }
      public NwPlayer oTarget { get; }
      public int channel { get; }

      public Context(string msg, NwPlayer oSender, NwPlayer oTarget, int channel)
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
        if (ctx.oTarget != null)
        {
          string filename = String.Format("{0:yyyy-MM-dd}_{1}.txt", DateTime.Now, "chatlog");
          string path = Path.Combine(Environment.GetEnvironmentVariable("HOME") + "/ChatLog", filename);

          using (StreamWriter file =
          new StreamWriter(path, true))
            file.WriteLineAsync(DateTime.Now.ToShortTimeString() + " - [" + ctx.channel + " - " + ctx.oSender.Area.Name + "] " + NWScript.GetName(ctx.oSender, 1) + " : " + ctx.msg);
        }
        else
        {
          string filename = $"{ctx.oTarget.PlayerName}_{ctx.oSender.PlayerName}.txt";
          string path = Path.Combine(Environment.GetEnvironmentVariable("HOME") + "/ChatLog", filename);

          if (!File.Exists(path))
          {
            filename = $"{ctx.oSender.PlayerName}_{ctx.oTarget.PlayerName}.txt";
            path = Path.Combine(Environment.GetEnvironmentVariable("HOME") + "/ChatLog", filename);
          }
          using (StreamWriter file =
          new StreamWriter(path, true))
            file.WriteLineAsync(DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString() + " - " + ctx.oSender.Name + " To : " + NWScript.GetName(ctx.oTarget, 1) + " : " + ctx.msg);
        }

        if (PlayerSystem.Players.TryGetValue(ctx.oSender, out PlayerSystem.Player player) && (player.oid.GetLocalVariable<int>("_PLAYER_INPUT").HasValue || player.oid.GetLocalVariable<int>("_PLAYER_INPUT_STRING").HasValue))
        {
          if (Int32.TryParse(ctx.msg, out int value))
          {
            player.setValue = value;
            player.oid.GetLocalVariable<int>("_PLAYER_INPUT").Delete();
            ChatPlugin.SkipMessage();
            return;
          }
          else
          {
            player.setString = ctx.msg;
            player.oid.GetLocalVariable<int>("_PLAYER_INPUT_STRING").Delete();
            ChatPlugin.SkipMessage();
          }
        }
           
        next();
    }
    public static void ProcessMutePMMiddleware(Context ctx, Action next)
    {
      if (ctx.oTarget != null)
      {
        if (ObjectPlugin.GetInt(ctx.oTarget, "__BLOCK_ALL_MP") > 0 || ObjectPlugin.GetInt(ctx.oTarget, "__BLOCK_" + NWScript.GetName(ctx.oSender, 1) + "_MP") > 0)
          if (!ctx.oTarget.IsDM)
          {
            ChatPlugin.SkipMessage();
            ctx.oSender.SendServerMessage($"{ctx.oTarget.Name.ColorString(Color.WHITE)} bloque actuellement la réception des mp.", Color.ORANGE);
            return;
          }
      }

      next();
    }
    public static void ProcessPMMiddleware(Context ctx, Action next)
    {
      if (ctx.oTarget != null)
      {
        ChatPlugin.SkipMessage();
        if (ctx.oTarget.GetLocalVariable<NwObject>("_POSSESSING").HasValue)
          ChatPlugin.SendMessage(ctx.channel, ctx.msg, ctx.oSender, ctx.oTarget.GetLocalVariable<NwObject>("_POSSESSING").Value);
        else
          ChatPlugin.SendMessage(ctx.channel, ctx.msg, ctx.oSender, ctx.oTarget);
        return;
      }
      else if (ctx.channel == ChatPlugin.NWNX_CHAT_CHANNEL_PLAYER_TELL || ctx.channel == ChatPlugin.NWNX_CHAT_CHANNEL_DM_TELL)
      {
        ChatPlugin.SkipMessage();
        ctx.oSender.SendServerMessage("La personne à laquelle vous tentez d'envoyer un message n'est plus connectée.", Color.ORANGE);
        return;
      }

      next();
    }
    public static void ProcessAFKDetectionMiddleware(Context ctx, Action next)
    {
      if (PlayerSystem.Players.TryGetValue(ctx.oSender, out PlayerSystem.Player player))
      {
        if (player.isAFK)
          if (ctx.channel == ChatPlugin.NWNX_CHAT_CHANNEL_PLAYER_TALK || ctx.channel == ChatPlugin.NWNX_CHAT_CHANNEL_PLAYER_WHISPER)
            if (!ctx.msg.Contains("(") && !ctx.msg.Contains(")"))
              if (ctx.oSender.GetNearestCreatures(CreatureTypeFilter.PlayerChar(true)).Any(p => p.Distance(ctx.oSender) < ChatPlugin.GetChatHearingDistance(p, ctx.channel)))
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
        if (oInviSender != null)
        {
          foreach (NwPlayer oDM in NwModule.Instance.Players.Where(d => d.IsDM || d.IsDMPossessed || d.IsPlayerDM))
          {
            if (PlayerSystem.Players.TryGetValue(oDM, out PlayerSystem.Player dungeonMaster))
            {
              if (dungeonMaster.listened.Contains(ctx.oSender))
              {
                if (ctx.oSender.Area != oDM.Area || oDM.Distance(ctx.oSender) > ChatPlugin.GetChatHearingDistance(oDM, ctx.channel))
                {
                  oInviSender.Name = ctx.oSender.Name;

                  if (oDM.GetLocalVariable<NwObject>("_POSSESSING").HasNothing)
                    ChatPlugin.SendMessage(ChatPlugin.NWNX_CHAT_CHANNEL_PLAYER_TELL, "[COPIE - " + ctx.oSender.Area.Name + "] " + ctx.msg, oInviSender, oDM);
                  else
                  {
                    NwCreature oPossessed = (NwCreature)oDM.GetLocalVariable<NwObject>("_POSSESSING").Value;
                    if(oPossessed != null)
                      ChatPlugin.SendMessage(ChatPlugin.NWNX_CHAT_CHANNEL_PLAYER_TELL, "[COPIE - " + ctx.oSender.Area.Name + "] " + ctx.msg, oInviSender, oPossessed);
                  }
                }
              }
            }
          }
        }
        else
          Utils.LogMessageToDMs("Warning - Invisible Sender not set");
      }

      next();
    }
    public static void ProcessLanguageMiddleware(Context ctx, Action next) // SYSTEME DE LANGUE
    {
      int iLanguage = ctx.oSender.GetLocalVariable<int>("_ACTIVE_LANGUAGE").Value;
      if (iLanguage != (int)CustomFeats.Invalid && (ctx.channel == ChatPlugin.NWNX_CHAT_CHANNEL_PLAYER_TALK || ctx.channel == ChatPlugin.NWNX_CHAT_CHANNEL_PLAYER_WHISPER || ctx.channel == ChatPlugin.NWNX_CHAT_CHANNEL_DM_TALK || ctx.channel == ChatPlugin.NWNX_CHAT_CHANNEL_DM_WHISPER))
      {
        string sLanguageName = Enum.GetName(typeof(CustomFeats), iLanguage);
        //string sName = NWScript.GetLocalString(ctx.oSender, "__DISGUISE_NAME");
        //if (sName == "") sName = NWScript.GetName(ctx.oSender);

        foreach (NwPlayer players in NwModule.Instance.Players.Where(p => p.Area == ctx.oSender.Area && p.Distance(ctx.oSender) < ChatPlugin.GetChatHearingDistance(p, ctx.channel)))
        {
          if (ctx.oSender != players && ctx.oSender != NWScript.GetLocalObject(players, "_POSSESSING"))
          {
            NwGameObject oEavesdrop;

            if (players.IsDM && NWScript.GetIsObjectValid(NWScript.GetLocalObject(players, "_POSSESSING")) == 1)
              oEavesdrop = NWScript.GetLocalObject(players, "_POSSESSING").ToNwObject<NwGameObject>();
            else
              oEavesdrop = players;

            if (NWScript.GetHasFeat(ctx.oSender.GetLocalVariable<int>("_ACTIVE_LANGUAGE").Value, oEavesdrop) == 1 || players.IsDM || players.IsDMPossessed || players.IsPlayerDM)
            {
              ChatPlugin.SkipMessage();
              ChatPlugin.SendMessage(ctx.channel, "[" + sLanguageName + "] " + ctx.msg, ctx.oSender, oEavesdrop);
              NWScript.SendMessageToPC(oEavesdrop, ctx.oSender.Name + " : [" + sLanguageName + "] " + Languages.GetLangueStringConvertedHRPProtection(ctx.msg, (Feat)ctx.oSender.GetLocalVariable<int>("_ACTIVE_LANGUAGE").Value));
            }
            else
            {
              ChatPlugin.SkipMessage();
              ChatPlugin.SendMessage(ctx.channel, Languages.GetLangueStringConvertedHRPProtection(ctx.msg, (Feat)ctx.oSender.GetLocalVariable<int>("_ACTIVE_LANGUAGE").Value), ctx.oSender, oEavesdrop);
            }
          }
        }

        ChatPlugin.SkipMessage();
        ChatPlugin.SendMessage(ctx.channel, "[" + sLanguageName + "] " + ctx.msg, ctx.oSender, ctx.oSender);
        NWScript.SendMessageToPC(ctx.oSender, ctx.oSender.Name + " : [" + sLanguageName + "] " + Languages.GetLangueStringConvertedHRPProtection(ctx.msg, (Feat)ctx.oSender.GetLocalVariable<int>("_ACTIVE_LANGUAGE").Value));
        return;
      }

      next();
    }
  }
}
