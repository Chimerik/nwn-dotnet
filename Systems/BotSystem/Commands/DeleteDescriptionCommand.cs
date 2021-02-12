using System.Threading.Tasks;
using Discord.Commands;
using Microsoft.Data.Sqlite;
using NWN.API;
using NWN.Core;

namespace NWN.Systems
{
  public static partial class BotSystem
  {
    public static async Task ExecuteDeleteDescriptionCommand(SocketCommandContext context, string pcName, string descriptionName)
    {
      int pcID = DiscordUtils.CheckPlayerCredentialsFromDiscord(context, pcName);
      if (pcID == 0)
      {
        await context.Channel.SendMessageAsync("Le personnage indiqué n'existe pas ou n'a pas été enregistré avec votre code Discord et votre clef cd.");

        return;
      }

      await context.Channel.SendMessageAsync($"Description {descriptionName} supprimée pour le personnage {pcName}");

      await NwTask.SwitchToMainThread();

      var query = NWScript.SqlPrepareQueryCampaign(Config.database, "DELETE FROM playerDescriptions WHERE characterId = @characterId AND descriptionName = @descriptionName");
      NWScript.SqlBindInt(query, "@characterId", pcID);
      NWScript.SqlBindString(query, "@descriptionName", descriptionName);
      NWScript.SqlStep(query);
    }
  }
}
