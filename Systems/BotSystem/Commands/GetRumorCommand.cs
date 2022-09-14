using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord.WebSocket;

namespace NWN.Systems
{
  public static partial class BotSystem
  {
    public static async Task ExecuteGetRumorCommand(SocketSlashCommand command)
    {
      string rumorId = command.Data.Options.First().ToString();

      var query = await SqLiteUtils.SelectQueryAsync("rumors",
            new List<string>() { { "title" }, { "content" } },
            new List<string[]>() { new string[] { "ROWID", rumorId } });

      if(query == null || query.Count < 1)
        await command.RespondAsync($"La rumeur numéro {rumorId} ne semble pas exister.", ephemeral: true);
      else
        await command.RespondAsync($"{query[0][0]} : \n{query[0][1]}\n", ephemeral: true);
    }
  }
}
