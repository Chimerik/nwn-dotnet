using Anvil.API;
using Anvil.API.Events;
using NWN.Core.NWNX;

namespace NWN.Systems
{
  public partial class ItemSystem
  {
    public static void OnEquipCheckThreatRange(OnItemEquip onEquip)
    {
      NwCreature oCreature = onEquip.EquippedBy;
      NwItem oItem = onEquip.Item;

      if (oCreature is null || oItem is null || onEquip.Slot != InventorySlot.RightHand)
        return;

      if (ItemUtils.IsMeleeWeapon(oItem.BaseItem))
        CreatureUtils.InitThreatRange(oCreature);
    }
  }
}
