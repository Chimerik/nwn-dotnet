using NWN.Native.API;
using NWN.Systems;

namespace NWN
{
  public static partial class CreatureUtils
  {
    public static bool GetTargetFaerieFireAdvantage(CGameEffect eff)
    {
      return eff.m_sCustomTag.CompareNoCase(EffectSystem.faerieFireEffectExoTag) > 0;
    }
  }
}
