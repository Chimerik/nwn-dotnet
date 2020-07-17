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
        CommandSystem.ProcessChatCommandMiddleware
      }
    );
  }
}
