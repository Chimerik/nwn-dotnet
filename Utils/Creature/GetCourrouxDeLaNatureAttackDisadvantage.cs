using Anvil.API;

namespace NWN.Systems
{
  public static partial class CreatureUtils
  {
    public static bool GetCourrouxDeLaNatureAttackDisadvantage(Native.API.CGameEffect eff)
    {
      if(eff.m_sCustomTag.CompareNoCase(EffectSystem.CourrouxDeLaNatureEffectExoTag).ToBool())
      {
        LogUtils.LogMessage("Désavantage - Affecté par Courroux de la Nature", LogUtils.LogType.Combat);
        return true;
      }
      else
        return false;
    }
  }
}
