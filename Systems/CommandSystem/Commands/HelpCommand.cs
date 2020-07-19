using System.Collections.Generic;

namespace NWN.Systems
{
  public static partial class CommandSystem
  {
    private static void ExecuteHelpCommand(ChatSystem.Context ctx, Options.Result options)
    {
      var commandName = options.positional[0];

      if (commandName == null)
      {
        __ShowAllCommands(ctx);
      } else
      {
        __ShowSingleCommand(ctx, commandName);
      }
    }

    private static void __ShowAllCommands (ChatSystem.Context ctx)
    {
      var msg = "\nList of all available commands :\n";
      foreach (KeyValuePair<string, Command> entry in commandDic)
      {
        msg += $"\n{entry.Value.shortDesc}";
      }

      NWScript.SendMessageToPC(ctx.oSender, msg);
    }

    private static void __ShowSingleCommand (ChatSystem.Context ctx, string name)
    {
      Command command;
      string msg;
      if (!commandDic.TryGetValue(name, out command))
      {
        msg = $"Unknown command \"{name}\".";
      } else
      {
        msg = command.longDesc;
      }

      NWScript.SendMessageToPC(ctx.oSender, msg);
    }
  }
}
