﻿using Anvil.API;
using NWN.Native.API;

namespace NWN.Systems
{
  public static partial class NativeUtils
  {
    public static int HandleAttaquePreciseUsed(CNWSCreature creature, int superiorityDiceBonus)
    {
      creature.m_ScriptVars.DestroyInt(CreatureUtils.ManoeuvreTypeVariableExo);
      creature.m_ScriptVars.DestroyInt(CreatureUtils.ManoeuvreDiceVariableExo);

      LogUtils.LogMessage($"Activation attaque précise : +{superiorityDiceBonus}", LogUtils.LogType.Combat);
      BroadcastNativeServerMessage($"Attaque précise (+{StringUtils.ToWhitecolor(superiorityDiceBonus)})".ColorString(StringUtils.gold), creature);

      return superiorityDiceBonus;
    }
  }
}
