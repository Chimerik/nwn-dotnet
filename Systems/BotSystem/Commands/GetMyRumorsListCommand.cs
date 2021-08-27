using System.Collections.Generic;
using System.Threading.Tasks;
using Discord.Commands;
using Anvil.API;
using NWN.Core;
using Microsoft.Data.Sqlite;

namespace NWN.Systems
{
  public static partial class BotSystem
  {
    public static async Task ExecuteGetMyRumorsListCommand(SocketCommandContext context)
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

          var query = await SqLiteUtils.SelectQueryAsync("rumors",
            new List<string>() { { "title" }, { "rowid" } },
            new List<string[]>() { new string[] { "accountId", accountId.ToString() } });

          string result = "";

          if (query != null)
            foreach (var rumors in query)
              result += rumors[1] + " - " + rumors[0] + "\n";

          await context.Channel.SendMessageAsync($"Voici la liste de vos rumeurs actuellement en vogue :\n{result}");
          return;
        case "admin":
        case "staff":

          using (var connection = new SqliteConnection(Config.dbPath))
          {
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = "SELECT title, r.rowid, accountName from rumors r " +
                                  "LEFT JOIN PlayerAccounts pa on r.accountId = pa.ROWID ";

            string staffResult = "";

            using (var reader = await command.ExecuteReaderAsync())
            {
              while (reader.Read())
                staffResult = reader.GetString(1) + " - " + reader.GetString(2) + " - " + reader.GetString(0) + "\n";
            }

            await context.Channel.SendMessageAsync($"Voici la liste des rumeurs actuellement en vogue :\n{staffResult}");
          }

          return;
      }
    }
  }
}
