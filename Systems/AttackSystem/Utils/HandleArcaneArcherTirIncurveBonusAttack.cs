using System.Numerics;
using Anvil.API;
using NWN.Native.API;

namespace NWN.Systems
{
  public static partial class NativeUtils
  {
    public static void HandleArcaneArcherTirIncurveBonusAttack(CNWSCreature attacker, CNWSCombatRound combatRound)
    {
      var target = NWNXLib.AppManager().m_pServerExoApp.GetCreatureByGameObjectID(attacker.m_ScriptVars.GetObject(CreatureUtils.TirIncurveVariableExo));

      if (target is null)
        return;

      combatRound.AddCleaveAttack(target.m_idSelf);
      attacker.m_ScriptVars.DestroyObject(CreatureUtils.TirIncurveVariableExo);
    }
  }
}
