using System;
using System.Collections.Generic;

namespace NWN.Systems
{
  public static partial class BotSystem
  {
    public static List<string> ExecuteHelpCommand()
    {
      var msgs = new List<string>();

      foreach (var command in Bot.GetCommands())
      {
        var msg = $"**{Bot.prefix}{command.Name}**";
        
        foreach (var param in command.Parameters)
        {
          msg += $" <{param.Type.Name}: {param.Name}>";
        }

        msg += $" : {command.Summary}\n";
        msgs.Add(msg);
      }

      return msgs;
    }
  }
}
