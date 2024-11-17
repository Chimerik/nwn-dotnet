using Anvil.API;

namespace NWN.Systems
{
  public static partial class CreatureUtils
  {
    public static bool GetTargetFrappeEtourdissanteAdvantage(Native.API.CGameEffect eff)
    {
      if(eff.m_sCustomTag.CompareNoCase(EffectSystem.FrappeEtourdissanteEffectExoTag).ToBool())
      {
        LogUtils.LogMessage("Avantage - Frappe Etourdissante", LogUtils.LogType.Combat);
        return true;
      }
      else
        return false;
    }
  }
}
