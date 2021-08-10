using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord.Commands;
using Anvil.API;
using NWN.Core;

namespace NWN.Systems
{
  public static partial class BotSystem
  {
    public static async Task ExecuteSendMailCommand(SocketCommandContext context, string senderName, int characterId, string title, string content)
    {
      await NwTask.SwitchToMainThread();
      int result = DiscordUtils.CheckPlayerCredentialsFromDiscord(context, senderName);

      if (result <= 0)
      {
        await context.Channel.SendMessageAsync("Le personnage indiqué n'existe pas ou n'a pas été enregistré avec votre code Discord et votre clef cd.");
        return;
      }

      var queryResult = SqLiteUtils.SelectQuery("playerCharacters",
        new List<string>() { { "characterName" } },
        new List<string[]>() { new string[] { "ROWID", result.ToString() } });

      if (queryResult.Result == null)
      {
        await context.Channel.SendMessageAsync("Impossible de trouver une correspondance pour le nom du personnage indiqué.");
        return;
      }

      string senderFullName = queryResult.Result.GetString(0);
      Utils.SendMailToPC(characterId, senderFullName, title, content);

      await context.Channel.SendMessageAsync("Courrier en cours d'envoi.");
    }
  }
}
