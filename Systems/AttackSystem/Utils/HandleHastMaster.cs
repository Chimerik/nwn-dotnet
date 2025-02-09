using Anvil.API;
using NWN.Core;
using NWN.Native.API;

namespace NWN.Systems
{
  public static partial class NativeUtils
  {
    public static void HandleHastMaster(CNWSCreature attacker, CNWSObject target, CNWSCombatRound combatRound, string attackerName)
    {
      if (combatRound.GetCurrentAttackWeapon() is null || !attacker.m_pStats.HasFeat(CustomSkill.HastMaster).ToBool())
        return;

      uint opportunityTargetId = attacker.m_ScriptVars.GetObject(CreatureUtils.HastMasterOpportunityVariableExo);
      if (opportunityTargetId > 0 && opportunityTargetId != NWScript.OBJECT_INVALID)
      {
        string targetName = $"{target.GetFirstName().GetSimple(0)} {target.GetLastName().GetSimple(0)}".ColorString(ColorConstants.Cyan);
        BroadcastNativeServerMessage($"Opportunité du maître d'Hast {attackerName.ColorString(ColorConstants.Cyan)} contre {targetName}", attacker);

        combatRound.AddWhirlwindAttack(attacker.m_ScriptVars.GetObject(CreatureUtils.HastMasterOpportunityVariableExo), 1);
        attacker.m_ScriptVars.DestroyObject(CreatureUtils.HastMasterOpportunityVariableExo);
        attacker.m_ScriptVars.SetInt(CreatureUtils.SentinelleOpportunityVariableExo, 1);

        LogUtils.LogMessage($"Attaque supplémentaire - Maître d'Hast", LogUtils.LogType.Combat);
      }

      if (!attacker.m_ScriptVars.GetInt(CreatureUtils.HastMasterCooldownVariableExo).ToBool())
      {
        switch (combatRound.GetCurrentAttackWeapon().m_nBaseItem)
        {
          case (uint)BaseItemType.Halberd:
          case (uint)BaseItemType.ShortSpear:
          case (uint)BaseItemType.Quarterstaff:

            string targetName = $"{target.GetFirstName().GetSimple(0)} {target.GetLastName().GetSimple(0)}".ColorString(ColorConstants.Cyan);
            BroadcastNativeServerMessage($"Attaque spéciale du maître d'Hast {attackerName.ColorString(ColorConstants.Cyan)} contre {targetName}", attacker);

            attacker.m_ScriptVars.SetInt(CreatureUtils.HastMasterSpecialAttackExo, 1);
            attacker.m_ScriptVars.SetInt(CreatureUtils.HastMasterCooldownVariableExo, 1);
            combatRound.AddCleaveAttack(target.m_idSelf);

            LogUtils.LogMessage($"Attaque supplémentaire - Maître d'Hast", LogUtils.LogType.Combat);
            break;
        }
      }
    }
  }
}
