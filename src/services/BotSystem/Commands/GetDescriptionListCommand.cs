using System.Collections.Generic;
using System.Threading.Tasks;
using Discord.Commands;
using Anvil.API;
using Utils;

namespace BotSystem
{
  public static partial class BotCommand
    {
    public static async Task ExecuteGetDescriptionListCommand(SocketCommandContext context, string pcName)
    {
      await NwTask.SwitchToMainThread();

      int pcID = DiscordUtils.CheckPlayerCredentialsFromDiscord(context, pcName);
      if (pcID == 0)
      {
        await context.Channel.SendMessageAsync("Le personnage indiqué n'existe pas ou n'a pas été enregistré avec votre code Discord et votre clef cd.");
        return;
      }

      var query = SqLiteUtils.SelectQuery("playerDescriptions",
        new List<string>() { { "descriptionName" } },
        new List<string[]>() { new string[] { "characterId", pcID.ToString() } });

      string result = "";

      foreach(var description in query.Results)
        result += description.GetString(0) + "\n";

      await context.Channel.SendMessageAsync($"Voici la liste des descriptions enregistrées pour {pcName} :\n{result}");
    }
  }
}
