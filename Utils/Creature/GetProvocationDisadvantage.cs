using Anvil.API;
using NWN.Native.API;

namespace NWN.Systems
{
  public static partial class CreatureUtils
  {
    public static bool GetProvocationDisadvantage(CGameEffect eff, uint targetId)
    {
      if (eff.m_sCustomTag.CompareNoCase(EffectSystem.provocationEffectExoTag).ToBool() && eff.m_oidCreator != targetId)
      {
        LogUtils.LogMessage("Désavantage - Affecté par Provocation", LogUtils.LogType.Combat);
        return true;
      }
      else
        return false;
    }
  }
}
