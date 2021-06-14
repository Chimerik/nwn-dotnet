using System.Linq;
using System.Threading.Tasks;
using Discord.Commands;
using NWN.API;
using NWN.Core;

namespace NWN.Systems
{
  public static partial class BotSystem
  {
    public static async Task ExecuteSaveAreaDescriptionCommand(SocketCommandContext context, string areaTag, string descriptionText)
    {
      await NwTask.SwitchToMainThread();

      switch(DiscordUtils.GetPlayerStaffRankFromDiscord(context.User.Id))
      {
        default:
        await context.Channel.SendMessageAsync("Il faut faire partie du staff pour exécuter ce type de commande.");
        return;
        case "admin":
        case "staff":

          if(!NwModule.FindObjectsWithTag<NwArea>(areaTag).Any())
          {
            await context.Channel.SendMessageAsync($"Aucune zone correspondant au tag {areaTag} n'existe sur le module.");
            return;
          }

          var query = NWScript.SqlPrepareQueryCampaign(Config.database, $"INSERT INTO areaDescriptions " +
        $"(areaTag, description) " +
        $"VALUES  (@areaTag, @description) " +
        $"ON CONFLICT(areaTag) DO UPDATE SET description = @description");
          NWScript.SqlBindString(query, "@areaTag", areaTag);
          NWScript.SqlBindString(query, "@description", descriptionText);
          NWScript.SqlStep(query);

          await context.Channel.SendMessageAsync($"La description de la zone a bien été enregistrée.");

          break;
      }
    }
  }
}
