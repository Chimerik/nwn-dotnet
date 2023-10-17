using Anvil.API.Events;
using Anvil.API;

namespace NWN.Systems
{
  public partial class ItemSystem
  {
    public static void OnAcquireForceDurability(ModuleEvents.OnAcquireItem onAcquireItem)
    {
      NwItem oItem = onAcquireItem.Item;

      if (onAcquireItem.AcquiredBy is null || onAcquireItem.AcquiredBy is not NwCreature oPC || oPC.ControllingPlayer is null || oItem is null)
        return;

      if (oItem.GetObjectVariable<LocalVariableInt>("_MAX_DURABILITY").HasNothing && (oItem.BaseItem.EquipmentSlots != EquipmentSlots.None || oItem.BaseItem.ItemType == BaseItemType.CreatureItem))
      {
        int durability = ItemUtils.GetBaseItemCost(oItem) * 25;
        oItem.GetObjectVariable<LocalVariableInt>("_MAX_DURABILITY").Value = durability;
        oItem.GetObjectVariable<LocalVariableInt>("_DURABILITY").Value = durability;
      }
    }
  }
}
