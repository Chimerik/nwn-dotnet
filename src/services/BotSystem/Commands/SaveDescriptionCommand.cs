using System.Collections.Generic;
using System.Threading.Tasks;
using Discord.Commands;
using Anvil.API;
using Utils;

namespace BotSystem
{
  public static partial class BotCommand
    {
    public static async Task ExecuteSaveDescriptionCommand(SocketCommandContext context, string pcName, string descriptionName, string descriptionText)
    {
      await NwTask.SwitchToMainThread();

      int pcID = DiscordUtils.CheckPlayerCredentialsFromDiscord(context, pcName);
      if (pcID == 0)
      {
        await context.Channel.SendMessageAsync("Le personnage indiqué n'existe pas ou n'a pas été enregistré avec votre code Discord et votre clef cd.");
        return;
      }

      SqLiteUtils.InsertQuery("playerDescriptions",
          new List<string[]>() {
            new string[] { "characterId", pcID.ToString() },
            new string[] { "descriptionName", descriptionName },
            new string[] { "description", descriptionText } },
          new List<string>() { "characterId", "descriptionName" },
          new List<string[]>() { new string[] { "description" } });

      await context.Channel.SendMessageAsync($"La description {descriptionName} a été enregistrée parmis les descriptions disponibles pour votre personnage {pcName}.");
    }
  }
}
