using System.Collections.Generic;
using System.Threading.Tasks;
using Discord.WebSocket;

namespace NWN.Systems
{
  public static partial class BotSystem
  {
    public static async Task ExecuteGetMailCommand(SocketSlashCommand command, string mailId, string characterName)
    {
      int result = await DiscordUtils.CheckPlayerCredentialsFromDiscord(command.User.Id, characterName);

      if (result <= 0)
      {
        //await context.Channel.SendMessageAsync("Le personnage indiqué n'existe pas ou n'a pas été enregistré avec votre code Discord et votre clef cd.");
        return;
      }

      var query = await SqLiteUtils.SelectQueryAsync("messenger",
        new List<string>() { { "senderName" }, { "title" }, { "sentDate" }, { "message" } },
        new List<string[]>() { new string[] { "characterId", result.ToString() }, { new string[] { "ROWID", mailId } } });

      if(query == null || query.Count < 1)
      {
        //await context.Channel.SendMessageAsync($"Le personnage indiqué n'a pas reçu de message dont le numéro correspond à {mailId}.");
        return;
      }

      //await context.Channel.SendMessageAsync($"De {query[0][0]}");
      //await context.Channel.SendMessageAsync($"Envoyé le {query[0][2]} :");
      //await context.Channel.SendMessageAsync($"{query[0][1]}");
      //await context.Channel.SendMessageAsync($"{query[0][3]}");

      SqLiteUtils.UpdateQuery("messenger",
        new List<string[]>() { new string[] { "read", "1" } },
        new List<string[]>() { new string[] { "rowid", mailId } });
    }
  }
}
