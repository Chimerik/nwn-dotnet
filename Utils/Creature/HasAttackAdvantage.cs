using System.Linq;
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
        // Si la cible est insaissible (Rogue 18) et n'est pas incapable d'agir, alors il est impossible d'avoir l'avantage sur elle
        if (RogueUtils.GetRogueLevel(target) > 17 && !EffectUtils.IsIncapacitated(target))
        {
          LogUtils.LogMessage("Cible insaississable - Pas d'avantage", LogUtils.LogType.Combat);
          return false;
        }
          if (rangedAttack && GetHighGroundAdvantage(attacker, target))
          return true;

        if (GetAttackerAdvantageEffects(attacker, target, attackStat, rangedAttack))
          return true;

        if (GetTargetAdvantageEffects(target, attacker, rangedAttack))
          return true;

        if (GetInvisibleAttackerAdvantage(attacker, target))
          return true;

        if (GetDiversionTargetAdvantage(attacker, target))
          return true;

        if (GetFeinteAttackerAdvantage(attacker))
          return true;

        if (GetWolfPackAttackerAdvantage(attacker, target))
          return true;

        if (GetGuerrierHeroiqueAdvantage(attacker))
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
        // Si la cible est insaissible (Rogue 18) et n'est pas incapable d'agir, alors il est impossible d'avoir l'avantage sur elle
        if (target.Classes.Any(c => Utils.In(c.Class.ClassType, Anvil.API.ClassType.Rogue, (Anvil.API.ClassType)CustomClass.RogueArcaneTrickster) && c.Level > 17) 
          && !EffectUtils.IsIncapacitated(target))
          return false;

        if (rangedSpell && GetHighGroundAdvantage(attacker, target))
          return true;

        if (GetAttackerAdvantageEffects(attacker, target, spellCastingAbility, rangedSpell, spell))
          return true;

        if (GetTargetAdvantageEffects(target, attacker, rangedSpell))
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
