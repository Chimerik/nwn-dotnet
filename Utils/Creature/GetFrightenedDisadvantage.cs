using Anvil.API;
using NWN.Native.API;
using NWN.Systems;

namespace NWN
{
  public static partial class CreatureUtils
  {
    public static bool GetFrightenedDisadvantage(CGameEffect eff, uint targetId)
    {
      if(eff.m_sCustomTag.ToExoLocString().GetSimple(0).ComparePrefixNoCase(EffectSystem.frightenedEffectExoTag, EffectSystem.frightenedEffectExoTag.GetLength()).ToBool()
        && eff.m_oidCreator == targetId)
      {
        LogUtils.LogMessage("Désavantage - Attaquant effrayé par la cible", LogUtils.LogType.Combat);
        return true;
      }
      else
        return false;        
    }
  }
}
