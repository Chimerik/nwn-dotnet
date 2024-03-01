using Anvil.API;
using NWN.Native.API;

namespace NWN.Systems
{
  public static partial class NativeUtils
  {
    public static void HandleArcaneArcherTirIncurveBonusAttack(CNWSCreature attacker, CNWSCombatAttackData data, CNWSCombatRound combatRound, string attackerName, CNWSItem weapon, CNWSObject currentTarget)
    {
      if (!data.m_bRangedAttack.ToBool() || data.m_nAttackResult != 4 
        || attacker.m_ScriptVars.GetInt(CreatureUtils.BonusActionVariableExo) < 1
        || !ItemUtils.HasBowEquipped(NwBaseItem.FromItemId((int)weapon.m_nBaseItem).ItemType))
        return;

      var target = NWNXLib.AppManager().m_pServerExoApp.GetCreatureByGameObjectID(attacker.GetNearestEnemy(18, currentTarget.m_idSelf, 1, 1));

      if (target is null)
        return;

      string targetName = $"{target.GetFirstName().GetSimple(0)} {target.GetLastName().GetSimple(0)}".ColorString(ColorConstants.Cyan);
      BroadcastNativeServerMessage($"{attackerName.ColorString(ColorConstants.Cyan)} courbe son tir manqué vers {targetName}", attacker);

      combatRound.AddWhirlwindAttack(target.m_idSelf, 1);
      attacker.m_ScriptVars.SetInt(CreatureUtils.BonusActionVariableExo, attacker.m_ScriptVars.GetInt(CreatureUtils.BonusActionVariableExo) - 1);
      LogUtils.LogMessage($"Attaque supplémentaire - Tir incurvé", LogUtils.LogType.Combat);
    }
  }
}
