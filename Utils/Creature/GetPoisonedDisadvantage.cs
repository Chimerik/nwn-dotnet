using NWN.Native.API;

namespace NWN.Systems
{
  public static partial class CreatureUtils
  {
    public static bool GetPoisonedDisadvantage(CGameEffect eff)
    {
      if((EffectTrueType)eff.m_nType == EffectTrueType.Poison)
      {
        LogUtils.LogMessage("Désavantage - Attaquant empoisonné", LogUtils.LogType.Combat);
        return true;
      }
      else 
        return false;
    }
  }
}
