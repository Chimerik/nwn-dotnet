using Discord.Commands;
using Microsoft.Data.Sqlite;

namespace NWN.Systems
{
  public static partial class BotSystem
  {
    public static string ExecuteSaveDescriptionCommand(SocketCommandContext context, string pcName, string descriptionName, string descriptionText)
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
        INSERT INTO playerDescriptions
        (characterId, descriptionName, description)
        VALUES  ($characterId, $descriptionName, $description)
        ON CONFLICT(characterId, descriptionName) DO UPDATE SET description = $description;
    ";
        command.Parameters.AddWithValue("$characterId", pcID);
        command.Parameters.AddWithValue("$descriptionName", descriptionName);
        command.Parameters.AddWithValue("$description", descriptionText);
        command.ExecuteNonQuery();
      }

      return $"La description {descriptionName} a été enregistrée parmis les descriptions disponibles pour votre personnage {pcName}.";
    }
  }
}
