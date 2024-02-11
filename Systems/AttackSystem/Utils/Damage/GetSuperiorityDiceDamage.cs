using Anvil.API;
using NWN.Native.API;

namespace NWN.Systems
{
  public static partial class NativeUtils
  {
    public static int GetSuperiorityDiceDamage(CNWSCreature creature, CNWSCombatAttackData data)
    {
      if (data.m_nAttackType == 6 && creature.m_ScriptVars.GetInt(CreatureUtils.ManoeuvreTypeVariableExo) == CustomSkill.WarMasterRiposte
        && creature.m_ScriptVars.GetInt(CreatureUtils.ManoeuvreRiposteVariableExo).ToBool())
      {
        int superiorityRoll = NwRandom.Roll(Utils.random, creature.m_ScriptVars.GetInt(CreatureUtils.ManoeuvreDiceVariableExo));
        LogUtils.LogMessage($"Ajout dé de supériorité : +{superiorityRoll}", LogUtils.LogType.Combat);
        return superiorityRoll;
      }

      switch(creature.m_ScriptVars.GetInt(CreatureUtils.ManoeuvreTypeVariableExo))
      {
        case CustomSkill.WarMasterAttaqueMenacante:
        case CustomSkill.WarMasterRenversement:
        case CustomSkill.WarMasterDiversion:
        case CustomSkill.WarMasterFeinte:
        case CustomSkill.WarMasterInstruction:
        case CustomSkill.WarMasterProvocation:

          int superiorityRoll = NwRandom.Roll(Utils.random, creature.m_ScriptVars.GetInt(CreatureUtils.ManoeuvreDiceVariableExo));
          LogUtils.LogMessage($"Ajout dé de supériorité : +{superiorityRoll}", LogUtils.LogType.Combat);
          return superiorityRoll;

        default: return 0;
      }
    }
  }
}
