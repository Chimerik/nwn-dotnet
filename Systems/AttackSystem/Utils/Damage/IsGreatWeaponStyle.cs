using Anvil.API;
using NWN.Native.API;
using CreatureSize = Anvil.API.CreatureSize;

namespace NWN.Systems
{
  public static partial class NativeUtils
  {
    public static bool IsGreatWeaponStyle(NwBaseItem baseItem, CNWSCreature creature)
    {
      return ItemUtils.IsMeleeWeapon(baseItem)
        && (ItemUtils.IsTwoHandedWeapon(baseItem, (CreatureSize)creature.m_nCreatureSize)
        || (ItemUtils.IsVersatileWeapon(baseItem.ItemType) && creature.m_pInventory.GetItemInSlot((uint)EquipmentSlot.LeftHand) is null));
    }
  }
}
