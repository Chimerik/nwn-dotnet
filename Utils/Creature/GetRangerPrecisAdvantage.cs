using Anvil.API;
using NWN.Native.API;

namespace NWN.Systems
{
  public static partial class CreatureUtils
  {
    public static bool GetRangerPrecisAdvantage(CGameEffect eff, CNWSCreature attacker)
    {
      if(attacker.m_pStats.HasFeat(CustomSkill.RangerPrecis).ToBool() 
        && eff.m_sCustomTag.CompareNoCase(EffectSystem.MarqueDuChasseurExoTag).ToBool()
        && eff.m_oidCreator == attacker.m_idSelf)
      {
        LogUtils.LogMessage("Avantage - Chasseur Précis", LogUtils.LogType.Combat);
        return true;
      }
      else
        return false;
    }
  }
}
