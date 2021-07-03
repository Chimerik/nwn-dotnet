using System.Collections.Generic;
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

      var query = SqLiteUtils.SelectQuery("playerCharacters",
        new List<string>() { { "characterName" }, { "rowid" } },
        new List<string[]>() );

      string result = "";

      if(query != null)
      foreach (var player in query)
        result += "ID : " + player.GetString(1) + " - " + player.GetString(0) + "\n";

      await context.Channel.SendMessageAsync($"Voici la liste des personnages créés sur le module :\n{result}");
      return;
    }
  }
}
