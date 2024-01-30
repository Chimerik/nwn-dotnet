using Anvil.API;
using NWN.Native.API;

namespace NWN.Systems
{
  public static partial class NativeUtils
  {
    public static void HandleFrappeFrenetiqueBonusAttack(CNWSCreature attacker, CNWSObject target, CNWSCombatRound round, CNWSCombatAttackData data, string attackerName)
    {
      if (!data.m_bRangedAttack.ToBool() && attacker.m_ScriptVars.GetInt(CreatureUtils.FrappeFrenetiqueVariableExo).ToBool())
      {
        string targetName = $"{target.GetFirstName().GetSimple(0)} {target.GetLastName().GetSimple(0)}".ColorString(ColorConstants.Cyan);
        BroadcastNativeServerMessage($"{attackerName.ColorString(ColorConstants.Cyan)} frappe frénétiquement {targetName}", attacker);

        round.AddCleaveAttack(target.m_idSelf);
        attacker.m_ScriptVars.DestroyInt(CreatureUtils.FrappeFrenetiqueVariableExo);
      }

    }
  }
}
