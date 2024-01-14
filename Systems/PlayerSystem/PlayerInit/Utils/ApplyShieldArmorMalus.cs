using Anvil.API;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      private void ApplyShieldArmorMalus()
      {
        ItemUtils.CheckArmorShieldProficiency(oid.LoginCreature, oid.LoginCreature.GetItemInSlot(InventorySlot.Chest));
        ItemUtils.CheckArmorShieldProficiency(oid.LoginCreature, oid.LoginCreature.GetItemInSlot(InventorySlot.LeftHand));
      }
    }
  }
}
