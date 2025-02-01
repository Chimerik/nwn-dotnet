using System.Linq;
using System.Numerics;
using Anvil.API;
using NWN.Core;
using NWN.Native.API;

namespace NWN.Systems
{
  public static partial class NativeUtils
  {
    public static void HandleMonkOpportunist(CNWSCreature attacker, CNWSCreature target, CNWSCombatAttackData data, CNWSCombatRound combatRound, string attackerName, string targetName)
    {
      if (data.m_bRangedAttack.ToBool())
        return;

      var opportunityTargetId = attacker.m_ScriptVars.GetObject(CreatureUtils.OpportunisteVariableExo);

      if (opportunityTargetId > 0 || opportunityTargetId != 0x7F000000)
      {
        var opportunityTarget = NWNXLib.AppManager().m_pServerExoApp.GetCreatureByGameObjectID(opportunityTargetId);
        
        if(opportunityTarget is not null && opportunityTarget.m_idSelf != 0x7F000000)
        {
          BroadcastNativeServerMessage($"{attackerName.ColorString(ColorConstants.Cyan)} opportuniste !", attacker);

          combatRound.AddWhirlwindAttack(opportunityTargetId, 1);
          LogUtils.LogMessage($"Attaque supplémentaire - Opportuniste", LogUtils.LogType.Combat);
        }

        attacker.m_ScriptVars.DestroyObject(CreatureUtils.OpportunisteVariableExo);
      }

      foreach (var eff in target.m_appliedEffects)
        if(eff.m_sCustomTag.CompareNoCase(EffectSystem.monkOpportunistEffectExoTag).ToBool())
        {
          if(eff.m_oidCreator != 0x7F000000)
          {
            if (eff.m_oidCreator == attacker.m_idSelf)
              continue;

            var opportunist = NWNXLib.AppManager().m_pServerExoApp.GetCreatureByGameObjectID(eff.m_oidCreator);

            if (opportunist is not null && opportunist.m_idSelf != 0x7F000000 && opportunist.m_oidArea == target.m_oidArea
              && opportunist.m_vPosition is not null
              && Vector3.DistanceSquared(opportunist.m_vPosition.ToManagedVector(), target.m_vPosition.ToManagedVector()) < 10)
            {
              var reaction = opportunist.m_appliedEffects.FirstOrDefault(e => e.m_sCustomTag.ToString() == EffectSystem.ReactionEffectTag);

              if (reaction is not null)
              {
                opportunist.RemoveEffect(reaction);
                opportunist.m_ScriptVars.SetObject(CreatureUtils.OpportunisteVariableExo, target.m_idSelf);
              }
            }
          }
          
          target.RemoveEffect(eff);
        }
    }
  }
}
