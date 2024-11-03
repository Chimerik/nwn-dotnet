using Anvil.API;
using NWN.Native.API;

namespace NWN.Systems
{
  public static partial class CreatureUtils
  {
    public static bool GetEspritEveilleDisadvantage(CGameEffect eff, uint targetId)
    {
      if(eff.m_sCustomTag.ToExoLocString().GetSimple(0).ComparePrefixNoCase(EffectSystem.EspritEveilleEffectExoTag, EffectSystem.EspritEveilleEffectExoTag.GetLength()).ToBool()
        && eff.m_oidCreator == targetId)
      {
        LogUtils.LogMessage("Désavantage - Combattant Clairvoyant", LogUtils.LogType.Combat);
        return true;
      }
      else
        return false;        
    }
  }
}
