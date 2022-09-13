using System.Threading.Tasks;
using Microsoft.Data.Sqlite;
using Discord.WebSocket;
using System.Linq;

namespace NWN.Systems
{
  public static partial class BotSystem
  {
    public static async Task ExecuteDBRequestCommand(SocketSlashCommand command)
    {
      using (var connection = new SqliteConnection(Config.dbPath))
      {
        connection.Open();

        var sqlCommand = connection.CreateCommand();
        sqlCommand.CommandText = command.Data.Options.First().ToString();

        await sqlCommand.ExecuteNonQueryAsync();
        await command.RespondAsync("Requête correctement exécutée.");
      }

      /*if (query.Error != "")
        await context.Channel.SendMessageAsync($"SQL ERROR : {query.Error}");
      else
        await context.Channel.SendMessageAsync("Requête correctement exécutée.");*/
    }
  }
}
