using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NLog;
using NWN.API;
using NWN.API.Constants;
using NWN.API.Events;
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
    private static string areaName = "";
    private static Dictionary<NwGameObject, string> chatReceivers = new Dictionary<NwGameObject, string>();
    public ChatSystem()
    {
      ChatPlugin.RegisterChatScript("on_chat");
    }

    [ScriptHandler("on_chat")]
    private void OnNWNXChatEvent(CallInfo callInfo)
    {
      if (ChatPlugin.GetChannel() == ChatPlugin.NWNX_CHAT_CHANNEL_SERVER_MSG)
        return;

      NwCreature oSender = ChatPlugin.GetSender().ToNwObjectSafe<NwCreature>();

      if (oSender.ControllingPlayer == null || oSender.GetLocalVariable<string>("_AWAITING_PLAYER_INPUT").HasValue)
        return;

      NwCreature target = ChatPlugin.GetTarget().ToNwObjectSafe<NwCreature>();
      NwPlayer targetPlayer = null;

      if (target != null)
        targetPlayer = target.ControllingPlayer;

      if (oSender.Area != null)
        areaName = oSender.Area.Name;
      else
        areaName = "Entre deux zones";

      chatReceivers.Clear();
 
      pipeline.Execute(new Context(
        msg: ChatPlugin.GetMessage(),
        oSender: oSender.ControllingPlayer,
        oTarget: targetPlayer,
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
            //ChatSystem.ProcessPMMiddleware,
            ChatSystem.ProcessAFKDetectionMiddleware,
            ChatSystem.ProcessDMListenMiddleware,
            ChatSystem.ProcessGetChatReceiversMiddleware,
            ChatSystem.ProcessChatColorMiddleware,
      }
    );
    public static void ProcessWriteLogMiddleware(Context ctx, Action next)
    {
      if (ctx.oTarget == null)
      {
        string filename = String.Format("{0:yyyy-MM-dd}_{1}.txt", DateTime.Now, "chatlog");
        string path = Path.Combine(Environment.GetEnvironmentVariable("HOME") + "/ChatLog", filename);

        using (StreamWriter file =
        new StreamWriter(path, true))
          file.WriteLineAsync(DateTime.Now.ToShortTimeString() + " - [" + ctx.channel + " - " + areaName + "] " + NWScript.GetName(ctx.oSender.ControlledCreature, 1) + " : " + ctx.msg);
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
          file.WriteLineAsync(DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString() + " - " + ctx.oSender.LoginCreature.Name + " To : " + NWScript.GetName(ctx.oTarget.LoginCreature, 1) + " : " + ctx.msg);
      }

      next();
    }
    public static void ProcessMutePMMiddleware(Context ctx, Action next)
    {
      if (ctx.oTarget != null)
      {
        if (ObjectPlugin.GetInt(ctx.oTarget.LoginCreature, "__BLOCK_ALL_MP") > 0 || ObjectPlugin.GetInt(ctx.oTarget.LoginCreature, "__BLOCK_" + NWScript.GetName(ctx.oSender.LoginCreature, 1) + "_MP") > 0)
          if (!ctx.oTarget.IsDM)
          {
            ChatPlugin.SkipMessage();
            ctx.oSender.SendServerMessage($"{ctx.oTarget.LoginCreature.Name.ColorString(ColorConstants.White)} bloque actuellement la réception des mp.", ColorConstants.Orange);
            return;
          }
      }

      next();
    }
    public static void HandlePM(Context ctx)
    {
      if (ctx.oTarget != null)
      {
        /*if (ctx.oTarget.GetLocalVariable<NwObject>("_POSSESSING").HasValue)
          ChatPlugin.SendMessage(ctx.channel, ctx.msg, ctx.oSender, ctx.oTarget.GetLocalVariable<NwObject>("_POSSESSING").Value);
        else*/
        if (!chatReceivers.ContainsKey(ctx.oTarget.LoginCreature))
          chatReceivers.Add(ctx.oTarget.LoginCreature, ctx.msg);
          //ChatPlugin.SendMessage(ctx.channel, ctx.msg, ctx.oSender, ctx.oTarget);
        ///return;
      }
      else
      {
        ChatPlugin.SkipMessage();
        ctx.oSender.SendServerMessage("La personne à laquelle vous tentez d'envoyer un message n'est plus connectée.", ColorConstants.Orange);
        return;
      }
    }
    public static void ProcessAFKDetectionMiddleware(Context ctx, Action next)
    {
      if (PlayerSystem.Players.TryGetValue(ctx.oSender.LoginCreature, out PlayerSystem.Player player))
      {
        if (player.isAFK)
          if (ctx.channel == ChatPlugin.NWNX_CHAT_CHANNEL_PLAYER_TALK || ctx.channel == ChatPlugin.NWNX_CHAT_CHANNEL_PLAYER_WHISPER)
            if (!ctx.msg.Contains("(") && !ctx.msg.Contains(")"))
              if (ctx.oSender.LoginCreature.GetNearestCreatures(CreatureTypeFilter.PlayerChar(true)).Any(p => p.Distance(ctx.oSender.LoginCreature) < ChatPlugin.GetChatHearingDistance(p, ctx.channel)))
                player.isAFK = false;
      }

      next();
    }
    public static void ProcessDMListenMiddleware(Context ctx, Action next)
    {
      //SYSTEME DE RECOPIE DE CHAT POUR LES DMS
      if (ctx.channel == ChatPlugin.NWNX_CHAT_CHANNEL_PLAYER_TALK || ctx.channel == ChatPlugin.NWNX_CHAT_CHANNEL_PLAYER_WHISPER)
      {
        NwCreature oInviSender = NwObject.FindObjectsWithTag<NwCreature>("_invisible_sender").FirstOrDefault();
        if (oInviSender != null)
        {
          foreach (NwPlayer oDM in NwModule.Instance.Players.Where(d => d.IsDM))
          {
            if (PlayerSystem.Players.TryGetValue(oDM.LoginCreature, out PlayerSystem.Player dungeonMaster))
            {
              if (dungeonMaster.listened.Contains(ctx.oSender))
              {
                if (ctx.oSender.ControlledCreature.Area != oDM.ControlledCreature.Area || oDM.ControlledCreature.Distance(ctx.oSender.ControlledCreature) > ChatPlugin.GetChatHearingDistance(oDM.LoginCreature, ctx.channel))
                {
                  oInviSender.Name = ctx.oSender.ControlledCreature.Name;
                  ChatPlugin.SendMessage(ChatPlugin.NWNX_CHAT_CHANNEL_PLAYER_TELL, "[COPIE - " + areaName + "] " + ctx.msg, oInviSender, oDM.ControlledCreature);
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
    public static void ProcessGetChatReceiversMiddleware(Context ctx, Action next) // SYSTEME DE LANGUE
    {
      switch (ctx.channel)
      {
        case ChatPlugin.NWNX_CHAT_CHANNEL_PLAYER_TALK:
        case ChatPlugin.NWNX_CHAT_CHANNEL_PLAYER_WHISPER:
        case ChatPlugin.NWNX_CHAT_CHANNEL_DM_TALK:
        case ChatPlugin.NWNX_CHAT_CHANNEL_DM_WHISPER:
          HandleLanguage(ctx);
          break;
        case ChatPlugin.NWNX_CHAT_CHANNEL_PLAYER_PARTY:
        case ChatPlugin.NWNX_CHAT_CHANNEL_DM_PARTY:
          foreach (NwPlayer oPartyMember in ctx.oSender.PartyMembers)
            if (!chatReceivers.ContainsKey(oPartyMember.ControlledCreature))
              chatReceivers.Add(oPartyMember.ControlledCreature, ctx.msg);
          break;
        case ChatPlugin.NWNX_CHAT_CHANNEL_PLAYER_TELL:
        case ChatPlugin.NWNX_CHAT_CHANNEL_DM_TELL:
          HandlePM(ctx);
          break;
        case ChatPlugin.NWNX_CHAT_CHANNEL_PLAYER_DM:
        case ChatPlugin.NWNX_CHAT_CHANNEL_DM_DM:
          foreach (NwPlayer oDM in NwModule.Instance.Players.Where(p => p.IsDM))
            if (!chatReceivers.ContainsKey(oDM.ControlledCreature))
              chatReceivers.Add(oDM.ControlledCreature, ctx.msg);
          break;
        case ChatPlugin.NWNX_CHAT_CHANNEL_DM_SHOUT:
          foreach (NwPlayer oPC in NwModule.Instance.Players)
            if (!chatReceivers.ContainsKey(oPC.ControlledCreature))
              chatReceivers.Add(oPC.ControlledCreature, ctx.msg);
          break;
      }
      next();
    }
    public static void HandleLanguage(Context ctx)
    {
      foreach (NwPlayer player in NwModule.Instance.Players.Where(p => p.ControlledCreature.Area == ctx.oSender.ControlledCreature.Area && p.ControlledCreature.Distance(ctx.oSender.ControlledCreature) < ChatPlugin.GetChatHearingDistance(p.LoginCreature, ctx.channel)))
      {
        Feat language = (Feat)ctx.oSender.ControlledCreature.GetLocalVariable<int>("_ACTIVE_LANGUAGE").Value;

        //string sName = NWScript.GetLocalString(ctx.oSender, "__DISGUISE_NAME");
        //if (sName == "") sName = NWScript.GetName(ctx.oSender);

        if (language != (Feat)CustomFeats.Invalid)
        {
          string sLanguageName = SkillSystem.customFeatsDictionnary[language].name;
          if (player.LoginCreature.KnowsFeat(language) || player.IsDM)
          {
            chatReceivers.Add(player.ControlledCreature, "[" + sLanguageName + "] " + ctx.msg);
            player.SendServerMessage(ctx.oSender.ControlledCreature + " : [" + sLanguageName + "] " + Languages.GetLangueStringConvertedHRPProtection(ctx.msg, language));
          }
          else
            chatReceivers.Add(player.ControlledCreature, Languages.GetLangueStringConvertedHRPProtection(ctx.msg, language));
        }
        else
          chatReceivers.Add(player.ControlledCreature, ctx.msg);
      }
    }
    public static void ProcessChatColorMiddleware(Context ctx, Action next)
    {
      foreach (KeyValuePair<NwGameObject, string> chatReceiver in chatReceivers)
      {
        ChatPlugin.SkipMessage();

        if (!PlayerSystem.Players.TryGetValue(chatReceiver.Key, out PlayerSystem.Player player))
        {
          ChatPlugin.SendMessage(ctx.channel, chatReceiver.Value, ctx.oSender.ControlledCreature, chatReceiver.Key);
          return;
        }
          
        string coloredChat = chatReceiver.Value;

        if (player.chatColors.ContainsKey(ctx.channel))
          coloredChat = chatReceiver.Value.ColorString(player.chatColors[ctx.channel]);

        if (player.chatColors.ContainsKey(100)) // 100 = emote
          coloredChat = HandleEmoteColoration(player, coloredChat);

        ChatPlugin.SendMessage(ctx.channel, coloredChat, ctx.oSender.ControlledCreature, chatReceiver.Key);
      }
      next();
    }
    public static string HandleEmoteColoration(PlayerSystem.Player player, string chat)
    {
      int starCount = chat.ToCharArray().Count(c => c == '*'); 

      if (starCount == 1 && player.chatColors.ContainsKey(101)) // 101 = chat correctif
          return chat.StripColors().ColorString(player.chatColors[101]);
      else if (starCount > 1)
      {
        string[] sArray = chat.Split('*', '*');
        string sColored = "";
        int i = 0;

        foreach (string s in sArray)
        {
          if (i % 2 == 0)
            sColored += s;
          else
            sColored += $" * {s} * ".ColorString(player.chatColors[100]);
          // test : color vert mp = new Color(32, 255, 32)

          i++;
        }

        return sColored;
      }

      return chat;
    }
    public static void HandlePlayerInputByte(ModuleEvents.OnPlayerChat onChat)
    {
      onChat.Volume = TalkVolume.SilentTalk;

      if(!byte.TryParse(onChat.Message, out byte input))
      {
        onChat.Sender.SendServerMessage($"{onChat.Message} n'est pas une entrée valide. La valeur doit être comprise entre 0 et 255.");
        return;
      }

      onChat.Sender.LoginCreature.GetLocalVariable<string>("_PLAYER_INPUT").Value = onChat.Message;
      onChat.Sender.LoginCreature.GetLocalVariable<string>("_AWAITING_PLAYER_INPUT").Delete();
      onChat.Sender.OnPlayerChat -= HandlePlayerInputByte;
    }
    public static void HandlePlayerInputInt(ModuleEvents.OnPlayerChat onChat)
    {
      onChat.Volume = TalkVolume.SilentTalk;

      if (!int.TryParse(onChat.Message, out int input))
      {
        onChat.Sender.SendServerMessage($"{onChat.Message} n'est pas une entrée valide.");
        return;
      }

      onChat.Sender.LoginCreature.GetLocalVariable<string>("_PLAYER_INPUT").Value = onChat.Message;
      onChat.Sender.LoginCreature.GetLocalVariable<int>("_AWAITING_PLAYER_INPUT").Delete();
      onChat.Sender.OnPlayerChat -= HandlePlayerInputInt;
    }
    public static void HandlePlayerInputString(ModuleEvents.OnPlayerChat onChat)
    {
      onChat.Volume = TalkVolume.SilentTalk;

      onChat.Sender.LoginCreature.GetLocalVariable<string>("_PLAYER_INPUT").Value = onChat.Message;
      onChat.Sender.LoginCreature.GetLocalVariable<int>("_AWAITING_PLAYER_INPUT").Delete();
      onChat.Sender.OnPlayerChat -= HandlePlayerInputString;
    }
  }
}
