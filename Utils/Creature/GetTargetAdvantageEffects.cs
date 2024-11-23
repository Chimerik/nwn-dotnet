using NWN.Native.API;

namespace NWN.Systems
{
  public static partial class CreatureUtils
  {
    public static bool GetTargetAdvantageEffects(CNWSCreature target, CNWSCreature attacker, bool rangedAttack)
    {
      foreach (var eff in target.m_appliedEffects)
      {
        if (GetKnockdownMeleeAdvantage(eff, rangedAttack))
          return true;

        if (GetRangerPrecisAdvantage(eff, attacker))
          return true;

        if (GetTargetStunnedAdvantage(eff))
          return true;

        if (GetTargetFrappeEtourdissanteAdvantage(eff))
          return true;

        if (GetTargetParalyzedAdvantage(eff))
          return true;

        if (GetTargetPetrifiedAdvantage(eff))
          return true;

        if(GetTargetBlindedAdvantage(eff))
          return true;

        if (GetTargetFaerieFireAdvantage(eff))
          return true;

        if (GetAgainstRecklessAttackAdvantage(eff))
          return true;

        if(GetWolfTotemAttackAdvantage(eff))
          return true;

        if (GetMauvaisAugureAttackAdvantage(eff))
          return true;

        if (GetCourrouxDeLaNatureAttackAdvantage(eff))
          return true;

        if (GetPolyvalentTricksterAdvantage(eff, attacker))
          return true;

        if (GetVoeudHostiliteAdvantage(eff, attacker))
          return true;

        if (GetRepliqueInvoqueeAdvantage(eff))
          return true;
      }

      return false;
    }
  }
}
