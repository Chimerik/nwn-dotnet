

using NLog;

using NWN.Systems;

using System;
using System.Collections.Generic;

using static NWN.LogUtils;

namespace NWN
{
  public static class LogUtils
  {
    private static readonly Logger Log = LogManager.GetCurrentClassLogger();

    public enum LogType
    {
      Console,
      Combat,
      IllegalItems,
      Durability,
      Craft,
      Learnables,
      MateriaSpawn,
      AreaManagement,
      ModuleAdministration,
      PlayerConnections,
      EnduranceSystem
    }

    public static readonly Dictionary<LogType, Queue<string>> logPile = new();
    public static readonly Dictionary<LogType, Discord.Rest.RestThreadChannel> logChannelDictionary = new();    

    public static void LogMessage(string message, LogType type)
    {
      Log.Info(message);

      if(type != LogType.Console)
      {
        if (Config.env == Config.Env.Chim)
          type = LogType.Console;

        logPile[type].Enqueue(message);
      }
    }

    public static async void LogLoop()
    {
      try
      {
        foreach (var logType in logPile)
        {
          string message = "";

          while (logType.Value.Count > 0)
          {
            if (message.Length + logType.Value.Peek().Length > 2000)
              break;

            message += $"\n{logType.Value.Dequeue()}";
          }

          if (!string.IsNullOrEmpty(message))
            await logChannelDictionary[logType.Key].SendMessageAsync(message);
        }
      }
      catch(Exception e)
      {
        ModuleSystem.Log.Info($"{e.Message}\n{e.StackTrace}");
      }
    }
  }
}
