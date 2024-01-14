using Anvil.API;
using NWN.Native.API;
using NWN.Systems;

namespace NWN
{
  public static partial class CreatureUtils
  {
    public static bool GetDrowLightSensitivityDisadvantage(CGameEffect eff)
    {
      if(eff.m_sCustomTag.CompareNoCase(EffectSystem.lightSensitivityEffectExoTag).ToBool())
      {
        LogUtils.LogMessage("Désavantage - Drow en pleine lumière", LogUtils.LogType.Combat);
        return true;
      }
      else 
        return false;
    }
  }
}
