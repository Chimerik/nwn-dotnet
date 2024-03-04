using NWN.Native.API;

namespace NWN.Systems
{
  public static partial class CreatureUtils
  {
    public static bool GetBlindedDisadvantage(CGameEffect eff)
    {
      if ((EffectTrueType)eff.m_nType == EffectTrueType.Blindness || (EffectTrueType)eff.m_nType == EffectTrueType.Darkness)
      {
        LogUtils.LogMessage("Désavantage - Attaquant aveuglé ou subissant Ténèbres", LogUtils.LogType.Combat);
        return true;
      }
      else
        return false;
    }
  }
}
