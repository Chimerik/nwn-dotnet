using System.Threading.Tasks;
using Discord.Commands;
using NWN.API;
using NWN.Core;

namespace NWN.Systems
{
  public static partial class BotSystem
  {
    public static async Task ExecuteGetCharacterListCommand(SocketCommandContext context)
    {
      await NwTask.SwitchToMainThread();

      var query = NWScript.SqlPrepareQueryCampaign(Config.database, $"SELECT characterName, rowid from playerCharacters");
      string result = "";

      while (NWScript.SqlStep(query) > 0)
        result += "ID : " + NWScript.SqlGetString(query, 1) + " - " + NWScript.SqlGetString(query, 0) + "\n";

      await context.Channel.SendMessageAsync($"Voici la liste des personnages créés sur le module :\n{result}");
      return;
    }
  }
}
