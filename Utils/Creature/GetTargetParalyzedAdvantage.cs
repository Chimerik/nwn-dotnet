using NWN.Native.API;

namespace NWN
{
  public static partial class CreatureUtils
  {
    public static bool GetTargetParalyzedAdvantage(CGameEffect eff)
    {
      return (EffectTrueType)eff.m_nType == EffectTrueType.SetState && eff.GetInteger(0) == 8;
    }
  }
}
