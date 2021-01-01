using System.Threading.Tasks;
using Discord.Commands;
using Microsoft.Data.Sqlite;

namespace NWN.Systems
{
  public static partial class BotSystem
  {
    public static async Task ExecuteGetDescriptionCommand(SocketCommandContext context, string pcName, string descriptionName)
    {
      int pcID = Utils.CheckPlayerCredentialsFromDiscord(context, pcName);
      if (pcID == 0)
      {
        await context.Channel.SendMessageAsync("Le personnage indiqué n'existe pas ou n'a pas été enregistré avec votre code Discord et votre clef cd.");
        return;
      }

      using (var connection = new SqliteConnection($"{ModuleSystem.db_path}"))
      {
        connection.Open();

        var command = connection.CreateCommand();
        command.CommandText =
        @"
        SELECT description
        FROM playerDescriptions
        WHERE characterId = $characterId AND descriptionName = $descriptionName
        ";
        command.Parameters.AddWithValue("$characterId", pcID);
        command.Parameters.AddWithValue("$descriptionName", descriptionName);

        string result = "";

        using (var reader = command.ExecuteReader())
        {
          while (reader.Read())
          {
            result = reader.GetString(0);
          }
        }

        await context.Channel.SendMessageAsync(result);
      }
    }
  }
}
