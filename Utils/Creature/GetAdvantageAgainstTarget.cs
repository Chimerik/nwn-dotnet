using Anvil.API;
using NWN.Native.API;
using Ability = Anvil.API.Ability;
using BaseItemType = Anvil.API.BaseItemType;

namespace NWN.Systems
{
  public static partial class CreatureUtils
  {
    // 1 => Advantage
    // 0 = Neutral
    // -1 => Disadvantage
    public static int GetAdvantageAgainstTarget(CNWSCreature attacker, CNWSCombatAttackData attackData, CNWSItem attackWeapon, Ability attackStat, CNWSCreature target)
    {
      BaseItemType weaponType = attackWeapon is not null ? NwBaseItem.FromItemId((int)attackWeapon.m_nBaseItem).ItemType : BaseItemType.Invalid;

      bool advantage = HasAttackAdvantage(attacker, attackData, weaponType, attackStat, target);
      bool disadvantage = HasAttackDisadvantage(attacker, attackData, attackWeapon, weaponType, attackStat, target);

      if(advantage)
      {
        if (disadvantage)
          return 0;

        return 1;
      }

      if (disadvantage)
        return -1;

      return 0;
    }
    public static int GetAdvantageAgainstTarget(NwCreature attacker, NwSpell spell, int isRangedSpell, NwCreature target, Ability spellCastingAbility)
    {
      bool advantage = GetSpellAttackAdvantageAgainstTarget(attacker, spell, isRangedSpell, target, spellCastingAbility);
      bool disadvantage = GetSpellAttackDisadvantageAgainstTarget(attacker, spell, isRangedSpell, target, spellCastingAbility);

      if (advantage)
      {
        if (disadvantage)
          return 0;

        return 1;
      }

      if (disadvantage)
        return -1;

      return 0;
    }
  }
}
