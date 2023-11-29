using System.Numerics;
using Anvil.API;
using NWN.Native.API;
using Feat = Anvil.API.Feat;

namespace NWN.Systems
{
  public static partial class NativeUtils
  {
    public static void HandleCrossbowMaster(CNWSCreature attacker, CNWSObject target, CNWSCombatRound combatRound, int proficiency)
    {
      if (combatRound.GetCurrentAttackWeapon() is not null && proficiency > 0 && combatRound.GetCurrentAttackWeapon().m_nBaseItem == (uint)BaseItemType.Shuriken
        && attacker.m_pStats.HasFeat((ushort)Feat.RapidReload).ToBool()
        && attacker.m_ScriptVars.GetInt(Config.isBonusActionAvailableVariable).ToBool())
      {
        combatRound.AddCleaveAttack(target.m_idSelf);
        attacker.m_ScriptVars.SetInt(Config.isBonusActionAvailableVariable, attacker.m_ScriptVars.GetInt(Config.isBonusActionAvailableVariable) - 1);
      }
    }
  }
}
