using Discord.Commands;
using System.Threading.Tasks;
using Anvil.API;

namespace NWN.Systems
{
  public static partial class BotSystem
  {
    public static async Task ExecuteDBRequestCommand(SocketCommandContext context, string request)
    {
      await NwTask.SwitchToMainThread();

      if (DiscordUtils.GetPlayerStaffRankFromDiscord(context.User.Id) != "admin")
      {
        await context.Channel.SendMessageAsync("Noooon, vous n'êtes pas la maaaaaître ! Le maaaaître est bien plus poli, d'habitude !");
        return;
      }

      var query = NwModule.Instance.PrepareCampaignSQLQuery(Config.database, request);
      query.Execute();
        
      if(query.Error != "")
        await context.Channel.SendMessageAsync($"SQL ERROR : {query.Error}");
      else
        await context.Channel.SendMessageAsync("Requête correctement exécutée.");
    }
  }
}
