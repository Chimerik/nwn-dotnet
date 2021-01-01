using System.Threading.Tasks;
using Discord.Commands;
using Microsoft.Data.Sqlite;

namespace NWN.Systems
{
  public static partial class BotSystem
  {
    public static async Task ExecuteSaveDescriptionCommand(SocketCommandContext context, string pcName, string descriptionName, string descriptionText)
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

      await context.Channel.SendMessageAsync($"La description {descriptionName} a été enregistrée parmis les descriptions disponibles pour votre personnage {pcName}.");
    }
  }
}
