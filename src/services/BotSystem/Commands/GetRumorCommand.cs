using System.Collections.Generic;
using System.Threading.Tasks;
using Discord.Commands;
using Anvil.API;
using Utils;

namespace BotSystem
{
  public static partial class BotCommand
    {
    public static async Task ExecuteGetRumorCommand(SocketCommandContext context, int rumorId)
    {
      await NwTask.SwitchToMainThread();

      var query = SqLiteUtils.SelectQuery("rumors",
            new List<string>() { { "title" }, { "content" } },
            new List<string[]>() { new string[] { "ROWID", rumorId.ToString() } });

      if(query.Result == null)
      {
        await context.Channel.SendMessageAsync($"La rumeur numéro {rumorId} ne semble pas exister.");
      }
      else
      {
        await context.Channel.SendMessageAsync($"{query.Result.GetString(0)} : \n{query.Result.GetString(1)}\n");
      }
    }
  }
}
