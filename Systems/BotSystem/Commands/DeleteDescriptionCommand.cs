using System.Threading.Tasks;
using Discord.Commands;
using Microsoft.Data.Sqlite;

namespace NWN.Systems
{
    public static partial class BotSystem
    {
        public static async Task ExecuteDeleteDescriptionCommand(SocketCommandContext context, string pcName, string descriptionName)
        {
            int pcID = DiscordUtils.CheckPlayerCredentialsFromDiscord(context, pcName);
            if (pcID == 0)
            {
                await context.Channel.SendMessageAsync("Le personnage indiqué n'existe pas ou n'a pas été enregistré avec votre code Discord et votre clef cd.");
                return;
            }

            using (var connection = new SqliteConnection($"{Config.db_path}"))
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

                await context.Channel.SendMessageAsync($"Description {descriptionName} supprimée pour le personnage {pcName}");
            }
        }
    }
}
