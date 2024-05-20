using Anvil.API;
using NWN.Native.API;

namespace NWN.Systems
{
  public static partial class CreatureUtils
  {
    public static bool GetPoisonedDisadvantage(CGameEffect eff)
    {
      if((EffectTrueType)eff.m_nType == EffectTrueType.Poison || eff.m_sCustomTag.CompareNoCase(EffectSystem.poisonEffectExoTag).ToBool())
      {
        LogUtils.LogMessage("Désavantage - Empoisonné", LogUtils.LogType.Combat);
        return true;
      }
      else 
        return false;
    }
  }
}
