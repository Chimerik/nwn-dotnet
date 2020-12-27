using Discord.Commands;
using Microsoft.Data.Sqlite;

namespace NWN.Systems
{
  public static partial class BotSystem
  {
    public static string ExecuteDeleteDescriptionCommand(SocketCommandContext context, string pcName, string descriptionName)
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
        DELETE FROM playerDescriptions
        WHERE characterId = $characterId AND descriptionName = $descriptionName
        ";
        command.Parameters.AddWithValue("$characterId", pcID);
        command.Parameters.AddWithValue("$descriptionName", descriptionName);
        command.ExecuteNonQuery();

        return $"Description {descriptionName} supprimée pour le personnage {pcName}";
      }
    }
  }
}
