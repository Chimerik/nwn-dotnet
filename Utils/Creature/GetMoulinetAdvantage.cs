using Anvil.API;

namespace NWN.Systems
{
  public static partial class CreatureUtils
  {
    public static bool GetMoulinetAdvantage(Native.API.CGameEffect eff)
    {
      if(eff.m_sCustomTag.CompareNoCase(EffectSystem.MoulinetEffectExoTag).ToBool())
      {
        LogUtils.LogMessage("Avantage - Moulinet", LogUtils.LogType.Combat);
        return true;
      }
      else
        return false;
    }
  }
}
