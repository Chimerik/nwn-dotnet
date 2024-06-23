using Anvil.API;

namespace NWN.Systems
{
  public static partial class CreatureUtils
  {
    public static bool GetAssassinateAdvantage(Native.API.CGameEffect eff)
    {
      if (eff.m_sCustomTag.CompareNoCase(EffectSystem.AssassinateExoTag).ToBool())
      {
        LogUtils.LogMessage("Avantage - Assassinat", LogUtils.LogType.Combat);
        return true;
      }
      else
        return false;
    }
  }
}
