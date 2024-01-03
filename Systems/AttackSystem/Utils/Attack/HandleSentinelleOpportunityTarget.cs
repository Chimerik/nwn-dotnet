using NWN.Core;
using NWN.Native.API;

namespace NWN.Systems
{
  public static partial class NativeUtils
  {
    public static void HandleSentinelleOpportunityTarget(CNWSCreature attacker, CNWSCombatRound combatRound)
    {
      uint target = attacker.m_ScriptVars.GetObject(CreatureUtils.SentinelleOpportunityTargetVariableExo);

      if (target > 0 && target != NWScript.OBJECT_INVALID)
      {
        combatRound.AddCleaveAttack(target);
        attacker.m_ScriptVars.DestroyObject(CreatureUtils.SentinelleOpportunityTargetVariableExo);
      }
    }
  }
}
