using System.Linq;
using Anvil.API;
using NWN.Native.API;

namespace NWN.Systems
{
  public static partial class NativeUtils
  {
    public static void HandleCogneurLourdBonusAttack(CNWSCreature attacker, CNWSCombatAttackData attackData, CNWSItem attackWeapon)
    {
      if (attacker.m_pStats.HasFeat(CustomSkill.CogneurLourd).ToBool() && attackWeapon is not null 
        && !attackData.m_bRangedAttack.ToBool() && attacker.m_ScriptVars.GetInt(Config.isBonusActionAvailableVariable).ToBool())
      {
        attacker.m_ScriptVars.SetInt(EffectSystem.CogneurLourdBonusAttackExoTag, 1);
        attacker.m_ScriptVars.SetInt(Config.isBonusActionAvailableVariable, attacker.m_ScriptVars.GetInt(Config.isBonusActionAvailableVariable) - 1);
      }
    }
  }
}
