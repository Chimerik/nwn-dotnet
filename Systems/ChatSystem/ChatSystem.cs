using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NLog;
using Anvil.API;
using Anvil.API.Events;
using Anvil.Services;
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
    //public static List<ChatLine> globalChat = new List<ChatLine>();

    public ChatSystem(ChatService customChatService)
    {
      NwModule.Instance.OnChatMessageSend += OnNWNXChatEvent;
      chatService = customChatService;
    }

    private static void OnNWNXChatEvent(OnChatMessageSend onChat)
    {
      if(onChat.ChatChannel == ChatChannel.ServerMessage)
        return;
      
      if (!(onChat.Sender is NwCreature oSender) || oSender.GetObjectVariable<LocalVariableString>("_AWAITING_PLAYER_INPUT").HasValue)
        return;

      if (!oSender.IsPlayerControlled || !PlayerSystem.Players.TryGetValue(oSender.ControllingPlayer.LoginCreature, out PlayerSystem.Player player))
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
        onChat: onChat,
        language: player.currentLanguage
      ));
    }

    public class Context
    {
      public string msg { get; }
      public NwPlayer oSender { get; }
      public NwPlayer oTarget { get; }
      public ChatChannel channel { get; }
      public OnChatMessageSend onChat { get; }
      public Learnable language { get; }
      public ChatLine chatLine { get; set; }

      public Context(string msg, NwPlayer oSender, NwPlayer oTarget, ChatChannel channel, OnChatMessageSend onChat, int language)
      {
        this.msg = msg;
        this.oSender = oSender;
        this.oTarget = oTarget;
        this.channel = channel;
        this.onChat = onChat;
        this.language = language > 0 ? SkillSystem.learnableDictionary[language] : null;
        chatLine = new ChatLine(oSender.ControlledCreature.PortraitResRef + "t", oSender.ControlledCreature.Name, oSender.PlayerName, msg, "", channel,
          msg.Trim().StartsWith("(") || channel == ChatChannel.PlayerParty ? ChatLine.ChatCategory.HorsRolePlay : ChatLine.ChatCategory.RolePlay,
          oTarget?.PlayerName, oTarget?.LoginCreature.PortraitResRef + "t");
      }
    }

    private static Pipeline<Context> pipeline = new Pipeline<Context>(
      new Action<Context, Action>[]
      {
            ChatSystem.ProcessWriteLogMiddleware,
            CommandSystem.ProcessChatCommandMiddleware,
            ChatSystem.ProcessMutePMMiddleware,
            ChatSystem.ProcessDMListenMiddleware,
            ChatSystem.ProcessGetChatReceiversMiddleware,
            ChatSystem.ProcessChatColorMiddleware,
      }
    );
    public static void ProcessWriteLogMiddleware(Context ctx, Action next)
    {
      if (ctx.oTarget == null)
      {
        string filename = string.Format("{0:yyyy-MM-dd}_{1}.txt", DateTime.Now, "chatlog");
        string path = Path.Combine(Environment.GetEnvironmentVariable("HOME") + "/ChatLog", filename);

        using StreamWriter file =
        new StreamWriter(path, true);
        file.WriteLineAsync(DateTime.Now.ToShortTimeString() + " - [" + ctx.channel + " - " + areaName + "] " + ctx.oSender.ControlledCreature.OriginalName + " : " + ctx.msg);
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
        file.WriteLineAsync(DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString() + " - " + ctx.oSender.LoginCreature.Name + " To : " + ctx.oSender.LoginCreature.OriginalName + " : " + ctx.msg); 
      }

      next();
    }
    public static void ProcessMutePMMiddleware(Context ctx, Action next)
    {
      if (ctx.oTarget != null && PlayerSystem.Players.TryGetValue(ctx.oTarget.LoginCreature, out PlayerSystem.Player targetPlayer) && PlayerSystem.Players.TryGetValue(ctx.oSender.LoginCreature, out PlayerSystem.Player player))
      {
        if (targetPlayer.mutedList.Count() > 0 && !ctx.oSender.IsDM && (targetPlayer.mutedList.Contains(player.accountId) || targetPlayer.mutedList.Contains(0)))
        {
          ctx.onChat.Skip = true;
          ctx.oSender.SendServerMessage($"{ctx.oTarget.LoginCreature.Name.ColorString(ColorConstants.White)} ne souhaite actuellement pas recevoir de mp.", ColorConstants.Red);
          return;
        }
      }

      next();
    }
    public static void HandlePM(Context ctx)
    {
      if (ctx.oTarget != null)
      {
        if (!chatReceivers.ContainsKey(ctx.oTarget))
          chatReceivers.Add(ctx.oTarget, ctx.msg);
      }
      else
      {
        ctx.onChat.Skip = true;
        ctx.oSender.SendServerMessage("La personne à laquelle vous tentez d'envoyer un message n'est pas connectée.", ColorConstants.Orange);
        return;
      }
    }
    public static void ProcessDMListenMiddleware(Context ctx, Action next)
    {
      //SYSTEME DE RECOPIE DE CHAT POUR LES DMS
      if (ctx.channel == ChatChannel.PlayerTalk || ctx.channel == ChatChannel.PlayerWhisper)
      {
        foreach (NwPlayer oDM in NwModule.Instance.Players.Where(d => d.IsDM || d.PlayerName == "Chim"))
        {
          if (PlayerSystem.Players.TryGetValue(oDM.LoginCreature, out PlayerSystem.Player dungeonMaster))
          {
            if (dungeonMaster.listened.Contains(ctx.oSender))
            {
              if (ctx.oSender.ControlledCreature.Area != oDM.ControlledCreature.Area || oDM.ControlledCreature.Distance(ctx.oSender.ControlledCreature) > chatService.GetPlayerChatHearingDistance(oDM, ctx.channel))
              {
                if (oDM.IsDM)
                  chatService.SendMessage(ChatChannel.PlayerDm, $"[COPIE - {areaName.ColorString(ColorConstants.Yellow)}] {ctx.msg.ColorString(ColorConstants.White)}".ColorString(ColorConstants.Orange), ctx.oSender.ControlledCreature, oDM);
                else
                  chatService.SendMessage(ChatChannel.PlayerParty, $"[COPIE - {areaName.ColorString(ColorConstants.Yellow)}] {ctx.msg.ColorString(ColorConstants.White)}".ColorString(ColorConstants.Orange), ctx.oSender.ControlledCreature, oDM);
              }
            }
          }
        }
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
      foreach (NwPlayer player in NwModule.Instance.Players.Where(p => p.ControlledCreature?.Area == ctx.oSender.ControlledCreature?.Area && p.ControlledCreature.Distance(ctx.oSender.ControlledCreature) < chatService.GetPlayerChatHearingDistance(p, ctx.channel)))
      {
        if (ctx.language != null)
        {
          ctx.chatLine.untranslatedText = Languages.GetLangueStringConvertedHRPProtection(ctx.msg, ctx.language.id);

          if ((PlayerSystem.Players.TryGetValue(player.LoginCreature, out PlayerSystem.Player listener) && listener.learnableSkills.ContainsKey(ctx.language.id)) || player.IsDM)
          {
            chatReceivers.Add(player, "[" + ctx.language.name + "] " + ctx.msg);
            player.SendServerMessage(ctx.oSender.ControlledCreature + " : [" + ctx.language.name + "] " + ctx.chatLine.untranslatedText);
          }
          else
            chatReceivers.Add(player, ctx.chatLine.untranslatedText);
        }
        else
          chatReceivers.Add(player, ctx.msg);
      }
    }
    public static void ProcessChatColorMiddleware(Context ctx, Action next)
    {
      NwModule.Instance.OnChatMessageSend -= OnNWNXChatEvent;

      ChatLine.ChatCategory chatCategory = ctx.msg.Trim().StartsWith("(") || ctx.channel == ChatChannel.PlayerParty ? ChatLine.ChatCategory.HorsRolePlay : ChatLine.ChatCategory.RolePlay;

      if (ctx.oTarget != null)
        chatCategory = ChatLine.ChatCategory.Private;

      foreach (KeyValuePair<NwPlayer, string> chatReceiver in chatReceivers)
      {
        ctx.onChat.Skip = true;

        if (!PlayerSystem.Players.TryGetValue(chatReceiver.Key.LoginCreature, out PlayerSystem.Player receiver))
        {
          chatService.SendMessage(ctx.channel, chatReceiver.Value, ctx.oSender.ControlledCreature, chatReceiver.Key);
          return;
        }

        if (receiver.readChatLines.Count > 150)
          receiver.readChatLines.RemoveAt(0);

        if (string.IsNullOrEmpty(ctx.chatLine.untranslatedText))
        {
          receiver.readChatLines.Add(ctx.chatLine);

          if (receiver.openedWindows.ContainsKey("chatReader") && ctx.chatLine.category != ChatLine.ChatCategory.Private)
            ((PlayerSystem.Player.ChatReaderWindow)receiver.windows["chatReader"]).InsertNewChatInWindow(ctx.chatLine);
        }
        else
        {
          ChatLine untranslatedChat = new ChatLine(ctx.chatLine.portrait, ctx.chatLine.name, ctx.chatLine.playerName, ctx.chatLine.untranslatedText, "", ctx.chatLine.channel, ctx.chatLine.category);

          receiver.readChatLines.Add(ctx.chatLine);

          if (receiver.openedWindows.ContainsKey("chatReader") && ctx.chatLine.category != ChatLine.ChatCategory.Private)
            ((PlayerSystem.Player.ChatReaderWindow)receiver.windows["chatReader"]).InsertNewChatInWindow(ctx.chatLine);
        }

        if (ctx.chatLine.category == ChatLine.ChatCategory.Private)
        {
          if (PlayerSystem.Players.TryGetValue(ctx.oSender.LoginCreature, out PlayerSystem.Player player))
          {
            if (player.readChatLines.Count > 150)
              player.readChatLines.RemoveAt(0);

            player.readChatLines.Add(ctx.chatLine);
          }

          if (receiver.openedWindows.ContainsKey(ctx.oSender.PlayerName))
            ((PlayerSystem.Player.PrivateMessageWindow)receiver.windows[ctx.oSender.PlayerName]).InsertNewChatInWindow(ctx.chatLine);
          else if (receiver.openedWindows.ContainsKey("chatReader"))
            ((PlayerSystem.Player.ChatReaderWindow)receiver.windows["chatReader"]).HandleNewPM(ctx.oSender.PlayerName);

          if (player.openedWindows.ContainsKey(ctx.oTarget.PlayerName))
            ((PlayerSystem.Player.PrivateMessageWindow)player.windows[ctx.oTarget.PlayerName]).InsertNewChatInWindow(ctx.chatLine);
        }

        string coloredChat = chatReceiver.Value;

        if (receiver.chatColors.ContainsKey((int)ctx.channel))
        {
          byte[] colorArray = receiver.chatColors[(int)ctx.channel];
          coloredChat = chatReceiver.Value.ColorString(new Color(colorArray[0], colorArray[1], colorArray[2], colorArray[3]));
        }

        if (receiver.chatColors.ContainsKey(100)) // 100 = emote
          coloredChat = HandleEmoteColoration(receiver, coloredChat);

        chatService.SendMessage(ctx.channel, coloredChat, ctx.oSender.ControlledCreature, chatReceiver.Key);
      }

      NwModule.Instance.OnChatMessageSend += OnNWNXChatEvent;

      next();
    }
    public static string HandleEmoteColoration(PlayerSystem.Player player, string chat)
    {
      int starCount = chat.ToCharArray().Count(c => c == '*');

      if (starCount == 1 && player.chatColors.ContainsKey(101)) // 101 = chat correctif
      {
        byte[] colorArray = player.chatColors[101];
        return chat.StripColors().ColorString(new Color(colorArray[0], colorArray[1], colorArray[2], colorArray[3]));
      }
      else if (starCount > 1)
      {
        string[] sArray = chat.Split('*', '*');
        string sColored = "";
        int i = 0;
        byte[] colorArray = player.chatColors[100];

        foreach (string s in sArray)
        {
          if (i % 2 == 0)
            sColored += s;
          else
          {
            sColored += $" * {s} * ".ColorString(new Color(colorArray[0], colorArray[1], colorArray[2], colorArray[3]));
          }
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

      onChat.Sender.LoginCreature.GetObjectVariable<LocalVariableString>("_PLAYER_INPUT").Value = onChat.Message;
      onChat.Sender.LoginCreature.GetObjectVariable<LocalVariableString>("_AWAITING_PLAYER_INPUT").Delete();
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

      onChat.Sender.LoginCreature.GetObjectVariable<LocalVariableString>("_PLAYER_INPUT").Value = onChat.Message;
      onChat.Sender.LoginCreature.GetObjectVariable<LocalVariableInt>("_AWAITING_PLAYER_INPUT").Delete();
      onChat.Sender.OnPlayerChat -= HandlePlayerInputInt;
    }
    public static void HandlePlayerInputString(ModuleEvents.OnPlayerChat onChat)
    {
      onChat.Volume = TalkVolume.SilentTalk;

      onChat.Sender.LoginCreature.GetObjectVariable<LocalVariableString>("_PLAYER_INPUT").Value = onChat.Message;
      onChat.Sender.LoginCreature.GetObjectVariable<LocalVariableInt>("_AWAITING_PLAYER_INPUT").Delete();
      onChat.Sender.OnPlayerChat -= HandlePlayerInputString;
    }
  }
}
