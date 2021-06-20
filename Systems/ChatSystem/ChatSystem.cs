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
    public static ChatService chatService { get; set; }
    private static string areaName = "";
    private static Dictionary<NwPlayer, string> chatReceivers = new Dictionary<NwPlayer, string>();
    public ChatSystem(ChatService customChatService)
    {
      NwModule.Instance.OnChatMessageSend += OnNWNXChatEvent;
      chatService = customChatService;
    }

    private void OnNWNXChatEvent(OnChatMessageSend onChat)
    {
      if(onChat.ChatChannel == ChatChannel.ServerMessage)
        return;
      
      if (!(onChat.Sender is NwCreature oSender) || oSender.GetLocalVariable<string>("_AWAITING_PLAYER_INPUT").HasValue)
        return;

      if (oSender.Area != null)
        areaName = oSender.Area.Name;
      else
        areaName = "Entre deux zones";

      chatReceivers.Clear();

      pipeline.Execute(new Context(
        msg: onChat.Message,
        oSender: oSender.ControllingPlayer,
        oTarget: onChat.Target,
        channel: onChat.ChatChannel,
        onChat: onChat
      ));
    }

    public class Context
    {
      public string msg { get; }
      public NwPlayer oSender { get; }
      public NwPlayer oTarget { get; }
      public ChatChannel channel { get; }
      public OnChatMessageSend onChat { get; }

      public Context(string msg, NwPlayer oSender, NwPlayer oTarget, ChatChannel channel, OnChatMessageSend onChat)
      {
        this.msg = msg;
        this.oSender = oSender;
        this.oTarget = oTarget;
        this.channel = channel;
        this.onChat = onChat;
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

        using StreamWriter file =
        new StreamWriter(path, true);
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

        using StreamWriter file =
        new StreamWriter(path, true);
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
            ctx.onChat.Skip = true;
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
        if (!chatReceivers.ContainsKey(ctx.oTarget))
          chatReceivers.Add(ctx.oTarget, ctx.msg);
          //ChatPlugin.SendMessage(ctx.channel, ctx.msg, ctx.oSender, ctx.oTarget);
        ///return;
      }
      else
      {
        ctx.onChat.Skip = true;
        ctx.oSender.SendServerMessage("La personne à laquelle vous tentez d'envoyer un message n'est plus connectée.", ColorConstants.Orange);
        return;
      }
    }
    public static void ProcessAFKDetectionMiddleware(Context ctx, Action next)
    {
      if (PlayerSystem.Players.TryGetValue(ctx.oSender.LoginCreature, out PlayerSystem.Player player))
      {
        if (player.isAFK)
          if (ctx.channel == ChatChannel.PlayerTalk || ctx.channel == ChatChannel.PlayerWhisper)
            if (!ctx.msg.Contains("(") && !ctx.msg.Contains(")"))
              if (ctx.oSender.ControlledCreature.GetNearestCreatures(CreatureTypeFilter.PlayerChar(true)).Any(p => p.LoginPlayer != ctx.oSender && p.Distance(ctx.oSender.ControlledCreature) < chatService.GetPlayerChatHearingDistance(p.ControllingPlayer, ctx.channel)))
                player.isAFK = false;
      }

      next();
    }
    public static void ProcessDMListenMiddleware(Context ctx, Action next)
    {
      //SYSTEME DE RECOPIE DE CHAT POUR LES DMS
      if (ctx.channel == ChatChannel.PlayerTalk || ctx.channel == ChatChannel.PlayerWhisper)
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
                if (ctx.oSender.ControlledCreature.Area != oDM.ControlledCreature.Area || oDM.ControlledCreature.Distance(ctx.oSender.ControlledCreature) > chatService.GetPlayerChatHearingDistance(oDM, ctx.channel))
                {
                  oInviSender.Name = ctx.oSender.ControlledCreature.Name;
                  chatService.SendMessage(ChatChannel.PlayerTell, "[COPIE - " + areaName + "] " + ctx.msg, oInviSender, oDM);
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
        case ChatChannel.PlayerTalk:
        case ChatChannel.PlayerWhisper:
        case ChatChannel.DmTalk:
        case ChatChannel.DmWhisper:
          HandleLanguage(ctx);
          break;
        case ChatChannel.PlayerParty:
        case ChatChannel.DmParty:
          foreach (NwPlayer oPartyMember in ctx.oSender.PartyMembers)
            if (!chatReceivers.ContainsKey(oPartyMember))
              chatReceivers.Add(oPartyMember, ctx.msg);
          break;
        case ChatChannel.PlayerTell:
        case ChatChannel.DmTell:
          HandlePM(ctx);
          break;
        case ChatChannel.PlayerDm:
        case ChatChannel.DmDm:
          foreach (NwPlayer oDM in NwModule.Instance.Players.Where(p => p.IsDM))
            if (!chatReceivers.ContainsKey(oDM))
              chatReceivers.Add(oDM, ctx.msg);
          break;
        case ChatChannel.DmShout:
          foreach (NwPlayer oPC in NwModule.Instance.Players)
            if (!chatReceivers.ContainsKey(oPC))
              chatReceivers.Add(oPC, ctx.msg);
          break;
      }
      next();
    }
    public static void HandleLanguage(Context ctx)
    {
      foreach (NwPlayer player in NwModule.Instance.Players.Where(p => p.ControlledCreature.Area == ctx.oSender.ControlledCreature.Area && p.ControlledCreature.Distance(ctx.oSender.ControlledCreature) < chatService.GetPlayerChatHearingDistance(p, ctx.channel)))
      {
        Feat language = (Feat)ctx.oSender.ControlledCreature.GetLocalVariable<int>("_ACTIVE_LANGUAGE").Value;

        //string sName = NWScript.GetLocalString(ctx.oSender, "__DISGUISE_NAME");
        //if (sName == "") sName = NWScript.GetName(ctx.oSender);

        if (language != (Feat)CustomFeats.Invalid)
        {
          string sLanguageName = SkillSystem.customFeatsDictionnary[language].name;
          if (player.LoginCreature.KnowsFeat(language) || player.IsDM)
          {
            chatReceivers.Add(player, "[" + sLanguageName + "] " + ctx.msg);
            player.SendServerMessage(ctx.oSender.ControlledCreature + " : [" + sLanguageName + "] " + Languages.GetLangueStringConvertedHRPProtection(ctx.msg, language));
          }
          else
            chatReceivers.Add(player, Languages.GetLangueStringConvertedHRPProtection(ctx.msg, language));
        }
        else
          chatReceivers.Add(player, ctx.msg);
      }
    }
    public static void ProcessChatColorMiddleware(Context ctx, Action next)
    {
      foreach (KeyValuePair<NwPlayer, string> chatReceiver in chatReceivers)
      {
        ctx.onChat.Skip = true;

        if (!PlayerSystem.Players.TryGetValue(chatReceiver.Key.LoginCreature, out PlayerSystem.Player player))
        {
          chatService.SendMessage(ctx.channel, chatReceiver.Value, ctx.oSender.ControlledCreature, chatReceiver.Key);
          return;
        }
          
        string coloredChat = chatReceiver.Value;

        if (player.chatColors.ContainsKey(ctx.channel))
          coloredChat = chatReceiver.Value.ColorString(player.chatColors[ctx.channel]);

        if (player.chatColors.ContainsKey((ChatChannel)100)) // 100 = emote
          coloredChat = HandleEmoteColoration(player, coloredChat);

        chatService.SendMessage(ctx.channel, coloredChat, ctx.oSender.ControlledCreature, chatReceiver.Key);
      }
      next();
    }
    public static string HandleEmoteColoration(PlayerSystem.Player player, string chat)
    {
      int starCount = chat.ToCharArray().Count(c => c == '*'); 

      if (starCount == 1 && player.chatColors.ContainsKey((ChatChannel)101)) // 101 = chat correctif
          return chat.StripColors().ColorString(player.chatColors[(ChatChannel)101]);
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
            sColored += $" * {s} * ".ColorString(player.chatColors[(ChatChannel)100]);
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
