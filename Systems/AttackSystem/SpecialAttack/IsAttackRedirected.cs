using Anvil.API;
using NWN.Native.API;

namespace NWN.Systems
{
  public static partial class NativeUtils
  {
    public static bool IsAttackRedirected(CNWSCreature attacker, CNWSCreature target, CNWSCombatRound combatRound, string attackerName)
    {
      if (!attacker.m_pStats.HasFeat(CustomSkill.ConspirateurRedirection).ToBool()
        || attacker.m_ScriptVars.GetInt(CreatureUtils.ReactionVariableExo) < 1)
        return false;

      var newTarget = NWNXLib.AppManager().m_pServerExoApp.GetCreatureByGameObjectID(target.GetNearestEnemy(2, attacker.m_idSelf, 1, 1));

      if (newTarget is null)
        return false;

      string newTargetName = $"{newTarget.GetFirstName().GetSimple(0)} {newTarget.GetLastName().GetSimple(0)}".ColorString(ColorConstants.Cyan);
      string targetName = $"{target.GetFirstName().GetSimple(0)} {target.GetLastName().GetSimple(0)}".ColorString(ColorConstants.Cyan);
      BroadcastNativeServerMessage($"{targetName.ColorString(ColorConstants.Cyan)} redirige l'attaque de {attackerName.ColorString(ColorConstants.Cyan)} sur {newTargetName.ColorString(ColorConstants.Cyan)}", target);

      combatRound.AddWhirlwindAttack(newTarget.m_idSelf, 1);
      attacker.m_ScriptVars.SetInt(CreatureUtils.ReactionVariableExo, attacker.m_ScriptVars.GetInt(CreatureUtils.ReactionVariableExo) - 1);
      LogUtils.LogMessage($"Attaque redirigée vers {newTargetName.StripColors()}", LogUtils.LogType.Combat);
      
      return true;
    }
  }
}
