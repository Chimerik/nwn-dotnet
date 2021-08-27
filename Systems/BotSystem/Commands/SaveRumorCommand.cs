using System.Collections.Generic;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Anvil.API;
using NWN.Core;

namespace NWN.Systems
{
  public static partial class BotSystem
  {
    public static async Task ExecuteSaveRumorCommand(SocketCommandContext context, string titre_rumeur, string contenu_rumeur)
    {
      int accountId = await DiscordUtils.GetPlayerAccountIdFromDiscord(context.User.Id);
      
      if (accountId < 0)
      {
        await context.Channel.SendMessageAsync("Votre compte Discord ne semble pas enregistré avec votre compte NwN. Veuillez suivre la procédure de la commande !register.");
        return;
      }

      bool awaitedQuery = await SqLiteUtils.InsertQueryAsync("rumors",
          new List<string[]>() {
            new string[] { "accountId", accountId.ToString() },
            new string[] { "title", titre_rumeur },
            new string[] { "content", contenu_rumeur } },
          new List<string>() { "accountId", "title" },
          new List<string[]>() { new string[] { "content" } });

      if(awaitedQuery)
        await context.Channel.SendMessageAsync($"La rumeur {titre_rumeur} a bien été enregistrée parmi les rumeurs en cours.");
      else
        await context.Channel.SendMessageAsync($"Erreur technique - La rumeur {titre_rumeur} n'a pas pu être enregistrée !");

      string rank = await DiscordUtils.GetPlayerStaffRankFromDiscord(context.User.Id);

      switch (rank)
      {
        default:
          await (Bot._client.GetChannel(680072044364562532) as IMessageChannel).SendMessageAsync($"{Bot._client.GetGuild(680072044364562528).EveryoneRole.Mention} Création de la rumeur {titre_rumeur} à valider.");
          return;
        case "admin":
        case "staff":
          return;
      }
    }
  }
}
