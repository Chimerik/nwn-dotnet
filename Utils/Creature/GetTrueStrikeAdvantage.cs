﻿using Anvil.API;

namespace NWN.Systems
{
  public static partial class CreatureUtils
  {
    public static bool GetTrueStrikeAdvantage(Native.API.CGameEffect eff)
    {
      if(eff.m_sCustomTag.CompareNoCase(EffectSystem.trueStrikeEffectExoTag).ToBool())
      {
        LogUtils.LogMessage("Avantage - Coup au But", LogUtils.LogType.Combat);
        return true;
      }
      else
        return false;
    }
  }
}
