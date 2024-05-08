using Anvil.API;
using NWN.Native.API;

namespace NWN.Systems
{
  public static partial class NativeUtils
  {
    public static int RollWeaponDamage(CNWSCreature creature, NwBaseItem weapon, CNWSCombatAttackData attackData, bool isCriticalRoll = false)
    {
      int dieToRoll = ItemUtils.IsVersatileWeapon(weapon.ItemType) && creature.m_pInventory.GetItemInSlot((uint)EquipmentSlot.LeftHand) is null
        ? weapon.DieToRoll + 2 : weapon.DieToRoll;

      int numDamageDice = weapon.NumDamageDice 
        + GetFureurOrcBonus(creature) 
        + GetOrcCriticalBonus(creature, attackData, isCriticalRoll)
        + GetEmpaleurCriticalBonus(creature, weapon, isCriticalRoll)
        + GetBarbarianBrutalCriticalBonus(creature, attackData.m_bRangedAttack.ToBool(), isCriticalRoll)
        + GetDegatsVaillantsBonus(creature);

      int damage = HandleWeaponDamageRerolls(creature, weapon, numDamageDice, dieToRoll);
      damage = HandleSavageAttacker(creature, weapon, attackData, numDamageDice, damage, dieToRoll);

      string criticalLog = isCriticalRoll ? "Critique - " : "";
      LogUtils.LogMessage($"{criticalLog}{weapon.Name.ToString()} - {numDamageDice}d{dieToRoll} => {damage}", LogUtils.LogType.Combat);

      damage += GetSuperiorityDiceDamage(creature);
      damage += GetBarbarianRageBonusDamage(creature, attackData);

      return damage;
    }
  }
}
