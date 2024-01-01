using Anvil.API;
using Anvil.API.Events;

namespace NWN.Systems
{
  public partial class ItemSystem
  {
    public static void OnEquipDesarmement(OnItemEquip onEquip)
    {
      if (onEquip.Slot != InventorySlot.RightHand)
        return;

      onEquip.EquippedBy.ControllingPlayer?.SendServerMessage("Désarmement : Impossible de réequiper pour le moment", ColorConstants.Red);
      onEquip.PreventEquip = true;
    }
  }
}
