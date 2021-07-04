using System.Collections.Generic;
using System.Threading.Tasks;
using Discord.Commands;
using NWN.API;
using NWN.Core;

namespace NWN.Systems
{
  public static partial class BotSystem
  {
    public static async Task ExecuteGetMyMailsListCommand(SocketCommandContext context, string characterName)
    {
      await NwTask.SwitchToMainThread();
      int result = DiscordUtils.CheckPlayerCredentialsFromDiscord(context, characterName);

      if (result <= 0)
      {
        await context.Channel.SendMessageAsync("Le personnage indiqué n'existe pas ou n'a pas été enregistré avec votre code Discord et votre clef cd.");
        return;
      }

      var query = SqLiteUtils.SelectQuery("messenger",
        new List<string>() { { "rowid" }, { "senderName" }, { "title" }, { "sentDate" }, { "read" } },
        new List<string[]>() { new string[] { "characterId", result.ToString() } },
        " order by sentDate desc");

      string messageList = $"Liste des messages reçus par {characterName} :\n\n";
      
      foreach(var mail in query.Results)
      {
        if (mail.GetInt(4) == 1)
          messageList += "Lu | ";
        messageList += $"{mail.GetInt(0)} | {mail.GetString(1)} | {mail.GetString(2)} | {mail.GetString(3)}";
      }

      await context.Channel.SendMessageAsync(messageList);
    }
  }
}
