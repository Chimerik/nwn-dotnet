using System.Collections.Generic;
using System.Threading.Tasks;
using Discord.Commands;
using Anvil.API;
using Utils;

namespace BotSystem
{
  public static partial class BotCommand
    {
    public static async Task ExecuteGetCharacterListCommand(SocketCommandContext context)
    {
      await NwTask.SwitchToMainThread();

      var query = SqLiteUtils.SelectQuery("playerCharacters",
        new List<string>() { { "characterName" }, { "rowid" } },
        new List<string[]>() );

      string result = "";

      foreach (var player in query.Results)
        result += "ID : " + player.GetString(1) + " - " + player.GetString(0) + "\n";

      await context.Channel.SendMessageAsync($"Voici la liste des personnages créés sur le module :\n{result}");
      return;
    }
  }
}
