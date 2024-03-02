using Anvil.API;

namespace NWN
{
  public static partial class CreatureUtils
  {
    public static bool GetAttackerDisadvantageEffects(Native.API.CNWSCreature attacker, Native.API.CNWSCreature targetId, Ability attackStat)
    {
      foreach (var eff in attacker.m_appliedEffects)
      {
        if(GetArmorShieldDisadvantage(eff, attackStat))
          return true;

        if (GetBoneChillDisadvantage(attacker, eff))
          return true;

        if (GetBlindedDisadvantage(eff))
          return true;

        if (GetPoisonedDisadvantage(eff))
          return true;

        if (GetDrowLightSensitivityDisadvantage(eff))
          return true;

        if (GetPourfendeurDisadvantage(eff))
          return true;

        if (GetFrightenedDisadvantage(eff, targetId.m_idSelf))
          return true;

        if (GetProvocationDisadvantage(eff, targetId.m_idSelf))
          return true;
      }

      return false;
    }
  }
}
