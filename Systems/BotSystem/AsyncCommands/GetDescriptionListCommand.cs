using Discord.Commands;
using Microsoft.Data.Sqlite;

namespace NWN.Systems
{
  public static partial class BotSystem
  {
    public static string ExecuteGetDescriptionListCommand(SocketCommandContext context, string pcName)
    {
      int pcID = Utils.CheckPlayerCredentialsFromDiscord(context, pcName);
      if (pcID == 0)
        return "Le personnage indiqué n'existe pas ou n'a pas été enregistré avec votre code Discord et votre clef cd.";

      using (var connection = new SqliteConnection($"{ModuleSystem.db_path}"))
      {
        connection.Open();

        var command = connection.CreateCommand();
        command.CommandText =
        @"
        SELECT descriptionName
        FROM playerDescriptions
        WHERE characterId = $characterId
        ";
        command.Parameters.AddWithValue("$characterId", pcID);

        string result = "";

        using (var reader = command.ExecuteReader())
        {
          while (reader.Read())
          {
            result += "-" + reader.GetString(0) + "\n";
          }
        }
        return $"Voici la liste des descriptions enregistrées pour {pcName} :\n{result}";
      }
    }
  }
}
