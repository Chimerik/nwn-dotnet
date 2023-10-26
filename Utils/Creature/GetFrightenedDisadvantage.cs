using Anvil.API;
using NWN.Native.API;
using NWN.Systems;

namespace NWN
{
  public static partial class CreatureUtils
  {
    public static bool GetFrightenedDisadvantage(CGameEffect eff, uint targetId)
    {
      var compString = eff.m_sCustomTag.ToExoLocString().GetSimple(0);

      return compString.ComparePrefixNoCase(EffectSystem.frightenedEffectExoTag, EffectSystem.frightenedEffectExoTag.GetLength()) > 0 
        ? (uint)compString.Split(EffectSystem.exoDelimiter)[1].AsINT() == targetId 
        ? true : false
        : false;        
    }
  }
}
