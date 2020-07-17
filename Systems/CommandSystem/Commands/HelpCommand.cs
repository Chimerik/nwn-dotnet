using System.Collections.Generic;

namespace NWN.Systems
{
  public static partial class CommandSystem
  {
    private static void ExecuteHelpCommand(ChatSystem.Context chatContext, Options.Result options)
    {
      var commandName = options.positional[0];

      if (commandName == null)
      {
        __ShowAllCommands(chatContext);
      } else
      {
        // TODO
      }
    }

    private static void __ShowAllCommands (ChatSystem.Context chatContext)
    {
      var msg = "\nList of all available commands :\n";
      foreach (KeyValuePair<string, Command> entry in commandDic)
      {
        msg += $"\n{entry.Value.shortDesc}";
      }

      NWScript.SendMessageToPC(chatContext.oSender, msg);
    }
  }
}
