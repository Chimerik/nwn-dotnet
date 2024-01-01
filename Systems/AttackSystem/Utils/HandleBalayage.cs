using Anvil.API;
using NWN.Core;
using NWN.Native.API;

namespace NWN.Systems
{
  public static partial class NativeUtils
  {
    public static void HandleBalayage(CNWSCreature attacker, CNWSCombatRound combatRound)
    {
      var target = NWNXLib.AppManager().m_pServerExoApp.GetCreatureByGameObjectID(attacker.m_ScriptVars.GetObject(CreatureUtils.ManoeuvreBalayageTargetVariableExo));
      
      if (target is null || !NWScript.GetIsObjectValid(target.m_idSelf).ToBool())
        return;

      attacker.m_ScriptVars.DestroyInt(CreatureUtils.ManoeuvreBalayageTargetVariableExo);
      combatRound.AddCleaveAttack(target.m_idSelf);
    }
  }
}
