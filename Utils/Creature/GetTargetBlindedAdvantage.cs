

using NWN.Native.API;

namespace NWN
{
  public static partial class CreatureUtils
  {
    public static bool GetTargetBlindedAdvantage(CGameEffect eff)
    {
      if((EffectTrueType)eff.m_nType == EffectTrueType.Blindness || (EffectTrueType)eff.m_nType == EffectTrueType.Darkness)
      {
        LogUtils.LogMessage("Avantage - Cible aveuglée", LogUtils.LogType.Combat);
        return true;
      }
      else
        return false;
    }
  }
}
