using Anvil.API;
using NWN.Native.API;

namespace NWN.Systems
{
  public static partial class NativeUtils
  {
    public static int GetSuperiorityDiceDamage(CNWSCreature creature, CNWSCombatAttackData data)
    {
      if (data.m_nAttackType == 6 && creature.m_ScriptVars.GetInt(CreatureUtils.ManoeuvreDiceVariableExo) == CustomSkill.WarMasterRiposte
        && creature.m_ScriptVars.GetInt(CreatureUtils.ManoeuvreRiposteVariableExo).ToBool())
        return NwRandom.Roll(Utils.random, creature.m_ScriptVars.GetInt(CreatureUtils.ManoeuvreDiceVariableExo));

      return creature.m_ScriptVars.GetInt(CreatureUtils.ManoeuvreDiceVariableExo) switch
      {
        CustomSkill.WarMasterAttaqueMenacante or CustomSkill.WarMasterRenversement or CustomSkill.WarMasterDiversion 
        or CustomSkill.WarMasterFeinte or CustomSkill.WarMasterInstruction or CustomSkill.WarMasterProvocation 
        => NwRandom.Roll(Utils.random, creature.m_ScriptVars.GetInt(CreatureUtils.ManoeuvreDiceVariableExo)),
        _ => 0,
      };
    }
  }
}
