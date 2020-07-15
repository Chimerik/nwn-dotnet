using System;
using System.Collections.Generic;
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

    public static event EventHandler<ChatEventArgs> OnChat = delegate { };
    public class ChatEventArgs : EventArgs
    {
      public string msg { get; }
      public NWObject oSender { get; }
      public NWObject oTarget { get; }
      public ChatChannel channel { get; }

      public ChatEventArgs(string msg, NWObject oSender, NWObject oTarget, ChatChannel channel)
      {
        this.msg = msg;
        this.oSender = oSender;
        this.oTarget = oTarget;
        this.channel = channel;
      }
    }

    private static int HandleChat (uint oidself)
    {
      string msg = Chat.GetMessage();
      var oSender = Chat.GetSender();
      var oTarget = Chat.GetTarget();
      var channel = Chat.GetChannel();

      OnChat(null, new ChatEventArgs(msg, oSender, oTarget, channel));

      return Entrypoints.SCRIPT_HANDLED;
    }
  }
}
