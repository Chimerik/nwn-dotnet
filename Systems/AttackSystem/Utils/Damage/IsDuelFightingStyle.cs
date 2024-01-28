using Anvil.API;
using NWN.Native.API;
using CreatureSize = Anvil.API.CreatureSize;

namespace NWN.Systems
{
  public static partial class NativeUtils
  {
    public static bool IsDuelFightingStyle(CNWSCreature creature, NwBaseItem weapon, CNWSCombatAttackData attackData)
    {
      if (attackData.m_bRangedAttack.ToBool() || !creature.m_pStats.HasFeat(CustomSkill.FighterCombatStyleDuel).ToBool()
        || ItemUtils.IsTwoHandedWeapon(weapon, (CreatureSize)creature.m_nCreatureSize))
        return false;

      var offHandWeapon = creature.m_pInventory.GetItemInSlot((uint)Native.API.InventorySlot.LeftHand);
      var baseOffHand = NwBaseItem.FromItemId((int)offHandWeapon.m_nBaseItem);

      if ((ItemUtils.IsVersatileWeapon(weapon.ItemType) && offHandWeapon is null) || (offHandWeapon is not null && ItemUtils.IsWeapon(baseOffHand)))
        return false;

      LogUtils.LogMessage($"Style de combat duel, +2 dégâts appliqués", LogUtils.LogType.Combat);
      return true;
    }
  }
}
