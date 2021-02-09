using Discord.Commands;
using Microsoft.Data.Sqlite;

namespace NWN.Systems
{
    class DiscordUtils
    {
        public static int CheckPlayerCredentialsFromDiscord(SocketCommandContext context, string sPCName)
        {
            using (var connection = new SqliteConnection($"{Config.db_path}"))
            {
                connection.Open();

                var command = connection.CreateCommand();
                command.CommandText =
                @"
        SELECT pc.ROWID
        FROM PlayerAccounts
        LEFT join playerCharacters pc on pc.accountId = PlayerAccounts.ROWID
        WHERE discordId = $discordId and pc.characterName = $characterName
        ";
                command.Parameters.AddWithValue("$discordId", context.User.Id);
                command.Parameters.AddWithValue("$characterName", sPCName);

                int result = 0;

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        result = reader.GetInt32(0);
                    }
                }

                if (result > 0)
                    return result;
            }

            return 0;
        }
        public static string GetPlayerStaffRankFromDiscord(ulong UserId)
        {
            using (var connection = new SqliteConnection($"{Config.db_path}"))
            {
                connection.Open();

                var command = connection.CreateCommand();
                command.CommandText =
                @"
        SELECT rank
        FROM PlayerAccounts
        WHERE discordId = $discordId
        ";
                command.Parameters.AddWithValue("$discordId", UserId);

                string result = "";

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        result = reader.GetString(0);
                    }
                }

                return result;
            }
        }
    }
}
