using Anvil.API;

using NWN.Native.API;

namespace NWN.Systems
{
  public static partial class NativeUtils
  {
    public static int GetSuperiorityDiceDamage(CNWSCreature creature, bool isCritical)
    {
      if (creature.m_ScriptVars.GetInt(CreatureUtils.ManoeuvreTypeVariableExo) == CustomSkill.WarMasterRiposte
        && creature.m_ScriptVars.GetInt(CreatureUtils.ManoeuvreRiposteVariableExo).ToBool())
      {
        int superiorityDice = creature.m_ScriptVars.GetInt(CreatureUtils.ManoeuvreDiceVariableExo);
        int nbDices = isCritical ? 2 : 1;
        int superiorityRoll = Utils.Roll(superiorityDice, nbDices);

        creature.m_ScriptVars.DestroyInt(CreatureUtils.ManoeuvreRiposteVariableExo);

        LogUtils.LogMessage($"Ajout dé de supériorité ({nbDices}d{superiorityDice}) : +{superiorityRoll}", LogUtils.LogType.Combat);
        return superiorityRoll;
      }

      switch(creature.m_ScriptVars.GetInt(CreatureUtils.ManoeuvreTypeVariableExo))
      {
        case CustomSkill.WarMasterAttaqueMenacante:
        case CustomSkill.WarMasterRenversement:
        case CustomSkill.WarMasterDiversion:
        case CustomSkill.WarMasterDesarmement:
        case CustomSkill.WarMasterFeinte:
        case CustomSkill.WarMasterInstruction:
        case CustomSkill.WarMasterProvocation:
        case CustomSkill.WarMasterManoeuvreTactique:

          int superiorityDice = creature.m_ScriptVars.GetInt(CreatureUtils.ManoeuvreDiceVariableExo);
          int superiorityRoll = Utils.Roll(superiorityDice);
          LogUtils.LogMessage($"Ajout dé de supériorité (1d{superiorityDice}) : +{superiorityRoll}", LogUtils.LogType.Combat);
          return superiorityRoll;

        default: return 0;
      }
    }
  }
}
