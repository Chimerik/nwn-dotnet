using System.Threading.Tasks;
using Microsoft.Data.Sqlite;
using Discord.WebSocket;

namespace NWN.Systems
{
  public static partial class BotSystem
  {
    public static async Task ExecuteGetRumorsListCommand(SocketSlashCommand command)
    {
      using (var connection = new SqliteConnection(Config.dbPath))
      {
        connection.Open();

        var sqlCommand = connection.CreateCommand();
        sqlCommand.CommandText = "SELECT title, r.rowid, accountName from rumors r " +
                              "LEFT JOIN PlayerAccounts pa on r.accountId = pa.ROWID ";

        string staffResult = "";

        using (var reader = await sqlCommand.ExecuteReaderAsync())
        {
          while (reader.Read())
            staffResult = reader.GetString(1) + " - " + reader.GetString(2) + " - " + reader.GetString(0) + "\n";
        }

        await command.RespondAsync(staffResult, ephemeral: true);
      }
    }
  }
}
