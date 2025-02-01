using System.Linq;
using Anvil.API;
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
        if(eff.m_sCustomTag.ToString() == EffectSystem.ThreatenedEffectTag)
        {
          CNWSCreature creature = NWNXLib.AppManager().m_pServerExoApp.GetCreatureByGameObjectID(eff.m_oidCreator);

          if (creature is null || !creature.m_pStats.HasFeat(CustomSkill.Sentinelle).ToBool())
            continue;

          var reaction = creature.m_appliedEffects.FirstOrDefault(e => e.m_sCustomTag.ToString() == EffectSystem.ReactionEffectTag);

          if (reaction is null)
            continue;

          switch ((Action)creature.m_nCurrentAction)
          {
            case Action.AttackObject:
              creature.m_ScriptVars.SetInt(CreatureUtils.SentinelleOpportunityVariableExo, 1);
              creature.m_ScriptVars.SetObject(CreatureUtils.SentinelleOpportunityTargetVariableExo, attacker.m_idSelf);
              creature.RemoveEffect(reaction);

              SendNativeServerMessage("Sentinelle".ColorString(StringUtils.gold), creature);
              break;

            case Action.Wait:
            case Action.Invalid:
              creature.m_ScriptVars.SetInt(CreatureUtils.SentinelleOpportunityVariableExo, 1);
              creature.m_ScriptVars.SetObject(CreatureUtils.SentinelleOpportunityTargetVariableExo, attacker.m_idSelf);
              creature.RemoveEffect(reaction);
              creature.AddAttackActions(attacker.m_idSelf, 1, 1, 1);

              SendNativeServerMessage("Sentinelle".ColorString(StringUtils.gold), creature);
              break;
          }
        }
      }  
    }
  }
}
