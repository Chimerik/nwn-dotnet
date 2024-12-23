using Anvil.API;
using NWN.Native.API;

namespace NWN.Systems
{
  public static partial class NativeUtils
  {
    public static int RollWeaponDamage(CNWSCreature creature, NwBaseItem weapon, CNWSCombatAttackData attackData, CNWSCreature target, CNWSItem attackWeapon, Anvil.API.Ability attackAbility, bool isCriticalRoll = false)
    {
      int damageDie = GetShillelaghDamageDie(creature, weapon);
      damageDie = ItemUtils.IsVersatileWeapon(weapon.ItemType) && creature.m_pInventory.GetItemInSlot((uint)EquipmentSlot.LeftHand) is null
        ? damageDie + 2 : damageDie;

      int numDamageDice = GetShillelaghNumDice(creature, weapon);
      numDamageDice = numDamageDice
        + GetFureurOrcBonus(creature)
        //+ GetOrcCriticalBonus(creature, attackData, isCriticalRoll)
        + GetEmpaleurCriticalBonus(creature, weapon, isCriticalRoll);
        //+ GetBarbarianBrutalCriticalBonus(creature, attackData.m_bRangedAttack.ToBool(), isCriticalRoll);

      int damage = HandleWeaponDamageRerolls(creature, weapon, numDamageDice, damageDie);
      damage = HandleSavageAttacker(creature, weapon, attackData, numDamageDice, damage, damageDie);

      LogUtils.LogMessage($"{(isCriticalRoll ? "Critique - " : "")}{weapon.Name.ToString()} - {numDamageDice}d{damageDie} => {damage}", LogUtils.LogType.Combat);

      damage += GetDegatsBotte(creature)
        + GetDegatsVaillantsBonus(creature)
        + GetSuperiorityDiceDamage(creature)
        + GetBarbarianRageBonusDamage(creature, attackAbility, !attackData.m_bRangedAttack.ToBool())
        + GetFrappeFrenetiqueBonusDamage(creature, attackAbility, !attackData.m_bRangedAttack.ToBool())
        + GetFrappeBrutaleBonusDamage(creature, target, attackAbility, !attackData.m_bRangedAttack.ToBool())
        + GetPhysicalBonusDamage(creature, attackWeapon)    
        + GetFaveurDuMalinBonusDamage(creature)    
        + GetChargeurBonusDamage(creature)
        + GetCogneurLourdBonusDamage(creature, attackWeapon)
        + GetTireurDeliteBonusDamage(creature, attackData, attackWeapon)
        + GetTranspercerBonusDamage(creature)
        + GetTirPercantBonusDamage(creature)
        + GetFormeSauvagePanthereBonusDamage(creature, target);

      return damage;
    }
  }
}
