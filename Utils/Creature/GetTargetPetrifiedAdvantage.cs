using NWN.Native.API;

namespace NWN.Systems
{
  public static partial class CreatureUtils
  {
    public static bool GetTargetPetrifiedAdvantage(CGameEffect eff)
    {
      if ((EffectTrueType)eff.m_nType == EffectTrueType.Petrify)
      {
        LogUtils.LogMessage("Avantage - Cible pétrifiée", LogUtils.LogType.Combat);
        return true;
      }
      else 
        return false;
    }
  }
}
