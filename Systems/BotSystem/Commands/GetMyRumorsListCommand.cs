using System.Collections.Generic;
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

          var query = SqLiteUtils.SelectQuery("rumors",
            new List<string>() { { "title" }, { "rowid" } },
            new List<string[]>() { new string[] { "accountId", accountId.ToString() } });

          string result = "";
          if(result != null)
          foreach (var rumors in query)
            result += rumors.GetString(1) + " - " + rumors.GetString(0) + "\n";

          await context.Channel.SendMessageAsync($"Voici la liste des vos rumeurs actuellement en vogue :\n{result}");
          return;
        case "admin":
        case "staff":

          var staffQuery = NwModule.Instance.PrepareCampaignSQLQuery(Config.database, "SELECT title, r.rowid, accountName from rumors r " +
        "LEFT JOIN PlayerAccounts pa on r.accountId = pa.ROWID ");
          string staffResult = "";

          foreach (var rumor in staffQuery.Results)
            staffResult += rumor.GetString(1) + " - " + rumor.GetString(2) + " - " + rumor.GetString(0) + "\n";

          await context.Channel.SendMessageAsync($"Voici la liste des rumeurs actuellement en vogue :\n{staffResult}");
          return;
      }
    }
  }
}
