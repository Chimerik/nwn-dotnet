using System.Threading.Tasks;
using Discord.Commands;
using NWN.API;
using NWN.Core;

namespace NWN.Systems
{
  public static partial class BotSystem
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

      var query = NWScript.SqlPrepareQueryCampaign(Config.database, 
        $"SELECT description " +
        $"FROM playerDescriptions " +
        $"WHERE characterId = @characterId AND descriptionName = @descriptionName");
      NWScript.SqlBindInt(query, "@characterId", pcID);
      NWScript.SqlBindString(query, "@descriptionName", descriptionName);

      NWScript.SqlStep(query);
      await context.Channel.SendMessageAsync(NWScript.SqlGetString(query, 0));
    }
  }
}
