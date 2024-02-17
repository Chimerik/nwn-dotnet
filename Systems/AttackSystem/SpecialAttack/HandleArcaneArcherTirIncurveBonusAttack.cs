using System.Numerics;
using Anvil.API;
using NWN.Native.API;

namespace NWN.Systems
{
  public static partial class NativeUtils
  {
    public static void HandleArcaneArcherTirIncurveBonusAttack(CNWSCreature attacker, CNWSCombatRound combatRound, string attackerName)
    {
      var target = NWNXLib.AppManager().m_pServerExoApp.GetCreatureByGameObjectID(attacker.m_ScriptVars.GetObject(CreatureUtils.TirIncurveVariableExo));

      if (target is null)
        return;

      string targetName = $"{target.GetFirstName().GetSimple(0)} {target.GetLastName().GetSimple(0)}".ColorString(ColorConstants.Cyan);
      BroadcastNativeServerMessage($"{attackerName.ColorString(ColorConstants.Cyan)} courbe son tir vers {targetName}", attacker);

      combatRound.AddWhirlwindAttack(target.m_idSelf, 1);
      attacker.m_ScriptVars.DestroyObject(CreatureUtils.TirIncurveVariableExo);
      LogUtils.LogMessage($"Attaque supplémentaire - Tir incurvé", LogUtils.LogType.Combat);
    }
  }
}
