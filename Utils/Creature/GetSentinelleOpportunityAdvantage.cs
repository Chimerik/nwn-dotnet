using Anvil.API;
using NWN.Native.API;
using NWN.Systems;

namespace NWN
{
  public static partial class CreatureUtils
  {
    public static int GetSentinelleOpportunityAdvantage(CNWSCreature attacker, CNWSCombatAttackData data)
    {
      return attacker.m_pStats.HasFeat(CustomSkill.Sentinelle).ToBool() 
        && (attacker.m_ScriptVars.GetInt(SentinelleOpportunityVariableExo).ToBool() || data.m_nAttackType == 65002)
        ? 1 : 0;
    }
  }
}
