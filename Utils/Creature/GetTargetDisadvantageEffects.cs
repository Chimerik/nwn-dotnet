using NWN.Native.API;

namespace NWN.Systems
{
  public static partial class CreatureUtils
  {
    public static bool GetTargetDisadvantageEffects(CNWSCreature attacker, CNWSCreature target, bool rangedAttack, CNWSCombatAttackData data = null)
    {
      foreach (var eff in target.m_appliedEffects)
      {
        if(GetTargetDodgingDisadvantage(eff))
          return true;

        if (GetProtectionStyleDisadvantage(eff))
          return true;

        if(GetJeuDeJambeDisadvantage(eff))
          return true;

        if (GetPatienceDisadvantage(eff))
          return true;

        if (GetKnockdownRangedDisadvantage(eff, rangedAttack))
          return true;

        if (GetEspritAigleDisadvantage(eff, target, data))
          return true;

        if (GetProtectionContreLeMalEtLeBienDisadvantage(eff, attacker, target))
          return true;
      }

      return false;
    }
  }
}
