using System;
using Anvil.API;
using NWN.Native.API;

namespace NWN.Systems
{
  public static partial class NativeUtils
  {
    public static void HandleBalayage(CNWSCreature attacker, CNWSObject currentTarget, CNWSCombatRound combatRound, string attackerName)
    {
      if (!attacker.m_ScriptVars.GetInt(CreatureUtils.ManoeuvreBalayageTargetVariableExo).ToBool())
        return;

      attacker.m_ScriptVars.DestroyInt(CreatureUtils.ManoeuvreBalayageTargetVariableExo);

      var target = NWNXLib.AppManager().m_pServerExoApp.GetCreatureByGameObjectID(attacker.GetNearestEnemy(3, currentTarget.m_idSelf, 1, 1));
      
      if (target is null || target.m_idSelf == 0x7F000000) // OBJECT_INVALID
        return;
      
      combatRound.AddWhirlwindAttack(target.m_idSelf, 1);

      string targetName = $"{target.GetFirstName().GetSimple(0)} {target.GetLastName().GetSimple(0)}".ColorString(ColorConstants.Cyan);
      DelayMessage($"{attackerName.ColorString(ColorConstants.Cyan)} balaye {targetName.ColorString(ColorConstants.Cyan)}", attacker);
    }
  }
}
