using Anvil.API;
using NWN.Native.API;

namespace NWN.Systems
{
  public static partial class NativeUtils
  {
    public static int RollWeaponDamage(CNWSCreature creature, NwBaseItem weapon, CNWSCombatAttackData attackData, CNWSCreature target, CNWSItem attackWeapon, Anvil.API.Ability attackAbility, bool isCriticalRoll = false)
    {
      int dieToRoll = ItemUtils.IsVersatileWeapon(weapon.ItemType) && creature.m_pInventory.GetItemInSlot((uint)EquipmentSlot.LeftHand) is null
        ? weapon.DieToRoll + 2 : weapon.DieToRoll;

      int numDamageDice = weapon.NumDamageDice
        + GetFureurOrcBonus(creature)
        //+ GetOrcCriticalBonus(creature, attackData, isCriticalRoll)
        + GetEmpaleurCriticalBonus(creature, weapon, isCriticalRoll);
        //+ GetBarbarianBrutalCriticalBonus(creature, attackData.m_bRangedAttack.ToBool(), isCriticalRoll);


      int damage = HandleWeaponDamageRerolls(creature, weapon, numDamageDice, dieToRoll);
      damage = HandleSavageAttacker(creature, weapon, attackData, numDamageDice, damage, dieToRoll);

      LogUtils.LogMessage($"{(isCriticalRoll ? "Critique - " : "")}{weapon.Name.ToString()} - {numDamageDice}d{dieToRoll} => {damage}", LogUtils.LogType.Combat);

      damage += GetDegatsBotte(creature)
        + GetDegatsVaillantsBonus(creature)
        + GetFavoredEnemyDegatsBonus(creature, target)
        + GetSuperiorityDiceDamage(creature)
        + GetBarbarianRageBonusDamage(creature, attackAbility, !attackData.m_bRangedAttack.ToBool())
        + GetFrappeBrutaleBonusDamage(creature, target, attackAbility, !attackData.m_bRangedAttack.ToBool())
        + GetPhysicalBonusDamage(creature, attackWeapon)    
        + GetFaveurDuMalinBonusDamage(creature)    
        + GetFormeSauvagePanthereBonusDamage(creature, target);    

      return damage;
    }
  }
}
