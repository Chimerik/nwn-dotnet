using NWN.Native.API;

namespace NWN.Systems
{
  public static partial class CreatureUtils
  {
    public static bool GetTargetAdvantageEffects(CNWSCreature target, bool rangedAttack)
    {
      foreach (var eff in target.m_appliedEffects)
      {
        if (target.m_pStats.GetClassLevel((byte)ClassType.Rogue) < 18)
        {
          if (GetKnockdownMeleeAdvantage(eff, rangedAttack))
            return true;

          if (GetTargetStunnedAdvantage(eff))
            return true;

          if (GetTargetParalyzedAdvantage(eff))
            return true;

          if (GetTargetPetrifiedAdvantage(eff))
            return true;
        }

        if(GetTargetBlindedAdvantage(eff))
          return true;

        if (GetTargetFaerieFireAdvantage(eff))
          return true;

        if (GetAgainstRecklessAttackAdvantage(eff))
          return true;

        if(GetWolfTotemAttackAdvantage(eff))
          return true;
      }

      return false;
    }
  }
}
