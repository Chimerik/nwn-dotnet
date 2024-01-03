using Anvil.API;
using NWN.Native.API;

namespace NWN.Systems
{
  public static partial class NativeUtils
  {
    public static void HandleFureurOrc(CNWSCreature attacker, CNWSCreature target, CNWSCombatRound combatRound)
    {
      if (attacker.m_ScriptVars.GetInt(CreatureUtils.FureurOrcBonusAttackVariableExo).ToBool())
      {
        attacker.m_ScriptVars.DestroyInt(CreatureUtils.FureurOrcBonusAttackVariableExo);
        combatRound.AddCleaveAttack(target.m_idSelf);
        SendNativeServerMessage("Fureur Orc - Attaque Supplémentaire".ColorString(StringUtils.gold), attacker);
      }
    }
  }
}
