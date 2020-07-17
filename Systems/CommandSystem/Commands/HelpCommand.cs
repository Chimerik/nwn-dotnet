﻿using System.Collections.Generic;

namespace NWN.Systems
{
  public static partial class CommandSystem
  {
    private static void ExecuteHelpCommand(ChatSystem.ChatEventArgs e)
    {
      var msg = "\nList of all available commands :\n";
      foreach (KeyValuePair<string, Command> entry in commandDic)
      {
        msg += $"\n{entry.Value.shortDesc}";
      }

      NWScript.SendMessageToPC(e.oSender, msg);
    }
  }
}