using Anvil.API;
using NWN.Systems;

namespace NWN
{
  public static partial class CreatureUtils
  {
    public static bool GetRecklessAttackAdvantage(Native.API.CGameEffect eff)
    {
      if(eff.m_sCustomTag.CompareNoCase(EffectSystem.RecklessAttackEffectExoTag).ToBool())
      {
        LogUtils.LogMessage("Avantage - Frappe Téméraire", LogUtils.LogType.Combat);
        return true;
      }
      else
        return false;
    }
  }
}
