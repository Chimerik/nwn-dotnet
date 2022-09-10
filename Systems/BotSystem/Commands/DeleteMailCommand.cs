using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord.WebSocket;

namespace NWN.Systems
{
  public static partial class BotSystem
  {
    public static async Task ExecuteDeleteMailCommand(SocketSlashCommand command)
    {
      int result = await DiscordUtils.CheckPlayerCredentialsFromDiscord(command.User.Id, command.Data.Options.First().Value.ToString());

      if (result <= 0)
      {
        await command.RespondAsync("Le personnage indiqué n'existe pas ou n'a pas été enregistré avec votre code Discord et votre clef cd.", ephemeral: true);
        return;
      }

      SqLiteUtils.DeletionQuery("messenger",
        new Dictionary<string, string>() { { "characterId", result.ToString() }, { "ROWID", command.Data.Options.ElementAt(1).Value.ToString() } });

      await command.RespondAsync("Le personnage indiqué n'existe pas ou n'a pas été enregistré avec votre code Discord et votre clef cd.", ephemeral: true);
    }
  }
}
