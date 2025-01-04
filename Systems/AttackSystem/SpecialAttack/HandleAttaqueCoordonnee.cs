using Anvil.API;
using NWN.Native.API;
using AssociateType = NWN.Native.API.AssociateType;

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

      if (!attacker.m_pStats.HasFeat(CustomSkill.BelluaireAttaqueCoordonnee).ToBool())
        return;

      var companion = NWNXLib.AppManager().m_pServerExoApp.GetCreatureByGameObjectID(attacker.GetAssociateId((ushort)AssociateType.AnimalCompanion));

      if (companion is null || companion.m_idSelf == 0x7F000000 || companion.m_ScriptVars.GetInt(CreatureUtils.AttaqueCoordonneCoolDownVariableExo).ToBool()) // OBJECT_INVALID
        return;

      companion.m_ScriptVars.SetInt(CreatureUtils.AttaqueCoordonneeVariableExo, 1);
    }
  }
}
