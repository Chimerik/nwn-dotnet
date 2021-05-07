using System.Threading.Tasks;
using Discord.Commands;
using NWN.API;
using NWN.Core;

namespace NWN.Systems
{
  public static partial class BotSystem
  {
    public static async Task ExecuteDeleteMailCommand(SocketCommandContext context, int mailId, string characterName)
    {
      await NwTask.SwitchToMainThread();
      int result = DiscordUtils.CheckPlayerCredentialsFromDiscord(context, characterName);

      if (result <= 0)
      {
        await context.Channel.SendMessageAsync("Le personnage indiqué n'existe pas ou n'a pas été enregistré avec votre code Discord et votre clef cd.");
        return;
      }

      var query = NWScript.SqlPrepareQueryCampaign(Config.database, $"DELETE from messenger where characterId = @characterId and ROWID = @mailId");
      NWScript.SqlBindInt(query, "@characterId", result);
      NWScript.SqlBindInt(query, "@mailId", mailId);
      NWScript.SqlStep(query);

      await context.Channel.SendMessageAsync("Message supprimé");
    }
  }
}
