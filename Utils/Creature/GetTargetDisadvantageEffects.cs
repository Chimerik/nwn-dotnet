using NWN.Native.API;
using NWN.Systems;

namespace NWN.Systems
{
  public static partial class CreatureUtils
  {
    public static bool GetTargetDisadvantageEffects(CNWSCreature target, bool rangedAttack, CNWSCombatAttackData data = null)
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
      }

      return false;
    }
  }
}
