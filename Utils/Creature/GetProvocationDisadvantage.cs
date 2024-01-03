using Anvil.API;
using NWN.Native.API;
using NWN.Systems;

namespace NWN
{
  public static partial class CreatureUtils
  {
    public static bool GetProvocationDisadvantage(CGameEffect eff, uint targetId)
    {
      return eff.m_sCustomTag.ToExoLocString().GetSimple(0).ComparePrefixNoCase(EffectSystem.provocationEffectExoTag, EffectSystem.provocationEffectExoTag.GetLength()).ToBool()
        && eff.m_oidCreator != targetId;        
    }
  }
}
