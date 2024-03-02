using Anvil.API;
using NWN.Native.API;
using NWN.Systems;

namespace NWN
{
  public static partial class CreatureUtils
  {
    public static bool GetSentinelleOpportunityAdvantage(CNWSCreature attacker, CNWSCombatAttackData data)
    {
      if (attacker.m_pStats.HasFeat(CustomSkill.Sentinelle).ToBool()
        && (attacker.m_ScriptVars.GetInt(SentinelleOpportunityVariableExo).ToBool() || data.m_nAttackType == 65002))
      {
        LogUtils.LogMessage("Avantage - Sentinelle (attaque d'opportunité)", LogUtils.LogType.Combat);
        return true;
      }
      else
        return false;
    }
  }
}
