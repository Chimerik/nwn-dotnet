using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;

namespace NWN.Systems
{
  class DiscordUtils
  {
    public static async Task<int> CheckPlayerCredentialsFromDiscord(ulong userId, string sPCName)
    {
      using (var connection = new SqliteConnection(Config.dbPath))
      {
        connection.Open();

        var command = connection.CreateCommand();
        command.CommandText = "SELECT pc.ROWID from PlayerAccounts " +
        $"LEFT join playerCharacters pc on pc.accountId = PlayerAccounts.ROWID WHERE discordId = @discordId and pc.characterName like '{sPCName}%' ";

        command.Parameters.AddWithValue("@discordId", userId.ToString());

        using (var reader = await command.ExecuteReaderAsync())
        {
          while (reader.Read())
            return reader.GetInt32(0);
        }
      }

      return 0;
    }
    public static async Task<int> GetPlayerAccountIdFromDiscord(ulong UserId)
    {
      var result = await SqLiteUtils.SelectQueryAsync("PlayerAccounts",
        new List<string>() { { "ROWID" } },
        new List<string[]>() { new string[] { "discordId", UserId.ToString() } });

      if (result != null && result.Count > 0 && int.TryParse(result[0][0], out int accountId))
        return accountId;

      return -1;
    }
  }
}
