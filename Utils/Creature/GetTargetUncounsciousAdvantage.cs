﻿using NWN.Native.API;

namespace NWN.Systems
{
  public static partial class CreatureUtils
  {
    public static bool GetTargetUncounsciousAdvantage(CGameEffect eff)
    {
      if ((EffectTrueType)eff.m_nType == EffectTrueType.SetState && eff.GetInteger(0) == EffectState.Endormi)
      {
        LogUtils.LogMessage("Avantage - Cible inconsciente", LogUtils.LogType.Combat);
        return true;
      }
      else
        return false;
    }
  }
}
