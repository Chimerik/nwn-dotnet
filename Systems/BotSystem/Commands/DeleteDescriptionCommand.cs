using System.Collections.Generic;
using System.Threading.Tasks;
using Discord.Commands;
using Anvil.API;
using NWN.Core;

namespace NWN.Systems
{
  public static partial class BotSystem
  {
    public static async Task ExecuteDeleteDescriptionCommand(SocketCommandContext context, string pcName, string descriptionName)
    {
      await NwTask.SwitchToMainThread();

      int pcID = DiscordUtils.CheckPlayerCredentialsFromDiscord(context, pcName);
      if (pcID == 0)
      {
        await context.Channel.SendMessageAsync("Le personnage indiqué n'existe pas ou n'a pas été enregistré avec votre code Discord et votre clef cd.");

        return;
      }

      SqLiteUtils.DeletionQuery("playerDescriptions",
        new Dictionary<string, string>() { { "characterId", pcID.ToString() }, { "descriptionName", descriptionName } });

      await context.Channel.SendMessageAsync($"Description {descriptionName} supprimée pour le personnage {pcName}");
    }
  }
}
