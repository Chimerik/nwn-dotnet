using Anvil.API;

namespace NWN.Systems
{
  public static partial class CreatureUtils
  {
    public static bool GetAttackerAdvantageEffects(Native.API.CNWSCreature attacker, Native.API.CNWSCreature targetId, Ability attackStat)
    {
      foreach (var eff in attacker.m_appliedEffects)
      {
        if (GetTrueStrikeAdvantage(eff))
          return true;

        if (GetBroyeurAdvantage(eff))
          return true;

        if (GetRecklessAttackAdvantage(eff))
          return true;
      }

      return false;
    }
  }
}
