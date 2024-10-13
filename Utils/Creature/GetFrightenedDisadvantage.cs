using Anvil.API;
using NWN.Native.API;

namespace NWN.Systems
{
  public static partial class CreatureUtils
  {
    public static bool GetFrightenedDisadvantage(CGameEffect eff, uint targetId)
    {
      if(eff.m_sCustomTag.ToExoLocString().GetSimple(0).ComparePrefixNoCase(EffectSystem.frightenedEffectExoTag, EffectSystem.frightenedEffectExoTag.GetLength()).ToBool())
      {
        LogUtils.LogMessage("Désavantage - Attaquant effrayé", LogUtils.LogType.Combat);
        return true;
      }
      else
        return false;        
    }
  }
}
