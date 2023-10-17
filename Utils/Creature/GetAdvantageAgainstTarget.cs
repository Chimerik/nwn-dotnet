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
    public static int GetAdvantageAgainstTarget(CNWSCreature attacker, CNWSCombatAttackData attackData, CNWSItem attackWeapon, Ability attackStat, CExoString spellCastVariable, CNWSCreature target = null)
    {
      BaseItemType weaponType = attackWeapon is not null ? NwBaseItem.FromItemId((int)attackWeapon.m_nBaseItem).ItemType : BaseItemType.Invalid;
      int advantage = 0;

      advantage += GetHighGroundAdvantage(attacker, attackData, target);
      advantage += GetKnockdownAdvantage(attackData.m_bRangedAttack, target);
      advantage += GetAttackerAdvantageEffects(attacker, target.m_idSelf, attackStat);
      advantage += GetTargetAdvantageEffects(target);
      advantage += GetSmallCreaturesHeavyWeaponDisadvantage(attacker, weaponType);
      advantage += GetRangedWeaponDistanceDisadvantage(attacker, attackData, weaponType, target);
      advantage += GetThreatenedDisadvantage(attacker, attackData, spellCastVariable);
      advantage += GetInvisibleTargetDisadvantage(attacker, target);
      advantage += GetInvisibleAttackerAdvantage(attacker, target);


      return advantage;
    }
  }
}
