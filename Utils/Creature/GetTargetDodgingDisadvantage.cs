using Anvil.API;
using NWN.Native.API;
using NWN.Systems;

namespace NWN
{
  public static partial class CreatureUtils
  {
    public static bool GetTargetDodgingDisadvantage(CGameEffect eff)
    {
      return eff.m_sCustomTag.ToExoLocString().GetSimple(0).ComparePrefixNoCase(EffectSystem.DodgeEffectExoTag, EffectSystem.DodgeEffectExoTag.GetLength()) > 0
        ? true : false;     
    }
  }
}
