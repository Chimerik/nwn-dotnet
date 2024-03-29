﻿using Anvil.API;
using NWN.Native.API;

namespace NWN.Systems
{
  public static partial class CreatureUtils
  {
    public static bool GetProtectionStyleDisadvantage(CGameEffect eff)
    {
      if (eff.m_sCustomTag.CompareNoCase(EffectSystem.ProtectionStyleEffectExoTag).ToBool())
      {
        LogUtils.LogMessage("Désavantage - Cible sous Protection (Guerrier)", LogUtils.LogType.Combat);
        return true;
      }
      else
        return false;
    }
  }
}
