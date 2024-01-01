using Anvil.API;
using NWN.Native.API;

namespace NWN.Systems
{
  public static partial class NativeUtils
  {
    public static int GetSuperiorityDiceDamage(CNWSCreature creature)
    {
      return creature.m_ScriptVars.GetInt(CreatureUtils.ManoeuvreDiceVariableExo) switch
      {
        CustomSkill.WarMasterAttaqueMenacante or CustomSkill.WarMasterRenversement or CustomSkill.WarMasterDiversion or CustomSkill.WarMasterFeinte or CustomSkill.WarMasterInstruction or CustomSkill.WarMasterProvocation or CustomSkill.WarMasterRiposte => NwRandom.Roll(Utils.random, creature.m_ScriptVars.GetInt(CreatureUtils.ManoeuvreDiceVariableExo)),
        _ => 0,
      };
    }
  }
}
