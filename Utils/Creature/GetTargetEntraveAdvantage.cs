﻿using Anvil.API;

namespace NWN.Systems
{
  public static partial class CreatureUtils
  {
    public static bool GetTargetEntraveAdvantage(Native.API.CGameEffect eff)
    {
      if(eff.m_sCustomTag.CompareNoCase(EffectSystem.EntraveEffectExoTag).ToBool())
      {
        LogUtils.LogMessage("Avantage - Cible Entravée", LogUtils.LogType.Combat);
        return true;
      }
      else
        return false;
    }
  }
}
