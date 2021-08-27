using System.Collections.Generic;
using System.Threading.Tasks;
using Discord.Commands;
using Anvil.API;

namespace NWN.Systems
{
  public static partial class BotSystem
  {
    public static async Task ExecuteGetMyMailsListCommand(SocketCommandContext context, string characterName)
    {
      int result = await DiscordUtils.CheckPlayerCredentialsFromDiscord(context, characterName);

      if (result <= 0)
      {
        await context.Channel.SendMessageAsync("Le personnage indiqué n'existe pas ou n'a pas été enregistré avec votre code Discord et votre clef cd.");
        return;
      }

      var query = await SqLiteUtils.SelectQueryAsync("messenger",
        new List<string>() { { "rowid" }, { "senderName" }, { "title" }, { "sentDate" }, { "read" } },
        new List<string[]>() { new string[] { "characterId", result.ToString() } },
        " order by sentDate desc");

      string messageList = $"Liste des messages reçus par {characterName} :\n\n";
      
      foreach(var mail in query)
      {
        if (mail[4] == "1")
          messageList += "Lu | ";
        messageList += $"{mail[0]} | {mail[1]} | {mail[2]} | {mail[3]}";
      }

      await context.Channel.SendMessageAsync(messageList);
    }
  }
}
