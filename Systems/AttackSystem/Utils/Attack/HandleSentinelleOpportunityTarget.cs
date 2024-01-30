using Anvil.API;
using NWN.Core;
using NWN.Native.API;

namespace NWN.Systems
{
  public static partial class NativeUtils
  {
    public static void HandleSentinelleOpportunityTarget(CNWSCreature attacker, CNWSCombatRound combatRound, string attackerName)
    {
      var target = NWNXLib.AppManager().m_pServerExoApp.GetCreatureByGameObjectID(attacker.m_ScriptVars.GetObject(CreatureUtils.ManoeuvreBalayageTargetVariableExo));

      if (target is null || !NWScript.GetIsObjectValid(target.m_idSelf).ToBool())
        return;

      string targetName = $"{target.GetFirstName().GetSimple(0)} {target.GetLastName().GetSimple(0)}".ColorString(ColorConstants.Cyan);
      BroadcastNativeServerMessage($"Frappe sentinelle de {attackerName.ColorString(ColorConstants.Cyan)} contre {targetName}", attacker);

      combatRound.AddCleaveAttack(target.m_idSelf);
      attacker.m_ScriptVars.DestroyObject(CreatureUtils.SentinelleOpportunityTargetVariableExo);
    }
  }
}
