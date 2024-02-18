using Anvil.API;
using NWN.Core;
using NWN.Native.API;

namespace NWN.Systems
{
  public static partial class NativeUtils
  {
    public static int GetSuperiorityDiceDamage(CNWSCreature creature, CNWSCombatAttackData data)
    {
      if (data.m_nAttackType == 39102 && creature.m_ScriptVars.GetInt(CreatureUtils.ManoeuvreTypeVariableExo) == CustomSkill.WarMasterRiposte
        && creature.m_ScriptVars.GetObject(CreatureUtils.ManoeuvreRiposteVariableExo) != NWScript.OBJECT_INVALID)
      {
        int superiorityDice = creature.m_ScriptVars.GetInt(CreatureUtils.ManoeuvreDiceVariableExo);
        int superiorityRoll = NwRandom.Roll(Utils.random, superiorityDice);
        LogUtils.LogMessage($"Ajout dé de supériorité (1d{superiorityDice}) : +{superiorityRoll}", LogUtils.LogType.Combat);
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
        case CustomSkill.WarMasterManoeuvreTactique:

          int superiorityDice = creature.m_ScriptVars.GetInt(CreatureUtils.ManoeuvreDiceVariableExo);
          int superiorityRoll = NwRandom.Roll(Utils.random, superiorityDice);
          LogUtils.LogMessage($"Ajout dé de supériorité (1d{superiorityDice}) : +{superiorityRoll}", LogUtils.LogType.Combat);
          return superiorityRoll;

        default: return 0;
      }
    }
  }
}
