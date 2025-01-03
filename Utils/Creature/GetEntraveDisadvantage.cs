﻿using Anvil.API;

namespace NWN.Systems
{
  public static partial class CreatureUtils
  {
    public static bool GetEntraveDisadvantage(Native.API.CGameEffect eff)
    {
      if(eff.m_sCustomTag.CompareNoCase(EffectSystem.EntraveEffectExoTag).ToBool())
      {
        LogUtils.LogMessage("Désavantage - Entravé", LogUtils.LogType.Combat);
        return true;
      }
      else
        return false;
    }
  }
}
