using System.Threading.Tasks;
using Discord.Commands;
using Microsoft.Data.Sqlite;
using NWN.API;
using NWN.Core;

namespace NWN.Systems
{
  public static partial class BotSystem
  {
    public static async Task ExecuteGetDescriptionListCommand(SocketCommandContext context, string pcName)
    {
      int pcID = DiscordUtils.CheckPlayerCredentialsFromDiscord(context, pcName);
      if (pcID == 0)
      {
        await context.Channel.SendMessageAsync("Le personnage indiqué n'existe pas ou n'a pas été enregistré avec votre code Discord et votre clef cd.");
        return;
      }

      await NwTask.SwitchToMainThread();

      var query = NWScript.SqlPrepareQueryCampaign(Config.database,
        $"SELECT descriptionName " +
        $"FROM playerDescriptions " +
        $"WHERE characterId = @characterId");
      NWScript.SqlBindInt(query, "@characterId", pcID);

      string result = "";

      while(NWScript.SqlStep(query)> 0);
        result += NWScript.SqlGetString(query, 0) + "\n";

      await context.Channel.SendMessageAsync($"Voici la liste des descriptions enregistrées pour {pcName} :\n{result}");
    }
  }
}
