using System.Linq;
using Anvil.API;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      private void ResetFlameBlade()
      {
        if (oid.LoginCreature.GetItemInSlot(InventorySlot.RightHand)?.Tag == "_TEMP_FLAME_BLADE"
          && !oid.LoginCreature.ActiveEffects.Any(e => e.Tag == EffectSystem.LameArdenteEffectTag))
          oid.LoginCreature.GetItemInSlot(InventorySlot.RightHand).Destroy();
      }
    }
  }
}
