using Anvil.API;
using NWN.Native.API;
using Ability = Anvil.API.Ability;
using BaseItemType = Anvil.API.BaseItemType;

namespace NWN
{
  public static partial class CreatureUtils
  {
    // > 1 => Advantage
    // 0 = Neutral
    // < 1 => Disadvantage
    public static int GetAdvantageAgainstTarget(CNWSCreature attacker, CNWSCombatAttackData attackData, CNWSItem attackWeapon, Ability attackStat, CNWSCreature target)
    {
      BaseItemType weaponType = attackWeapon is not null ? NwBaseItem.FromItemId((int)attackWeapon.m_nBaseItem).ItemType : BaseItemType.Invalid;
      int advantage = 0;

      if (target is not null)
      {
        if (attackData.m_bRangedAttack.ToBool())
        {
          advantage += GetHighGroundAdvantage(attacker, target);
          advantage += GetRangedWeaponDistanceDisadvantage(attacker, weaponType, target);
          advantage += GetThreatenedDisadvantage(attacker, attackWeapon);
        }
        else
          advantage += GetJeuDeJambeAttackerDisadvantage(target);

        advantage += GetKnockdownAdvantage(attackData.m_bRangedAttack, target);
        advantage += GetAttackerAdvantageEffects(attacker, target, attackStat);
        advantage += GetTargetAdvantageEffects(target);
        advantage += GetInvisibleTargetDisadvantage(attacker, target);
        advantage += GetInvisibleAttackerAdvantage(attacker, target);
        advantage += GetDiversionTargetAdvantage(attacker, target);
        advantage += GetFeinteAttackerAdvantage(attacker);
      }

      advantage += GetSmallCreaturesHeavyWeaponDisadvantage(attacker, weaponType);
      advantage += GetSentinelleOpportunityAdvantage(attacker, attackData);

      return advantage;
    }
    public static int GetSpellAttackAdvantageAgainstTarget(NwCreature attacker, NwSpell spell, int isRangedSpell, NwCreature target, Ability spellCastingAbility)
    {
      int advantage = 0;

      if (target is not null)
      {
        if (isRangedSpell.ToBool())
        {
          advantage += GetHighGroundAdvantage(attacker, target);
          advantage += GetThreatenedDisadvantage(attacker);
        }

        advantage += GetKnockdownAdvantage(isRangedSpell, target);
        advantage += GetAttackerAdvantageEffects(attacker, target, spellCastingAbility);
        advantage += GetTargetAdvantageEffects(target);
        advantage += GetInvisibleTargetDisadvantage(attacker, target);
        advantage += GetInvisibleAttackerAdvantage(attacker, target);
        advantage += GetDiversionTargetAdvantage(attacker, target);
        advantage += GetFeinteAttackerAdvantage(attacker);
        advantage += GetMetallicArmorAdvantage(target, spell);
      }

      return advantage;
    }
  }
}
