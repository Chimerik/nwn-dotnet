using Anvil.API;
using NWN.Native.API;

namespace NWN.Systems
{
  public static partial class NativeUtils
  {
    public static void HandleAttaqueCoordonnee(CNWSCreature attacker, CNWSObject currentTarget, CNWSCombatRound combatRound)
    {
      if(attacker.m_ScriptVars.GetInt(CreatureUtils.AttaqueCoordonneeVariableExo).ToBool() && !attacker.m_ScriptVars.GetInt(CreatureUtils.AttaqueCoordonneCoolDownVariableExo).ToBool())
      {
        attacker.m_ScriptVars.DestroyInt(CreatureUtils.AttaqueCoordonneeVariableExo);
        attacker.m_ScriptVars.SetInt(CreatureUtils.AttaqueCoordonneCoolDownVariableExo, 1);

        combatRound.AddWhirlwindAttack(currentTarget.m_idSelf, 1);
        return;
      }

      var animalId = attacker.m_ScriptVars.GetObject(CreatureUtils.AnimalCompanionVariableExo);
      var animal = NWNXLib.AppManager().m_pServerExoApp.GetCreatureByGameObjectID(animalId);

      if (animal is null || animal.m_idSelf == 0x7F000000 || animal.m_ScriptVars.GetInt(CreatureUtils.AttaqueCoordonneCoolDownVariableExo).ToBool()) // OBJECT_INVALID
        return;

      animal.m_ScriptVars.SetInt(CreatureUtils.AttaqueCoordonneeVariableExo, 1);
    }
  }
}
