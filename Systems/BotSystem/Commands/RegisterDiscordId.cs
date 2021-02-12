using System.Threading.Tasks;
using Discord.Commands;
using Microsoft.Data.Sqlite;

namespace NWN.Systems
{
  public static partial class BotSystem
  {
    public static async Task ExecuteRegisterDiscordId(SocketCommandContext context, string cdKey)
    {
      using (var connection = new SqliteConnection($"{Config.db_path}"))
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

      await context.Channel.SendMessageAsync("Voilà qui est fait. Enfin, pour tant soit peu que la clef fournie fusse valide !");
    }
  }
}
