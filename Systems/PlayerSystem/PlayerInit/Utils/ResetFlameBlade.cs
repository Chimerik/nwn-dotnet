using Anvil.API;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      private void ResetFlameBlade()
      {
        if (oid.LoginCreature.GetObjectVariable<LocalVariableInt>(EffectSystem.ConcentrationSpellIdString).Value != CustomSpell.FlameBlade
          && oid.LoginCreature.GetItemInSlot(InventorySlot.RightHand)?.Tag == "_TEMP_FLAME_BLADE")
          oid.LoginCreature.GetItemInSlot(InventorySlot.RightHand).Destroy();
      }
    }
  }
}
