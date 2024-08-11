using Anvil.API;
using static NWN.Systems.PlayerSystem;

namespace NWN.Systems
{
  public partial class FeatSystem
  {
    private static void SourceToSlot(NwCreature caster)
    {
      if (Players.TryGetValue(caster, out var player))
      {
        if (!player.windows.TryGetValue("sourceToSlot", out var sourceToSlot))
          player.windows.Add("sourceToSlot", new Player.SourceToSlotWindow(player));
        else if (((Player.SourceToSlotWindow)sourceToSlot).IsOpen)
          ((Player.SourceToSlotWindow)sourceToSlot).CloseWindow();
        else
          ((Player.SourceToSlotWindow)sourceToSlot).CreateWindow();
      }
    }
  }
}
