using System.Threading.Tasks;
using Discord.Commands;
using NWN.API;
using NWN.Core;

namespace NWN.Systems
{
  public static partial class BotSystem
  {
    public static async Task ExecuteGetMyMailsListCommand(SocketCommandContext context, string characterName)
    {
      await NwTask.SwitchToMainThread();
      int result = DiscordUtils.CheckPlayerCredentialsFromDiscord(context, characterName);

      if (result <= 0)
      {
        await context.Channel.SendMessageAsync("Le personnage indiqué n'existe pas ou n'a pas été enregistré avec votre code Discord et votre clef cd.");
        return;
      }

      var query = NWScript.SqlPrepareQueryCampaign(Config.database, $"SELECT rowid, senderName, title, sentDate, read from messenger where characterId = @characterId order by sentDate desc");
      NWScript.SqlBindInt(query, "@characterId", result);

      string messageList = $"Liste des messages reçus par {characterName} :\n\n";
      
      while(NWScript.SqlStep(query) != 0)
      {
        if (NWScript.SqlGetInt(query, 4) == 1)
          messageList += "Lu | ";
        messageList += $"{NWScript.SqlGetInt(query, 0)} | {NWScript.SqlGetString(query, 1)} | {NWScript.SqlGetString(query, 2)} | {NWScript.SqlGetString(query, 3)}";
      }

      await context.Channel.SendMessageAsync(messageList);
    }
  }
}
