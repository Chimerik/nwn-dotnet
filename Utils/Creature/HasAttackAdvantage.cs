using Anvil.API;
using NWN.Native.API;
using Ability = Anvil.API.Ability;
using BaseItemType = Anvil.API.BaseItemType;

namespace NWN.Systems
{
  public static partial class CreatureUtils
  {
    public static bool HasAttackAdvantage(CNWSCreature attacker, CNWSCombatAttackData attackData, BaseItemType weaponType, Ability attackStat, CNWSCreature target)
    {
      bool rangedAttack = attackData.m_bRangedAttack.ToBool();

      if (target is not null)
      {
        if (rangedAttack && GetHighGroundAdvantage(attacker, target))
          return true;

        if (GetAttackerAdvantageEffects(attacker, target, attackStat))
          return true;

        if (GetTargetAdvantageEffects(target, rangedAttack))
          return true;

        if (GetInvisibleAttackerAdvantage(attacker, target))
          return true;

        if (GetDiversionTargetAdvantage(attacker, target))
          return true;

        if (GetFeinteAttackerAdvantage(attacker))
          return true;
      }

      if (GetSentinelleOpportunityAdvantage(attacker, attackData))
        return true;

      return false;
    }
    public static bool GetSpellAttackAdvantageAgainstTarget(NwCreature attacker, NwSpell spell, int isRangedSpell, NwCreature target, Ability spellCastingAbility)
    {
      bool rangedSpell = isRangedSpell.ToBool();

      if (target is not null)
      {
        if (rangedSpell && GetHighGroundAdvantage(attacker, target))
          return true;

        if (GetAttackerAdvantageEffects(attacker, target, spellCastingAbility))
          return true;

        if (GetTargetAdvantageEffects(target, rangedSpell))
          return true;

        if(GetDiversionTargetAdvantage(attacker, target))
          return true;

        if (GetMetallicArmorAdvantage(target, spell))
          return true;

        if (GetInvisibleAttackerAdvantage(attacker, target))
          return true;
      }

      return false;
    }
  }
}
