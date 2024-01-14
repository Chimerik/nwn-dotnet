using NWN.Native.API;

namespace NWN
{
  public static partial class CreatureUtils
  {
    public static bool GetTargetParalyzedAdvantage(CGameEffect eff)
    {
      if ((EffectTrueType)eff.m_nType == EffectTrueType.SetState && eff.GetInteger(0) == 8)
      {
        LogUtils.LogMessage("Avantage - Cible paralysée", LogUtils.LogType.Combat);
        return true;
      }
      else
        return false;
    }
  }
}
