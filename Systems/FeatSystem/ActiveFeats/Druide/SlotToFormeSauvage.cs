using Anvil.API;
using static NWN.Systems.PlayerSystem;

namespace NWN.Systems
{
  public partial class FeatSystem
  {
    private static void SlotToFormeSauvage(NwCreature caster)
    {
      if (Players.TryGetValue(caster, out var player))
      {
        if (!player.windows.TryGetValue("formeSauvageToSlot", out var sourceToSlot))
          player.windows.Add("formeSauvageToSlot", new Player.FormeSauvageToSlotWindow(player));
        else if (((Player.FormeSauvageToSlotWindow)sourceToSlot).IsOpen)
          ((Player.FormeSauvageToSlotWindow)sourceToSlot).CloseWindow();
        else
          ((Player.FormeSauvageToSlotWindow)sourceToSlot).CreateWindow();
      }
    }
  }
}
