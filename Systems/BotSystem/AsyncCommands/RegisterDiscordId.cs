using Discord.Commands;
using Microsoft.Data.Sqlite;

namespace NWN.Systems
{
  public static partial class BotSystem
  {
    public static string ExecuteRegisterDiscordId(SocketCommandContext context, string cdKey)
    {
      using (var connection = new SqliteConnection($"{ModuleSystem.db_path}"))
      {
        connection.Open();

        var command = connection.CreateCommand();
        command.CommandText =
        @"
        UPDATE PlayerAccounts
        SET discordId = $discordId
        WHERE cdKey = $cdKey
    ";
        command.Parameters.AddWithValue("$cdKey", cdKey);
        command.Parameters.AddWithValue("$discordId", context.User.Id);
        command.ExecuteNonQuery();
      }

      return "Voilà qui est fait. Enfin, pour tant soit peu que la clef fournie fusse valide !";
    }
  }
}
