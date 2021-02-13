using System.Threading.Tasks;
using Discord.Commands;
using NWN.API;
using NWN.Core;

namespace NWN.Systems
{
  public static partial class BotSystem
  {
    public static async Task ExecuteRegisterDiscordId(SocketCommandContext context, string cdKey)
    {
      await NwTask.SwitchToMainThread();

      var query = NWScript.SqlPrepareQueryCampaign(Config.database, $"UPDATE PlayerAccounts SET discordId = @discordId WHERE cdKey = @cdKey");
      NWScript.SqlBindString(query, "@cdKey", cdKey);
      NWScript.SqlBindInt(query, "@discordId", (int)context.User.Id);
      NWScript.SqlStep(query);

      await context.Channel.SendMessageAsync("Voilà qui est fait. Enfin, pour tant soit peu que la clef fournie fusse valide !");
    }
  }
}
