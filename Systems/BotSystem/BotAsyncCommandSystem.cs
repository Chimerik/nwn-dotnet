﻿using System;
using NWN.Core;

namespace NWN.Systems
{
  public static partial class BotAsyncCommandSystem
  {
    public static void ProcessBotAsyncCommand(string command)
    {
      BotAsyncCommand asyncCommand;
      if (!BotSystem.commandDic.TryGetValue(command, out asyncCommand))
        return;

      try
      {
        asyncCommand.execute(command);
      }
      catch (Exception err)
      {
        Utils.LogMessageToDMs($"\nAsync command failed: {err.Message}");
      }
    }
  }
}