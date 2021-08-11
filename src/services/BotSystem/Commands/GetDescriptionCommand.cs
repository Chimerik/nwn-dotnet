using System.Collections.Generic;
using System.Threading.Tasks;
using Discord.Commands;
using Anvil.API;
using Utils;

namespace BotSystem
{
  public static partial class BotCommand
    {
    public static async Task ExecuteGetDescriptionCommand(SocketCommandContext context, string pcName, string descriptionName)
    {
      await NwTask.SwitchToMainThread();

      int pcID = DiscordUtils.CheckPlayerCredentialsFromDiscord(context, pcName);
      if (pcID == 0)
      {
        await context.Channel.SendMessageAsync("Le personnage indiqué n'existe pas ou n'a pas été enregistré avec votre code Discord et votre clef cd.");
        return;
      }

      var query = SqLiteUtils.SelectQuery("playerDescriptions",
        new List<string>() { { "description" } },
        new List<string[]>() { new string[] { "characterId", pcID.ToString() }, { new string[] { "descriptionName", descriptionName } } });

      if(query.Result != null)
        await context.Channel.SendMessageAsync(query.Result.GetString(0));
    }
  }
}
