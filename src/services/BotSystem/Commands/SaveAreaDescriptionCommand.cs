using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord.Commands;
using Anvil.API;
using Utils;

namespace BotSystem
{
  public static partial class BotCommand
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

          if(!NwObject.FindObjectsWithTag<NwArea>(areaTag).Any())
          {
            await context.Channel.SendMessageAsync($"Aucune zone correspondant au tag {areaTag} n'existe sur le module.");
            return;
          }

          SqLiteUtils.InsertQuery("areaDescriptions",
          new List<string[]>() {
            new string[] { "areaTag", areaTag },
            new string[] { "description", descriptionText } },
          new List<string>() { "areaTag" },
          new List<string[]>() { new string[] { "description" } });

          await context.Channel.SendMessageAsync($"La description de la zone a bien été enregistrée.");

          break;
      }
    }
  }
}
