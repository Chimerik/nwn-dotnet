
using Anvil.API;
using NWN.Native.API;

namespace NWN.Systems
{
  public static partial class CreatureUtils
  {
    public static bool GetViseeStableAdvantage(CGameEffect eff, CNWSCreature attacker)
    {
      if(eff.m_sCustomTag.CompareNoCase(EffectSystem.ViseeStableEffectExoTag).ToBool())
      {
        EffectUtils.RemoveTaggedEffect(attacker, EffectSystem.ViseeStableEffectExoTag);
        LogUtils.LogMessage("Avantage - Visee Stable", LogUtils.LogType.Combat);
        return true;
      }
      else
        return false;
    }
  }
}
