using Anvil.API;
using NWN.Native.API;

namespace NWN.Systems
{
  public static partial class NativeUtils
  {
    public static void HandleFureurOrc(CNWSCreature attacker, CNWSCreature target, CNWSCombatRound combatRound, string attackerName)
    {
      if (attacker.m_ScriptVars.GetInt(CreatureUtils.FureurOrcBonusAttackVariableExo).ToBool())
      {
        string targetName = $"{target.GetFirstName().GetSimple(0)} {target.GetLastName().GetSimple(0)}".ColorString(ColorConstants.Cyan);
        BroadcastNativeServerMessage($"La fureur orc de {attackerName.ColorString(ColorConstants.Cyan)} s'abat sur {targetName}", attacker);

        attacker.m_ScriptVars.DestroyInt(CreatureUtils.FureurOrcBonusAttackVariableExo);
        combatRound.AddCleaveAttack(target.m_idSelf);
      }
    }
  }
}
