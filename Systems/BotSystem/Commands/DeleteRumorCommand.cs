using System.Collections.Generic;
using System.Threading.Tasks;
using Discord.Commands;
using NWN.API;
using NWN.Core;

namespace NWN.Systems
{
  public static partial class BotSystem
  {
    public static async Task ExecuteDeleteRumorCommand(SocketCommandContext context, string rumorId)
    {
      await NwTask.SwitchToMainThread();

      int accountId = DiscordUtils.GetPlayerAccountIdFromDiscord(context.User.Id);

      if (accountId < 0)
      {
        await context.Channel.SendMessageAsync("Votre compte Discord ne semble pas enregistré avec votre compte NwN. Veuillez suivre la procédure de la commande !register.");
        return;
      }

      switch (DiscordUtils.GetPlayerStaffRankFromDiscord(context.User.Id))
      {
        default:

          if (SqLiteUtils.DeletionQuery("rumors",
            new Dictionary<string, string>() { { "accountId", accountId.ToString() }, { "ROWID", rumorId } }))
            await context.Channel.SendMessageAsync($"Votre rumeur numéro {rumorId} a bien été supprimée.");
          else
            await context.Channel.SendMessageAsync($"Vous n'avez pas enregistré de rumeur numéro {rumorId} avec ce compte.");

          return;

        case "admin":
        case "staff":

          if (SqLiteUtils.DeletionQuery("rumors",
            new Dictionary<string, string>() { { "ROWID", rumorId } }))
            await context.Channel.SendMessageAsync($"La rumeur numéro {rumorId} a bien été supprimée.");
          else
            await context.Channel.SendMessageAsync($"La rumeur {rumorId} n'a pas pu être trouvée.");

          return;
      }
    }
  }
}
