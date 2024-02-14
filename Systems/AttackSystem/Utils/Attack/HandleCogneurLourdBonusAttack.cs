using System.Numerics;
using Anvil.API;
using NWN.Native.API;

namespace NWN.Systems
{
  public static partial class NativeUtils
  {
    public static void HandleCogneurLourdBonusAttack(CNWSCreature attacker, CNWSObject target, CNWSCombatRound combatRound, CNWSCombatAttackData attackData, int damageDealt, string attackerName)
    {
      if (attackData.m_nAttackResult != 3 && target.m_nCurrentHitPoints > damageDealt)
        return;

      if (attacker.m_pStats.HasFeat(CustomSkill.CogneurLourd).ToBool() && combatRound.GetCurrentAttackWeapon() is not null
      && !attackData.m_bRangedAttack.ToBool() && attacker.m_ScriptVars.GetInt(Config.isBonusActionAvailableVariable).ToBool())
      {
        if (target.m_nCurrentHitPoints - damageDealt < 1)
        {
          var newTarget = NWNXLib.AppManager().m_pServerExoApp.GetCreatureByGameObjectID(attacker.GetNearestEnemy(3, target.m_idSelf, 1, 1));

          if (newTarget is null || newTarget.m_idSelf == 0x7F000000) // OBJECT_INVALID
            return;

          string targetName = $"{newTarget.GetFirstName().GetSimple(0)} {newTarget.GetLastName().GetSimple(0)}".ColorString(ColorConstants.Cyan);
          BroadcastNativeServerMessage($"{attackerName.ColorString(ColorConstants.Cyan)} cogneur lourd contre {targetName}", attacker);

          combatRound.AddWhirlwindAttack(newTarget.m_idSelf, 1);
          attacker.m_ScriptVars.SetInt(Config.isBonusActionAvailableVariable, attacker.m_ScriptVars.GetInt(Config.isBonusActionAvailableVariable) - 1);

          LogUtils.LogMessage($"Attaque supplémentaire - Cogneur Lourd", LogUtils.LogType.Combat);
        }
        else
        {
          string targetName = $"{target.GetFirstName().GetSimple(0)} {target.GetLastName().GetSimple(0)}".ColorString(ColorConstants.Cyan);
          BroadcastNativeServerMessage($"{attackerName.ColorString(ColorConstants.Cyan)} cogneur lourd contre {targetName}", attacker);

          combatRound.AddCleaveAttack(target.m_idSelf);
          attacker.m_ScriptVars.SetInt(Config.isBonusActionAvailableVariable, attacker.m_ScriptVars.GetInt(Config.isBonusActionAvailableVariable) - 1);

          LogUtils.LogMessage($"Attaque supplémentaire - Cogneur Lourd", LogUtils.LogType.Combat);
        }
      }
    }
  }
}
