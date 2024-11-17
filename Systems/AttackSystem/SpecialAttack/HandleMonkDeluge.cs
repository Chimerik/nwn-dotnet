using Anvil.API;
using NWN.Native.API;

namespace NWN.Systems
{
  public static partial class NativeUtils
  {
    public static void HandleMonkDeluge(CNWSCreature attacker, CNWSObject currentTarget, CNWSCombatRound combatRound, string attackerName, string targetName)
    {
      if (!attacker.m_ScriptVars.GetInt(CreatureUtils.MonkDelugeVariableExo).ToBool())
        return;

      attacker.m_ScriptVars.DestroyInt(CreatureUtils.MonkDelugeVariableExo);
      attacker.m_ScriptVars.SetInt(CreatureUtils.MonkUnarmedDamageVariableExo, 2);

      combatRound.AddCleaveAttack(currentTarget.m_idSelf);
      combatRound.AddCleaveAttack(currentTarget.m_idSelf);

      if(attacker.m_pStats.GetNumLevelsOfClass(CustomClass.Monk) > 9)
        combatRound.AddCleaveAttack(currentTarget.m_idSelf);

      DelayMessage($"{attackerName.ColorString(ColorConstants.Cyan)} déchaîne un déluge de coups sur {targetName.ColorString(ColorConstants.Cyan)}", attacker);
    }
  }
}
