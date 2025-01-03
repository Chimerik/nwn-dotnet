﻿using Anvil.API;

namespace NWN.Systems
{
  public static partial class CreatureUtils
  {
    public static bool GetAttackerDisadvantageEffects(Native.API.CNWSCreature attacker, Native.API.CNWSCreature targetId, Ability attackStat)
    {
      foreach (var eff in attacker.m_appliedEffects)
      {
        if (GetMoquerieVicieuseDisadvantage(attacker, eff))
          return true;

        if (GetArmorShieldDisadvantage(eff, attackStat))
          return true;

        if (GetBoneChillDisadvantage(attacker, eff))
          return true;

        if (GetBlindedDisadvantage(attacker, targetId, eff))
          return true;

        if (GetPoisonedDisadvantage(eff))
          return true;

        if (GetDrowLightSensitivityDisadvantage(eff))
          return true;

        if (GetPourfendeurDisadvantage(eff))
          return true;

        if (GetEntraveDisadvantage(eff))
          return true;

        if (GetAffaiblissementDisadvantage(attacker, eff))
          return true;

        if (GetFrightenedDisadvantage(eff, targetId.m_idSelf))
          return true;

        if (GetDuelForceDisadvantage(eff, targetId))
          return true;

        if (GetProvocationDisadvantage(eff, targetId))
          return true;

        if (GetCourrouxDeLaNatureAttackDisadvantage(eff))
          return true;

        if (GetEspritEveilleDisadvantage(eff, targetId.m_idSelf))
          return true;
      }

      return false;
    }
  }
}
