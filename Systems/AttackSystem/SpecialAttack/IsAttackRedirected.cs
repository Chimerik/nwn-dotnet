﻿using NWN.Native.API;

namespace NWN.Systems
{
  public static partial class NativeUtils
  {
    public static bool IsAttackRedirected(CNWSCreature attacker, CNWSCreature target, CNWSCombatRound combatRound, string attackerName, string targetName)
    {
      if (IsIllusionDouble(attacker, target, combatRound, attackerName, targetName))
        return true;

      if (IsConspirateurRedirection(attacker, target, combatRound, attackerName))
        return true;

      if (IsEnchanteurRedirection(attacker, target, combatRound, attackerName))
        return true;

      return false;
    }
  }
}
