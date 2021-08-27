using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord.Commands;
using Anvil.API;
using NWN.Core;

namespace NWN.Systems
{
  public static partial class BotSystem
  {
    public static async Task ExecuteGetRumorCommand(SocketCommandContext context, int rumorId)
    {
      var query = await SqLiteUtils.SelectQueryAsync("rumors",
            new List<string>() { { "title" }, { "content" } },
            new List<string[]>() { new string[] { "ROWID", rumorId.ToString() } });

      if(query == null || query.Count < 1)
      {
        await context.Channel.SendMessageAsync($"La rumeur numéro {rumorId} ne semble pas exister.");
      }
      else
      {
        await context.Channel.SendMessageAsync($"{query[0][0]} : \n{query[0][1]}\n");
      }
    }
  }
}
