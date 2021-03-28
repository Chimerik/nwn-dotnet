using System.Threading.Tasks;
using Discord.Commands;
using NWN.API;
using NWN.Core;

namespace NWN.Systems
{
  public static partial class BotSystem
  {
    public static async Task ExecuteGetMyRumorsListCommand(SocketCommandContext context)
    {
      await NwTask.SwitchToMainThread();
      int accountId = DiscordUtils.GetPlayerAccountIdFromDiscord(context.User.Id);

      if (accountId < 0)
      {
        await context.Channel.SendMessageAsync("Votre compte Discord ne semble pas enregistré avec votre compte NwN. Veuillez suivre la procédure de la commande !register.");
        return;
      }

      switch(DiscordUtils.GetPlayerStaffRankFromDiscord(context.User.Id))
      {
        default:
          var query = NWScript.SqlPrepareQueryCampaign(Config.database, $"SELECT title, rowid from rumors where accountId = @accountId");
          NWScript.SqlBindInt(query, "@accountId", accountId);
          string result = "";

          while (NWScript.SqlStep(query) > 0)
            result += NWScript.SqlGetString(query, 1) + " - " + NWScript.SqlGetString(query, 0) + "\n";

          await context.Channel.SendMessageAsync($"Voici la liste des vos rumeurs actuellement en vogue :\n{result}");
          return;
        case "admin":
        case "staff":
          var staffQuery = NWScript.SqlPrepareQueryCampaign(Config.database, "SELECT title, rowid, accountName from rumors r " +
        "LEFT JOIN PlayerAccounts pa on r.accountId = pa.ROWID ");
          string staffResult = "";

          while (NWScript.SqlStep(staffQuery) > 0)
            staffResult += NWScript.SqlGetString(staffQuery, 1) + " - " + NWScript.SqlGetString(staffQuery, 2) + " - " + NWScript.SqlGetString(staffQuery, 0) + "\n";

          await context.Channel.SendMessageAsync($"Voici la liste des rumeurs actuellement en vogue :\n{staffResult}");
          return;
      }
    }
  }
}
