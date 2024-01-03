using System.Numerics;
using Anvil.API;
using NWN.Native.API;

namespace NWN.Systems
{
  public static partial class NativeUtils
  {
    public static void HandleCogneurLourdBonusAttack(CNWSCreature attacker, CNWSObject target, CNWSCombatRound combatRound, CNWSCombatAttackData attackData, int damageDealt)
    {
      if (attackData.m_nAttackResult != 3 && target.m_nCurrentHitPoints > damageDealt)
        return;

      if (attacker.m_pStats.HasFeat(CustomSkill.CogneurLourd).ToBool() && combatRound.GetCurrentAttackWeapon() is not null
      && !attackData.m_bRangedAttack.ToBool() && attacker.m_ScriptVars.GetInt(Config.isBonusActionAvailableVariable).ToBool())
      {
        if (target.m_nCurrentHitPoints - damageDealt < 1)
        {
          foreach (var gameObject in attacker.GetArea().m_aGameObjects)
          {
            if (gameObject == attacker.m_idSelf || gameObject == target.m_idSelf)
              continue;

            var creature = NWNXLib.AppManager().m_pServerExoApp.GetCreatureByGameObjectID(gameObject);

            if (creature is null || creature.m_nCurrentHitPoints < 1)
              continue;

            if (creature.m_bPlayerCharacter > 0)
            {
              if (creature.GetPVPReputation(attacker.m_idSelf) > 49)
                continue;
            }
            else
            {
              if (attacker.GetCreatureReputation(creature.m_idSelf, creature.m_nOriginalFactionId, 0) > 49)
                continue;
            }

            if (Vector3.Distance(attacker.m_vPosition.ToManagedVector(), creature.m_vPosition.ToManagedVector()) > 3)
              continue;

            SendNativeServerMessage("Cogneur Lourd - Attaque supplémentaire".ColorString(StringUtils.gold), attacker);
            combatRound.AddCleaveAttack(target.m_idSelf);
            attacker.m_ScriptVars.SetInt(Config.isBonusActionAvailableVariable, attacker.m_ScriptVars.GetInt(Config.isBonusActionAvailableVariable) - 1);
          }
        }
        else
        {
          SendNativeServerMessage("Cogneur Lourd - Attaque supplémentaire".ColorString(StringUtils.gold), attacker);
          combatRound.AddCleaveAttack(target.m_idSelf);
          attacker.m_ScriptVars.SetInt(Config.isBonusActionAvailableVariable, attacker.m_ScriptVars.GetInt(Config.isBonusActionAvailableVariable) - 1);
        }
      }
    }
  }
}
