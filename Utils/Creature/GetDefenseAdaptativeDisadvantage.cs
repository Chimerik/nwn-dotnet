using Anvil.API;
using NWN.Native.API;
using RacialType = NWN.Native.API.RacialType;

namespace NWN.Systems
{
  public static partial class CreatureUtils
  {
    public static bool GetDefenseAdaptativeDisadvantage(CGameEffect eff, CNWSCreature attacker, CNWSCreature target)
    {
      if (!eff.m_sCustomTag.CompareNoCase(EffectSystem.DefenseAdaptativeMalusEffectExoTag).ToBool()
        || eff.m_oidCreator == target.m_idSelf)
        return false;

      LogUtils.LogMessage("Désavantage - Défense Adaptative", LogUtils.LogType.Combat);
      return true;
    }
  }
}
