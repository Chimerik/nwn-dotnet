﻿using Anvil.API;

namespace NWN.Systems
{
  public static partial class CreatureUtils
  {
    public static bool GetPourfendeurDisadvantage(Native.API.CGameEffect eff)
    {
      if(eff.m_sCustomTag.CompareNoCase(EffectSystem.PourfendeurDisadvantageEffectExoTag).ToBool())
      {
        LogUtils.LogMessage("Désavantage - Affecté par Pourfendeur", LogUtils.LogType.Combat);
        return true;
      }
      else
        return false;
    }
  }
}
