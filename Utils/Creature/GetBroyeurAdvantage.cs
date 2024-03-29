﻿using Anvil.API;

namespace NWN.Systems
{
  public static partial class CreatureUtils
  {
    public static bool GetBroyeurAdvantage(Native.API.CGameEffect eff)
    {
      if(eff.m_sCustomTag.CompareNoCase(EffectSystem.BroyeurEffectExoTag).ToBool())
      {
        LogUtils.LogMessage("Avantage - Broyeur", LogUtils.LogType.Combat);
        return true;
      }
      else
        return false;
    }
  }
}
