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
          //LogUtils.LogMessage($"GetHighGroundAdvantage : {advantage}", LogUtils.LogType.Combat);
          advantage += GetRangedWeaponDistanceDisadvantage(attacker, weaponType, target);
          //LogUtils.LogMessage($"GetRangedWeaponDistanceDisadvantage : {advantage}", LogUtils.LogType.Combat);
          advantage += GetThreatenedDisadvantage(attacker, attackWeapon);
          //LogUtils.LogMessage($"GetThreatenedDisadvantage : {advantage}", LogUtils.LogType.Combat);
        }

        advantage += GetKnockdownAdvantage(attackData.m_bRangedAttack, target);
        //LogUtils.LogMessage($"GetKnockdownAdvantage : {advantage}", LogUtils.LogType.Combat);
        advantage += GetAttackerAdvantageEffects(attacker, target, attackStat);
        //LogUtils.LogMessage($"GetAttackerAdvantageEffects : {advantage}", LogUtils.LogType.Combat);
        advantage += GetTargetAdvantageEffects(target);
        //LogUtils.LogMessage($"GetTargetAdvantageEffects : {advantage}", LogUtils.LogType.Combat);
        advantage += GetInvisibleTargetDisadvantage(attacker, target);
        //LogUtils.LogMessage($"GetInvisibleTargetDisadvantage : {advantage}", LogUtils.LogType.Combat);
        advantage += GetInvisibleAttackerAdvantage(attacker, target);
        //LogUtils.LogMessage($"GetInvisibleAttackerAdvantage : {advantage}", LogUtils.LogType.Combat);
        advantage += GetDiversionTargetAdvantage(attacker, target);
        //LogUtils.LogMessage($"GetDiversionTargetAdvantage : {advantage}", LogUtils.LogType.Combat);
      }

      advantage += GetSmallCreaturesHeavyWeaponDisadvantage(attacker, weaponType);
      //LogUtils.LogMessage($"GetSmallCreaturesHeavyWeaponDisadvantage : {advantage}", LogUtils.LogType.Combat);
      advantage += GetSentinelleOpportunityAdvantage(attacker, attackData);
      //LogUtils.LogMessage($"GetSentinelleOpportunityAdvantage : {advantage}", LogUtils.LogType.Combat);

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

        if (spell.SpellType == Spell.ElectricJolt)
          advantage += GetMetallicArmorAdvantage(target);
      }

      return advantage;
    }
  }
}
