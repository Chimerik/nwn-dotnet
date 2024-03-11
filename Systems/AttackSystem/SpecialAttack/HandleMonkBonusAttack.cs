using Anvil.API;
using NWN.Native.API;

namespace NWN.Systems
{
  public static partial class NativeUtils
  {
    public static void HandleMonkBonusAttack(CNWSCreature attacker, CNWSObject currentTarget, CNWSCombatRound combatRound, string attackerName, string targetName)
    {
      if (!attacker.m_ScriptVars.GetInt(CreatureUtils.MonkBonusAttackVariableExo).ToBool())
        return;

      attacker.m_ScriptVars.DestroyInt(CreatureUtils.MonkBonusAttackVariableExo);
      attacker.m_ScriptVars.SetInt(CreatureUtils.MonkUnarmedDamageVariableExo, 1);

      combatRound.AddCleaveAttack(currentTarget.m_idSelf);

      DelayMessage($"{attackerName.ColorString(ColorConstants.Cyan)} art martial sur {targetName.ColorString(ColorConstants.Cyan)}", attacker);
    }
  }
}
