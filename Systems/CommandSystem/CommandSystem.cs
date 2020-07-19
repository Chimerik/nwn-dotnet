using System;
using System.Linq;
using NWN.NWNX;

namespace NWN.Systems
{
  public static partial class CommandSystem
  {
    private const string PREFIX = "!";

    public static void ProcessChatCommandMiddleware(ChatSystem.Context ctx, Action next)
    {
      if (ctx.msg.Length <= PREFIX.Length ||
        !ctx.msg.StartsWith(PREFIX) ||
        !NWScript.GetIsPC(ctx.oSender)
      )
      {
        next();
        return;
      }

      Chat.SkipMessage();

      string[] args = ctx.msg.Split(' ');

      string commandName = args.FirstOrDefault().Substring(PREFIX.Length);
      args = args.Skip(1).ToArray();

      Command command;
      if (!commandDic.TryGetValue(commandName, out command))
      {
        NWScript.SendMessageToPC(ctx.oSender,
       $"\nUnknown command \"{commandName}\".\n\n" +
      $"Type \"{PREFIX}help\" for a list of all available commands."
        );
        return;
      }

      Options.Result optionsResult;
      try
      {
        optionsResult = command.options.Parse(args);
      } catch (Exception err)
      {
        var msg = $"\nInvalid options :\n" +
          err.Message + "\n\n" +
          $"Please type \"{PREFIX}help {commandName}\" to get a description of the command.";
        NWScript.SendMessageToPC(ctx.oSender, msg);
        return;
      }

      try
      {
        command.execute(ctx, optionsResult);
      }
      catch (Exception err)
      {
        NWScript.SendMessageToPC(ctx.oSender, $"\nUnable to process command: {err.Message}");
      }
    }
  }
}
