using Anvil.API;
using NWN.Native.API;
using Ability = Anvil.API.Ability;
using BaseItemType = Anvil.API.BaseItemType;

namespace NWN.Systems
{
  public static partial class CreatureUtils
  {
    public static bool HasAttackDisadvantage(CNWSCreature attacker, CNWSCombatAttackData attackData, CNWSItem attackWeapon, BaseItemType weaponType, Ability attackStat, CNWSCreature target)
    {
      bool rangedAttack = attackData.m_bRangedAttack.ToBool();

      if (target is not null)
      {
        if (rangedAttack)
        {
          if(GetLowGroundDisadvantage(attacker, target))
            return true;

          if (GetRangedWeaponDistanceDisadvantage(attacker, weaponType, target))
            return true;

          if (GetThreatenedDisadvantage(attacker, attackWeapon))
            return true;
        }

        if (GetAttackerDisadvantageEffects(attacker, target, attackStat))
          return true;

        if (GetTargetDisadvantageEffects(target, rangedAttack))
          return true;

        if (GetInvisibleTargetDisadvantage(attacker, target))
          return true;
      }

      if (GetSmallCreaturesHeavyWeaponDisadvantage(attacker, weaponType))
        return true;

      return false;
    }
    public static bool GetSpellAttackDisadvantageAgainstTarget(NwCreature attacker, NwSpell spell, int isRangedSpell, NwCreature target, Ability spellCastingAbility)
    {
      bool rangedSpell = isRangedSpell.ToBool();

      if (target is not null)
      {
        if (rangedSpell && GetThreatenedDisadvantage(attacker))
          return true;

        if (GetAttackerDisadvantageEffects(attacker, target, spellCastingAbility))
          return true;

        if (GetTargetDisadvantageEffects(target, rangedSpell))
          return true;

        if (GetInvisibleTargetDisadvantage(attacker, target))
          return true;
      }

      return false;
    }
  }
}
