﻿using System.Collections.Generic;
using Anvil.API;
using NWN.Native.API;

namespace NWN.Systems
{
  public static partial class NativeUtils
  {
    public static int GetMonkParadeDamageReduction(CNWSCreature target, CGameEffect eff, List<string> noStack)
    {
      noStack.Add(EffectSystem.MonkParadeEffectTag);

      if (target is null)
        return 0;

      int monkLevel = target.m_pStats.GetNumLevelsOfClass(CustomClass.Monk);

      if (monkLevel > 2)
      {
        target.RemoveEffect(eff);

        int roll = NwRandom.Roll(Utils.random, 10);
        int dexBonus = GetAbilityModifier(target, Anvil.API.Ability.Dexterity);
        int damageReduction = roll + dexBonus + monkLevel;

        LogUtils.LogMessage($"Parade - Réduction de dégâts : {roll} (1d10) + {dexBonus} + {monkLevel} = {damageReduction}", LogUtils.LogType.Combat);
        BroadcastNativeServerMessage($"Parade - Réduction de dégâts : {roll} (1d10) + {dexBonus} + {monkLevel} = {damageReduction}", target);
        return damageReduction;
      }

      return 0;
    }
  }
}
