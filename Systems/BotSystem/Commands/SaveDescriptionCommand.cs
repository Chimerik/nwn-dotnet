using System.Threading.Tasks;
using Discord.Commands;
using NWN.API;
using NWN.Core;

namespace NWN.Systems
{
  public static partial class BotSystem
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

      var query = NWScript.SqlPrepareQueryCampaign(Systems.Config.database, $"INSERT INTO playerDescriptions " +
        $"(characterId, descriptionName, description) " +
        $"VALUES  (@characterId, @descriptionName, @description) " +
        $"ON CONFLICT(characterId, descriptionName) DO UPDATE SET description = @description");
      NWScript.SqlBindInt(query, "@characterId", pcID);
      NWScript.SqlBindString(query, "@descriptionName", descriptionName);
      NWScript.SqlBindString(query, "@description", descriptionText);
      NWScript.SqlStep(query);

      await context.Channel.SendMessageAsync($"La description {descriptionName} a été enregistrée parmis les descriptions disponibles pour votre personnage {pcName}.");
    }
  }
}
