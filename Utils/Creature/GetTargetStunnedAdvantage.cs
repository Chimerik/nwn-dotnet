using NWN.Native.API;

namespace NWN.Systems
{
  public static partial class CreatureUtils
  {
    public static bool GetTargetStunnedAdvantage(CGameEffect eff)
    {
      if((EffectTrueType)eff.m_nType == EffectTrueType.SetState && eff.GetInteger(0) == 6)
      {
        LogUtils.LogMessage("Avantage - Cible étourdie", LogUtils.LogType.Combat);
        return true;
      }
      else
        return false;
    }
  }
}
