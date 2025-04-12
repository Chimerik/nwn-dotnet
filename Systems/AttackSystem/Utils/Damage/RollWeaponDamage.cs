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

      LogUtils.LogMessage($"{(isCriticalRoll ? "Critique - " : "")}{weapon.Name.ToString()} - {numDamageDice}d{damageDie} => {damage}", LogUtils.LogType.Combat);

      damage += GetDamageEffects(creature, target, attackAbility, false, isCriticalRoll, !attackData.m_bRangedAttack.ToBool())
        + GetCogneurLourdBonusDamage(creature, attackWeapon, isCriticalRoll)
        + GetTireurDeliteBonusDamage(creature, attackData, attackWeapon, isCriticalRoll)
        + GetArcherDeForceBonusDamage(creature, attackData, attackWeapon, isCriticalRoll)
        + GetStyleArmeDeJetBonusDamage(creature, attackData, attackWeapon, isCriticalRoll)
        + GetSuperiorityDiceDamage(creature, isCriticalRoll)
        + GetFormeSauvagePanthereBonusDamage(creature, target)
        + GetAnimalCompanionBonusDamage(creature, isCriticalRoll)
        - GetMaitreArmureLourdeDamageReduction(target, isCriticalRoll)
        - GetParadeDamageReduction(target, isCriticalRoll);

      return damage;
    }
  }
}
