using System.Threading.Tasks;
using Discord.Commands;
using Microsoft.Data.Sqlite;

namespace NWN.Systems
{
    public static partial class BotSystem
    {
        public static async Task ExecuteGetDescriptionListCommand(SocketCommandContext context, string pcName)
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

                await context.Channel.SendMessageAsync($"Voici la liste des descriptions enregistrées pour {pcName} :\n{result}");
            }
        }
    }
}
