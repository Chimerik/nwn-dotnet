using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using NWN.API;
using NWN.Core;

namespace NWN.Systems
{
  public static partial class BotSystem
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

      var query = NWScript.SqlPrepareQueryCampaign(Config.database, $"INSERT INTO rumors (accountId, title, content) VALUES (@accountId, @title, @content)" +
          $"ON CONFLICT (accountId, title) DO UPDATE SET content = @content;");
      NWScript.SqlBindInt(query, "@accountId", accountId);
      NWScript.SqlBindString(query, "@title", titre_rumeur);
      NWScript.SqlBindString(query, "@content", contenu_rumeur);
      NWScript.SqlStep(query);

      await context.Channel.SendMessageAsync($"La rumeur {titre_rumeur} a bien été enregistrée parmis les rumeurs en cours.");

      switch(DiscordUtils.GetPlayerStaffRankFromDiscord(context.User.Id))
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
