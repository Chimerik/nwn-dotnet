

using NWN.Native.API;

namespace NWN
{
  public static partial class CreatureUtils
  {
    public static bool GetTargetBlindedAdvantage(CGameEffect eff)
    {
      return (EffectTrueType)eff.m_nType == EffectTrueType.Blindness 
        || (EffectTrueType)eff.m_nType == EffectTrueType.Darkness;
    }
  }
}
