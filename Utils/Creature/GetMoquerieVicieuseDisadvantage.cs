using Anvil.API;
using NWN.Native.API;

namespace NWN.Systems
{
  public static partial class CreatureUtils
  {
    public static bool GetMoquerieVicieuseDisadvantage(CNWSCreature creature, CGameEffect eff)
    {
      if(eff.m_sCustomTag.CompareNoCase(EffectSystem.MoquerieVicieuseEffectExoTag).ToBool())
      {
        LogUtils.LogMessage("Désavantage - Moquerie Vicieuse", LogUtils.LogType.Combat);
        creature.RemoveEffect(eff);
        return true;
      }
      else
        return false;
    }
  }
}
