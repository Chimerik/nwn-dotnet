using Anvil.API;

namespace NWN.Systems
{
  public static partial class CreatureUtils
  {
    public static bool GetCourrouxDeLaNatureAttackAdvantage(Native.API.CGameEffect eff)
    {
      if(eff.m_sCustomTag.CompareNoCase(EffectSystem.CourrouxDeLaNatureEffectExoTag).ToBool())
      {
        LogUtils.LogMessage("Avantage - Courroux de la Nature", LogUtils.LogType.Combat);
        return true;
      }
      else
        return false;
    }
  }
}
