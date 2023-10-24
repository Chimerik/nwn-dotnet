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

      advantage += GetHighGroundAdvantage(attacker, attackData.m_bRangedAttack, target);
      advantage += GetKnockdownAdvantage(attackData.m_bRangedAttack, target);
      advantage += GetAttackerAdvantageEffects(attacker, target, attackStat);
      advantage += GetTargetAdvantageEffects(target);
      advantage += GetSmallCreaturesHeavyWeaponDisadvantage(attacker, weaponType);
      advantage += GetRangedWeaponDistanceDisadvantage(attacker, attackData.m_bRangedAttack, weaponType, target);
      advantage += GetThreatenedDisadvantage(attacker, attackData.m_bRangedAttack);
      advantage += GetInvisibleTargetDisadvantage(attacker, target);
      advantage += GetInvisibleAttackerAdvantage(attacker, target);

      return advantage;
    }
    public static int GetSpellAttackAdvantageAgainstTarget(NwCreature attacker, int isRangedSpell, NwCreature target, Ability spellCastingAbility)
    {
      int advantage = 0;

      advantage += GetHighGroundAdvantage(attacker, isRangedSpell, target);
      advantage += GetKnockdownAdvantage(isRangedSpell, target);
      advantage += GetAttackerAdvantageEffects(attacker, target, spellCastingAbility);
      advantage += GetTargetAdvantageEffects(target);
      advantage += GetThreatenedDisadvantage(attacker, isRangedSpell);
      advantage += GetInvisibleTargetDisadvantage(attacker, target);
      advantage += GetInvisibleAttackerAdvantage(attacker, target);

      return advantage;
    }
  }
}
