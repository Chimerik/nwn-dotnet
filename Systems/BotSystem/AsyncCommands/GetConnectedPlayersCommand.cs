using System.Linq;

namespace NWN.Systems
{
  public static partial class BotSystem
  {
    public static int ExecuteGetConnectedPlayersCommand()
    {
      return (PlayerSystem.Players.Where(kv => kv.Value.isConnected)).Count();
    }
  }
}
