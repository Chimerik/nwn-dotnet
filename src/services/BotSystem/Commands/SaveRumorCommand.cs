using System.Collections.Generic;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Anvil.API;
using Utils;

namespace BotSystem
{
  public static partial class BotCommand
    {
    public static async Task ExecuteSaveRumorCommand(SocketCommandContext context, string titre_rumeur, string contenu_rumeur)
    {
      await NwTask.SwitchToMainThread();
      int accountId = DiscordUtils.GetPlayerAccountIdFromDiscord(context.User.Id);
      
      if (accountId < 0)
      {
        await context.Channel.SendMessageAsync("Votre compte Discord ne semble pas enregistré avec votre compte NwN. Veuillez suivre la procédure de la commande !register.");
        return;
      }

      SqLiteUtils.InsertQuery("rumors",
          new List<string[]>() {
            new string[] { "accountId", accountId.ToString() },
            new string[] { "title", titre_rumeur },
            new string[] { "content", contenu_rumeur } },
          new List<string>() { "accountId", "title" },
          new List<string[]>() { new string[] { "content" } });

      await context.Channel.SendMessageAsync($"La rumeur {titre_rumeur} a bien été enregistrée parmis les rumeurs en cours.");

      switch(DiscordUtils.GetPlayerStaffRankFromDiscord(context.User.Id))
      {
        default:
          await (DiscordUtils._client.GetChannel(680072044364562532) as IMessageChannel).SendMessageAsync($"{Bot._client.GetGuild(680072044364562528).EveryoneRole.Mention} Création de la rumeur {titre_rumeur} à valider.");
          return;
        case "admin":
        case "staff":
          return;
      }
    }
  }
}
