

using NWN.Native.API;

namespace NWN
{
  public static partial class CreatureUtils
  {
    public static bool GetPoisonedDisadvantage(CGameEffect eff)
    {
      return (EffectTrueType)eff.m_nType == EffectTrueType.Poison;
    }
  }
}
