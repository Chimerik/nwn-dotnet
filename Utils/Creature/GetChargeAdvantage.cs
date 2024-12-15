using Anvil.API;

namespace NWN.Systems
{
  public static partial class CreatureUtils
  {
    public static bool GetChargeAdvantage(Native.API.CGameEffect eff)
    {
      if(eff.m_sCustomTag.CompareNoCase(EffectSystem.ChargeDebuffEffectExoTag).ToBool())
      {
        LogUtils.LogMessage("Avantage - Charge", LogUtils.LogType.Combat);
        return true;
      }
      else
        return false;
    }
  }
}
