using Anvil.API;
using NWN.Native.API;

namespace NWN.Systems
{
  public static partial class NativeUtils
  {
    public static void HandleRiposte(CNWSCreature attacker, CNWSCreature target, CNWSCombatAttackData data)
    {
      if (!data.m_bRangedAttack.ToBool() && target.m_ScriptVars.GetInt(CreatureUtils.ManoeuvreTypeVariableExo) == CustomSkill.WarMasterRiposte
        && target.m_ScriptVars.GetInt(CreatureUtils.ReactionVariableExo) > 0)
      {
        target.m_ScriptVars.SetInt(CreatureUtils.ReactionVariableExo, target.m_ScriptVars.GetInt(CreatureUtils.ReactionVariableExo) - 1);
        target.m_ScriptVars.SetObject(CreatureUtils.ManoeuvreRiposteVariableExo, attacker.m_idSelf);
      }
    }
  }
}
