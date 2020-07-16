using System;
using System.Linq;
using NWN.NWNX;

namespace NWN.Systems
{
  public static partial class CommandSystem
  {
    private const string PREFIX = "!";

    public static void Init()
    {
      ChatSystem.OnChat += HandleChat;
    }

    private static void HandleChat(object sender, ChatSystem.ChatEventArgs e)
    {
      if (e.msg.Length <= PREFIX.Length) return;
      if (!e.msg.StartsWith(PREFIX)) return;
      if (!NWScript.GetIsPC(e.oSender)) return;

      Chat.SkipMessage();

      string[] args = e.msg.Split(' ');

      string commandName = args.FirstOrDefault().Substring(PREFIX.Length);
      args = args.Skip(1).ToArray();

      Command command;
      if (!commandDic.TryGetValue(commandName, out command))
      {
        NWScript.SendMessageToPC(e.oSender,
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
        NWScript.SendMessageToPC(e.oSender, msg);
        return;
      }

      try
      {
        command.execute(e, optionsResult);
      }
      catch (Exception err)
      {
        NWScript.SendMessageToPC(e.oSender, $"\nUnable to process command: {err.Message}");
      }
    }
  }
}
