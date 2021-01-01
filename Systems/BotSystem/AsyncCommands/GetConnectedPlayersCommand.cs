using System.Linq;
using System.Threading.Tasks;
using Discord.Commands;

namespace NWN.Systems
{
  public static partial class BotSystem
  {
    public static async Task ExecuteGetConnectedPlayersCommand(SocketCommandContext context)
    {
      await context.Channel.SendMessageAsync($"Nous sommes actuellement {PlayerSystem.Players.Where(kv => kv.Value.isConnected).Count()} joueur(s) sur le module !");
    }
  }
}
