using Anvil.API;
using static NWN.Systems.PlayerSystem;

namespace NWN.Systems
{
  public partial class FeatSystem
  {
    private static void SlotToSource(NwCreature caster)
    {
      if (Players.TryGetValue(caster, out var player))
      {
        if (!player.windows.TryGetValue("slotToSource", out var sourceToSlot))
          player.windows.Add("slotToSource", new Player.SlotToSourceWindow(player));
        else if (((Player.SlotToSourceWindow)sourceToSlot).IsOpen)
          ((Player.SlotToSourceWindow)sourceToSlot).CloseWindow();
        else
          ((Player.SlotToSourceWindow)sourceToSlot).CreateWindow();
      }
    }
  }
}
