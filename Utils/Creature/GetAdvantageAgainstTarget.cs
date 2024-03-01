using System.Linq;

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

        advantage += GetKnockdownAdvantage(attackData.m_bRangedAttack, target);
        advantage += GetAttackerAdvantageEffects(attacker, target, attackStat);
        bool invisibleTarget = GetInvisibleTargetDisadvantage(attacker, target);
        advantage += invisibleTarget ? -1 : 0;
        advantage += GetTargetAdvantageEffects(target, invisibleTarget);
        advantage += GetInvisibleAttackerAdvantage(attacker, target);
        advantage += GetDiversionTargetAdvantage(attacker, target);
        advantage += GetFeinteAttackerAdvantage(attacker);
      }

      advantage += GetSmallCreaturesHeavyWeaponDisadvantage(attacker, weaponType);
      advantage += GetSentinelleOpportunityAdvantage(attacker, attackData);

      if(target.m_pStats.GetClassLevel((byte)Native.API.ClassType.Rogue) > 17 && advantage > 0)
      {
        if (target.m_appliedEffects.Any(e => (EffectTrueType)e.m_nType == EffectTrueType.Knockdown
        || (EffectTrueType)e.m_nType == EffectTrueType.Petrify || (EffectTrueType)e.m_nType == EffectTrueType.Sanctuary
        || (EffectTrueType)e.m_nType == EffectTrueType.Timestop || (EffectTrueType)e.m_nType == EffectTrueType.Pacify
        || ((EffectTrueType)e.m_nType == EffectTrueType.SetState && (e.GetInteger(0) == 6 || e.GetInteger(0) == 1 || e.GetInteger(0) == 2 || e.GetInteger(0) == 3 || e.GetInteger(0) == 7 || e.GetInteger(0) == 8 || e.GetInteger(0) == 9))))
        {
          Systems.NativeUtils.BroadcastNativeServerMessage("Insaisissable".ColorString(ColorConstants.Silver), target);
          return 0;
        }
      }

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
        bool invisibleTarget = GetInvisibleTargetDisadvantage(attacker, target);
        advantage += invisibleTarget ? -1 : 0;
        advantage += GetTargetAdvantageEffects(target, invisibleTarget);
        advantage += GetInvisibleAttackerAdvantage(attacker, target);
        advantage += GetDiversionTargetAdvantage(attacker, target);
        advantage += GetFeinteAttackerAdvantage(attacker);
        advantage += GetMetallicArmorAdvantage(target, spell);
      }

      if (target.Classes.Any(c => c.Class.ClassType == Anvil.API.ClassType.Rogue && c.Level > 17) && advantage > 0)
        foreach(var eff in target.ActiveEffects)
          if(EffectUtils.IsIncapacitatingEffect(eff))
          {
            Systems.NativeUtils.BroadcastNativeServerMessage("Insaisissable".ColorString(ColorConstants.Silver), target);
            return 0;
          }

      return advantage;
    }
  }
}
