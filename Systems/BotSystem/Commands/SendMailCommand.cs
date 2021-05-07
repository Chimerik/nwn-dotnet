using System.Threading.Tasks;
using Discord.Commands;
using NWN.API;
using NWN.Core;

namespace NWN.Systems
{
  public static partial class BotSystem
  {
    public static async Task ExecuteSendMailCommand(SocketCommandContext context, string senderName, int characterId, string title, string content)
    {
      await NwTask.SwitchToMainThread();
      int result = DiscordUtils.CheckPlayerCredentialsFromDiscord(context, senderName);

      if (result <= 0)
      {
        await context.Channel.SendMessageAsync("Le personnage indiqué n'existe pas ou n'a pas été enregistré avec votre code Discord et votre clef cd.");
        return;
      }

      var getNameQuery = NWScript.SqlPrepareQueryCampaign(Config.database, $"SELECT characterName from playerCharacters where ROWID = @rowid");
      NWScript.SqlBindInt(getNameQuery, "@rowid", result);

      if (NWScript.SqlStep(getNameQuery) == 0)
      {
        await context.Channel.SendMessageAsync("Impossible de trouver une correspondance pour le nom du personnage indiqué.");
        return;
      }

      string senderFullName = NWScript.SqlGetString(getNameQuery, 0);
      Utils.SendMailToPC(characterId, senderFullName, title, content);

      await context.Channel.SendMessageAsync("Courrier en cours d'envoi.");
    }
  }
}
