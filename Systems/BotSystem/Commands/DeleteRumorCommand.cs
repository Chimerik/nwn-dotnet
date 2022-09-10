using System.Collections.Generic;
using System.Threading.Tasks;
using Discord.WebSocket;
using System.Linq;

namespace NWN.Systems
{
  public static partial class BotSystem
  {
    public static async Task ExecuteDeleteRumorCommand(SocketSlashCommand command)
    {
      SqLiteUtils.DeletionQuery("rumors",
        new Dictionary<string, string>() { { "ROWID", command.Data.Options.First().Value.ToString() } });

      await command.RespondAsync($"La rumeur numéro {command.Data.Options.First().Value} a bien été supprimée.", ephemeral: true);

    }
    public static async Task ExecuteDeleteMyRumorCommand(SocketSlashCommand command)
    {
      int accountId = await DiscordUtils.GetPlayerAccountIdFromDiscord(command.User.Id);

      if (accountId < 0)
      {
        await command.RespondAsync("Votre compte Discord ne semble pas enregistré avec votre compte NwN. Veuillez suivre la procédure de la commande /register.", ephemeral: true);
        return;
      }

      SqLiteUtils.DeletionQuery("rumors",
            new Dictionary<string, string>() { { "accountId", accountId.ToString() }, { "ROWID", command.Data.Options.First().Value.ToString() } });

      await command.RespondAsync($"Votre rumeur numéro {command.Data.Options.First().Value} a bien été supprimée.", ephemeral: true);
    }
  }
}
