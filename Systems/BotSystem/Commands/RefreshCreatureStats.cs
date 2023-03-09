using Anvil.API;
using System.Threading.Tasks;
using Discord.WebSocket;

namespace NWN.Systems
{
  public static partial class BotSystem
  {
    public static async Task ExecuteRefreshCreatureStatsCommand(SocketSlashCommand command)
    {
      ModuleSystem.InitializeCreatureStats();
    }
  }
}
