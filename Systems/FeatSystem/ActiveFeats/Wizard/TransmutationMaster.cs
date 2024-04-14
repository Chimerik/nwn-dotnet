using Anvil.API;
using static NWN.Systems.PlayerSystem.Player;
using static NWN.Systems.PlayerSystem;

namespace NWN.Systems
{
  public partial class FeatSystem
  {
    private static void TransmutationMaster(Player player)
    {
      if (!player.windows.TryGetValue("transmutationMasterChoice", out var value)) player.windows.Add("transmutationMasterChoice", new TransmutationMasterWindow(player));
      else ((TransmutationMasterWindow)value).CreateWindow();
    }
  }
}
