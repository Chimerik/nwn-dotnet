using Anvil.API;
using NWN.Core;
using NWN.Native.API;

namespace NWN.Systems
{
  public static partial class NativeUtils
  {
    public static void HandleMonkOpportunist(CNWSCreature attacker, CNWSCreature target, CNWSCombatAttackData data, CNWSCombatRound combatRound, string attackerName, string targetName)
    {
      if (data.m_bRangedAttack.ToBool() && !attacker.m_pStats.HasFeat(CustomSkill.MonkOpportuniste).ToBool()
        && attacker.m_ScriptVars.GetInt(CreatureUtils.ReactionVariableExo) < 1)
      {
        var disturbedBy = target.m_ScriptVars.GetObject(CreatureUtils.OpportunisteVariableExo);
        if (disturbedBy != NWScript.OBJECT_INVALID && disturbedBy != attacker.m_idSelf)
        {
          BroadcastNativeServerMessage($"{attackerName.ColorString(ColorConstants.Cyan)} opportuniste contre {targetName.ColorString(ColorConstants.Cyan)}", attacker);

          combatRound.AddWhirlwindAttack(target.m_idSelf, 1);
          attacker.m_ScriptVars.SetInt(CreatureUtils.ReactionVariableExo, attacker.m_ScriptVars.GetInt(CreatureUtils.ReactionVariableExo) - 1);
          LogUtils.LogMessage($"Attaque supplémentaire - Opportuniste", LogUtils.LogType.Combat);
          return;
        }
      }

      target.m_ScriptVars.SetObject(CreatureUtils.OpportunisteVariableExo, attacker.m_idSelf);
    }
  }
}
