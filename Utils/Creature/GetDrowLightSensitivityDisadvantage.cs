﻿using Anvil.API;
using NWN.Native.API;
using NWN.Systems;

namespace NWN
{
  public static partial class CreatureUtils
  {
    public static bool GetDrowLightSensitivityDisadvantage(CGameEffect eff)
    {
      return eff.m_sCustomTag.CompareNoCase(EffectSystem.lightSensitivityEffectExoTag).ToBool();
    }
  }
}
