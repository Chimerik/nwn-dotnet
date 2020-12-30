using System;
using System.Collections.Generic;

namespace NWN.Systems
{
  public static partial class BotSystem
  {
    public static List<string> ExecuteHelpCommand()
    {
      var msgList = new List<string>();
      var msg = "";

      foreach (var command in Bot.GetCommands())
      {
        var line = $"**{Bot.prefix}{command.Name}**";
        
        foreach (var param in command.Parameters)
        {
          line += $" <{param.Name}: {param.Type.Name}>";
        }

        line += $" : {command.Summary}\n";

        // Limit de 2000 chars par message sur discord
        if (msg.Length + line.Length > 2000)
        {
          msgList.Add(msg);
          msg = "";
        } else
        {
          msg += line;
        }
      }

      if (msg != "")
      {
        msgList.Add(msg);
      }

      return msgList;
    }
  }
}
