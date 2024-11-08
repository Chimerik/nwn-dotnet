using System.Linq;
using Anvil.API;
using NWN.Native.API;

namespace NWN.Systems
{
  public static partial class CreatureUtils
  {
    public static bool GetProvocationDisadvantage(CGameEffect eff, CNWSCreature target)
    {
      if (eff.m_sCustomTag.CompareNoCase(EffectSystem.provocationEffectExoTag).ToBool() 
        && !target.m_appliedEffects.Any(e => e.m_sCustomTag.CompareNoCase(EffectSystem.provoqueurEffectExoTag).ToBool()))
      {
        LogUtils.LogMessage("Désavantage - Affecté par Provocation", LogUtils.LogType.Combat);
        return true;
      }
      else
        return false;
    }
  }
}
