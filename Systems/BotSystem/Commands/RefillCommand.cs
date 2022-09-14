using Anvil.API;
using System.Threading.Tasks;
using Discord.WebSocket;

namespace NWN.Systems
{
  public static partial class BotSystem
  {
    public static async Task ExecuteRefillCommand(SocketSlashCommand command)
    {
      await NwTask.SwitchToMainThread();

      ModuleSystem.SpawnCollectableResources();
      await command.RespondAsync("Refill terminé.", ephemeral: true);
    }
  }
}
