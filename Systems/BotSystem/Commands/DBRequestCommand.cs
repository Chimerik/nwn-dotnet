using Discord.Commands;
using System.Threading.Tasks;
using Anvil.API;
using Microsoft.Data.Sqlite;

namespace NWN.Systems
{
  public static partial class BotSystem
  {
    public static async Task ExecuteDBRequestCommand(SocketCommandContext context, string request)
    {
      string rank = await DiscordUtils.GetPlayerStaffRankFromDiscord(context.User.Id);
      if (rank != "admin")
      {
        await context.Channel.SendMessageAsync("Noooon, vous n'êtes pas la maaaaaître ! Le maaaaître est bien plus poli, d'habitude !");
        return;
      }

      using (var connection = new SqliteConnection(Config.dbPath))
      {
        connection.Open();

        var command = connection.CreateCommand();
        command.CommandText = request;

        await command.ExecuteNonQueryAsync();
        await context.Channel.SendMessageAsync("Requête correctement exécutée.");
      }

      /*if (query.Error != "")
        await context.Channel.SendMessageAsync($"SQL ERROR : {query.Error}");
      else
        await context.Channel.SendMessageAsync("Requête correctement exécutée.");*/
    }
  }
}
