using System.Collections.Generic;
using System.Threading.Tasks;
using Discord.Commands;
using Anvil.API;
using NWN.Core;

namespace NWN.Systems
{
  public static partial class BotSystem
  {
    public static async Task ExecuteDeleteRumorCommand(SocketCommandContext context, string rumorId)
    {
      int accountId = await DiscordUtils.GetPlayerAccountIdFromDiscord(context.User.Id);

      if (accountId < 0)
      {
        await context.Channel.SendMessageAsync("Votre compte Discord ne semble pas enregistré avec votre compte NwN. Veuillez suivre la procédure de la commande !register.");
        return;
      }

      string rank = await DiscordUtils.GetPlayerStaffRankFromDiscord(context.User.Id);

      switch (rank)
      {
        default:

          SqLiteUtils.DeletionQuery("rumors",
            new Dictionary<string, string>() { { "accountId", accountId.ToString() }, { "ROWID", rumorId } });

          await context.Channel.SendMessageAsync($"Votre rumeur numéro {rumorId} a bien été supprimée.");

          return;

        case "admin":
        case "staff":

          SqLiteUtils.DeletionQuery("rumors",
            new Dictionary<string, string>() { { "ROWID", rumorId } });

          await context.Channel.SendMessageAsync($"La rumeur numéro {rumorId} a bien été supprimée.");

          return;
      }
    }
  }
}
