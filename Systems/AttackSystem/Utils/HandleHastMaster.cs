using Anvil.API;
using NWN.Core;
using NWN.Native.API;

namespace NWN.Systems
{
  public static partial class NativeUtils
  {
    public static void HandleHastMaster(CNWSCreature attacker, CNWSObject target, CNWSCombatRound combatRound)
    {
      if (combatRound.GetCurrentAttackWeapon() is null || !attacker.m_pStats.HasFeat(CustomSkill.HastMaster).ToBool())
        return;

      uint opportunityTargetId = attacker.m_ScriptVars.GetObject(CreatureUtils.HastMasterOpportunityVariableExo);
      if (opportunityTargetId > 0 && opportunityTargetId != NWScript.OBJECT_INVALID)
      {
        combatRound.AddCleaveAttack(attacker.m_ScriptVars.GetObject(CreatureUtils.HastMasterOpportunityVariableExo));
        attacker.m_ScriptVars.DestroyObject(CreatureUtils.HastMasterOpportunityVariableExo);
      }

      if (!attacker.m_ScriptVars.GetInt(CreatureUtils.HastMasterCooldownVariableExo).ToBool())
      {
        switch (combatRound.GetCurrentAttackWeapon().m_nBaseItem)
        {
          case (uint)BaseItemType.Halberd:
          case (uint)BaseItemType.ShortSpear:
          case (uint)BaseItemType.Quarterstaff:
          case (uint)BaseItemType.Whip:
            attacker.m_ScriptVars.SetInt(CreatureUtils.HastMasterSpecialAttackExo, 1);
            attacker.m_ScriptVars.SetInt(CreatureUtils.HastMasterCooldownVariableExo, 1);
            combatRound.AddCleaveAttack(target.m_idSelf);
            break;
        }
      }
    }
  }
}
