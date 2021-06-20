using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using NLog;
using NWN.API;

namespace NWN.Systems
{
  public static partial class CommandSystem
  {
    private const string PREFIX = "!";
    public static readonly Logger Log = LogManager.GetCurrentClassLogger();

    public static void ProcessChatCommandMiddleware(ChatSystem.Context ctx, Action next)
    {
      if (ctx.msg.Length <= PREFIX.Length ||
              !ctx.msg.StartsWith(PREFIX)
        )
      {
        next();
        return;
      }

      ctx.onChat.Skip = true;

      var args = __SplitMessage(ctx.msg);

      string commandName = args.FirstOrDefault().Substring(PREFIX.Length);
      args = args.Skip(1).ToArray();

      Command command;
      if (!commandDic.TryGetValue(commandName, out command))
      {
        ctx.oSender.SendServerMessage(
       $"\nUnknown command \"{commandName}\".\n\n" +
      $"Type \"{PREFIX}help\" for a list of all available commands.", ColorConstants.Orange
        );
        return;
      }

      Options.Result optionsResult;
      try
      {
        optionsResult = command.options.Parse(args);
      }
      catch (Exception err)
      {
        var msg = $"\nInvalid options :\n" +
          err.Message + "\n\n" +
          $"Please type \"{PREFIX}help {commandName}\" to get a description of the command.";
        ctx.oSender.SendServerMessage(msg, ColorConstants.Red);
        return;
      }

      try
      {
        command.execute(ctx, optionsResult);
      }
      catch (Exception err)
      {
        ctx.oSender.SendServerMessage($"\nUnable to process command: {err.Message}", ColorConstants.Red);
      }
    }

    private static string[] __SplitMessage(string msg)
    {
      var rgx = new Regex(@"\\?.|^$");
      var matches = rgx.Matches(msg);
      var args = new List<string>() { "" };
      var isStartQuote = false;

      foreach (var match in matches)
      {
        var x = match.ToString();
        if (x == "\"")
        {
          isStartQuote = !isStartQuote;
        }
        else if (!isStartQuote && x == " ")
        {
          args.Add("");
        }
        else
        {
          args[args.Count - 1] += x.Replace("\\", "");
        }
      }

      return args.ToArray();
    }
  }
}
