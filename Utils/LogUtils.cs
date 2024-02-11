using Anvil.API;
using Discord;

using NLog;

using NWN.Systems;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
      EnduranceSystem,
      TradeSystem,
      ArenaSystem,
      PlayerSaveSystem,
      PersonalStorageSystem,
      PlayerDeath,
      DMAction,
      LootSystem,
      Traps
    }

    public static readonly Dictionary<LogType, Queue<string>> logPile = new();
    public static readonly Dictionary<LogType, Discord.Rest.RestThreadChannel> logChannelDictionary = new();
    public static readonly Dictionary<string, Queue<string>> chatLogPile = new();
    public static readonly Dictionary<string, Discord.Rest.RestThreadChannel> chatLogChannelDictionary = new();

    public static async void LogMessage(string message, LogType type)
    {
      Log.Info(message);

      if(type != LogType.Console)
      {
        if (Config.env == Config.Env.Chim)
          type = LogType.Console;

        await NwTask.WaitUntil(() => logPile.ContainsKey(type));
        logPile[type].Enqueue(message);
      }
    }
    public static void LogChatMessage(string areaName, string message)
    {
      if(!chatLogPile.TryAdd(areaName, new()))
        chatLogPile[areaName].Enqueue(message);
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

        ChatLogLoop();
      }
      catch(Exception e)
      {
        ModuleSystem.Log.Info($"{e.Message}\n{e.StackTrace}");
      }
    }
    public static async void ChatLogLoop()
    {
      foreach (var chat in chatLogPile)
      {
        string message = "";

        while (chat.Value.Count > 0)
        {
          if(chat.Value.Peek().Length > 2000)
          {
            message = chat.Value.Dequeue();

            while (message.Length > 2000)
            {
              string splitMessage = message[..1999];
              message = message[2000..];
              SendChatLogToDiscord(chat.Key, splitMessage);
              await Task.Delay(5);
            }

            SendChatLogToDiscord(chat.Key, message);

            return;
          }

          if (message.Length + chat.Value.Peek().Length > 2000)
            break;

          message += $"\n{chat.Value.Dequeue()}";
        }

        if (!string.IsNullOrEmpty(message))
          SendChatLogToDiscord(chat.Key, message);
      }
    }
    public static async void SendChatLogToDiscord(string areaName, string message)
    {
      try
      {
        if(Config.env != Config.Env.Prod)
        {
          await logChannelDictionary[LogType.Console].SendMessageAsync(message);
          return;
        }
        if (areaName.StartsWith("Introduction - La galère"))
          areaName = "Introduction - La galère";

        if (!chatLogChannelDictionary.ContainsKey(areaName))
        {
          var activeThreads = await Bot.chatLogForum.GetActiveThreadsAsync();
          var publicArchivedThreads = await Bot.chatLogForum.GetPublicArchivedThreadsAsync();

          var post = activeThreads.FirstOrDefault(t => t.Name == areaName);
          post ??= publicArchivedThreads.FirstOrDefault(t => t.Name == areaName);
          post ??= await Bot.chatLogForum.CreatePostAsync(areaName, ThreadArchiveDuration.OneWeek, null, "Système de logs du module");

          chatLogChannelDictionary.Add(areaName, post);
        }

        await chatLogChannelDictionary[areaName].SendMessageAsync(message);
      }
      catch (Exception e)
      {
        ModuleSystem.Log.Info($"{e.Message}\n{e.StackTrace}");
      }
    }
  }
}
