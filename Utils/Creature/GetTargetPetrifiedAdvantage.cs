using NWN.Native.API;

namespace NWN
{
  public static partial class CreatureUtils
  {
    public static bool GetTargetPetrifiedAdvantage(CGameEffect eff)
    {
      return (EffectTrueType)eff.m_nType == EffectTrueType.Petrify;
    }
  }
}
