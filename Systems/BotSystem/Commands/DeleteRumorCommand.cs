using System.Collections.Generic;
using System.Threading.Tasks;
using Discord.WebSocket;
using System.Linq;

namespace NWN.Systems
{
  public static partial class BotSystem
  {
    public static async Task ExecuteDeleteRumorCommand(SocketSlashCommand command)
    {
      SqLiteUtils.DeletionQuery("rumors",
        new Dictionary<string, string>() { { "ROWID", command.Data.Options.First().Value.ToString() } });

      await command.RespondAsync($"La rumeur numéro {command.Data.Options.First().Value} a bien été supprimée.", ephemeral: true);
    }
  }
}
