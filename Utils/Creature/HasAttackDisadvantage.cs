﻿using Anvil.API;
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
          if (GetLowGroundDisadvantage(attacker, target))
            return true;

          if (GetRangedWeaponDistanceDisadvantage(attacker, weaponType, target))
            return true;
        }

        if (GetEsquiveDuTraqueurDisadvantage(target))
          return true;

        if (GetAttackerDisadvantageEffects(attacker, target, attackStat, rangedAttack, attackWeapon))
          return true;

        if (GetTargetDisadvantageEffects(attacker, target, rangedAttack, attackData))
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
      if(target is null)
        return false;

      bool rangedSpell = isRangedSpell.ToBool();

      if (GetEsquiveDuTraqueurDisadvantage(target))
        return true;

      if (GetAttackerDisadvantageEffects(attacker, target, spellCastingAbility, rangedSpell))
        return true;

      if (GetTargetDisadvantageEffects(attacker, target, rangedSpell))
        return true;

      if (GetInvisibleTargetDisadvantage(attacker, target))
        return true;

      return false;
    }
  }
}
