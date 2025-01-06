using Anvil.API;
using Anvil.API.Events;

namespace NWN.Systems
{
  public partial class PlayerUtils
  {
    public static void OnEquipGlobal(ModuleEvents.OnPlayerEquipItem onEquip)
    {
      if (ModuleSystem.magicWeaponsToRemove.Contains(onEquip.Item.UUID))
        onEquip.Item.RemoveItemProperties(ItemPropertyType.EnhancementBonus);
    }
  }
}
