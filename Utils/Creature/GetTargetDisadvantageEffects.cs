using NWN.Native.API;
using NWN.Systems;

namespace NWN
{
  public static partial class CreatureUtils
  {
    public static bool GetTargetDisadvantageEffects(CNWSCreature target, bool rangedAttack)
    {
      foreach (var eff in target.m_appliedEffects)
      {
        if(GetTargetDodgingDisadvantage(eff))
          return true;

        if (GetProtectionStyleDisadvantage(eff))
          return true;

        if(GetJeuDeJambeDisadvantage(eff))
          return true;

        if (GetKnockdownRangedDisadvantage(eff, rangedAttack))
          return true;
      }

      return false;
    }
  }
}
