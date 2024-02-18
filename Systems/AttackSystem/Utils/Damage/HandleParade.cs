using Anvil.API;
using NWN.Native.API;

namespace NWN.Systems
{
  public static partial class NativeUtils
  {
    public static int HandleParade(CNWSCreature creature)
    {
      if(creature.m_ScriptVars.GetInt(CreatureUtils.ManoeuvreTypeVariableExo) == CustomSkill.WarMasterParade)
      {
        int superiorityDice = creature.m_ScriptVars.GetInt(CreatureUtils.ManoeuvreDiceVariableExo);
        int superiorityRoll = NwRandom.Roll(Utils.random, superiorityDice);
        int damageReduction = superiorityRoll + creature.m_pStats.m_nDexterityModifier;

        creature.m_ScriptVars.DestroyInt(CreatureUtils.ManoeuvreTypeVariableExo);
        creature.m_ScriptVars.DestroyInt(CreatureUtils.ManoeuvreDiceVariableExo);

        LogUtils.LogMessage($"Parade - Réduction de dégâts : {superiorityRoll} (1d{superiorityDice}) + {creature.m_pStats.m_nDexterityModifier} = {damageReduction}", LogUtils.LogType.Combat);
        BroadcastNativeServerMessage($"Parade - Réduction de dégâts : {superiorityRoll} (1d{superiorityDice}) + {creature.m_pStats.m_nDexterityModifier} = {damageReduction}", creature);
        return damageReduction;
      }

      return 0;
    }
  }
}
