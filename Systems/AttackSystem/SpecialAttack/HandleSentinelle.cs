using Anvil.API;
using NWN.Core;
using NWN.Native.API;

namespace NWN.Systems
{
  public static partial class NativeUtils
  {
    public static void HandleSentinelle(CNWSCreature attacker, CNWSCreature target, CNWSCombatRound combatRound)
    {
      if (target.m_pStats.HasFeat(CustomSkill.Sentinelle).ToBool())
        return;

      foreach(var eff in attacker.m_appliedEffects)
      {
        if(eff.m_sCustomTag.CompareNoCase(EffectSystem.threatenedEffectExoTag).ToBool())
        {
          CNWSCreature creature = NWNXLib.AppManager().m_pServerExoApp.GetCreatureByGameObjectID(eff.m_oidCreator);

          if (creature is null || !creature.m_pStats.HasFeat(CustomSkill.Sentinelle).ToBool()
            || creature.m_ScriptVars.GetInt(CreatureUtils.ReactionVariableExo) < 1)
            continue;

          switch ((Action)creature.m_nCurrentAction)
          {
            case Action.AttackObject:
              creature.m_ScriptVars.SetInt(CreatureUtils.SentinelleOpportunityVariableExo, 1);
              creature.m_ScriptVars.SetObject(CreatureUtils.SentinelleOpportunityTargetVariableExo, attacker.m_idSelf);
              creature.m_ScriptVars.SetInt(CreatureUtils.ReactionVariableExo, creature.m_ScriptVars.GetInt(CreatureUtils.ReactionVariableExo) - 1);

              SendNativeServerMessage("Sentinelle".ColorString(StringUtils.gold), creature);
              break;

            case Action.Wait:
            case Action.Invalid:
              creature.m_ScriptVars.SetInt(CreatureUtils.SentinelleOpportunityVariableExo, 1);
              creature.m_ScriptVars.SetObject(CreatureUtils.SentinelleOpportunityTargetVariableExo, attacker.m_idSelf);
              creature.m_ScriptVars.SetInt(CreatureUtils.ReactionVariableExo, creature.m_ScriptVars.GetInt(CreatureUtils.ReactionVariableExo) - 1);
              creature.AddAttackActions(attacker.m_idSelf, 1, 1, 1);

              SendNativeServerMessage("Sentinelle".ColorString(StringUtils.gold), creature);
              break;
          }
        }
      }  
    }
  }
}
