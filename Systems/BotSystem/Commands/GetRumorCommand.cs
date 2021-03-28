using System.Threading.Tasks;
using Discord.Commands;
using NWN.API;
using NWN.Core;

namespace NWN.Systems
{
  public static partial class BotSystem
  {
    public static async Task ExecuteGetRumorCommand(SocketCommandContext context, int rumorId)
    {
      await NwTask.SwitchToMainThread();

      var query = NWScript.SqlPrepareQueryCampaign(Config.database, "SELECT title, content from rumors r " +
        "where ROWID = @rowid");
      NWScript.SqlBindInt(query, "@rowid", rumorId);

      if(NWScript.SqlStep(query) == 0)
      {
        await context.Channel.SendMessageAsync($"La rumeur numéro {rumorId} ne semble pas exister.");
      }
      else
      {
        await context.Channel.SendMessageAsync($"{NWScript.SqlGetString(query, 0)} : \n{NWScript.SqlGetString(query, 1)}\n");
      }
    }
  }
}
