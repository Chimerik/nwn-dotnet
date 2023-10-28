﻿using NWN.Native.API;
using NWN.Systems;

namespace NWN
{
  public static partial class CreatureUtils
  {
    public static bool GetTrueStrikeAdvantage(CGameEffect eff)
    {
      return eff.m_sCustomTag.CompareNoCase(EffectSystem.trueStrikeEffectExoTag) > 0;
    }
  }
}
