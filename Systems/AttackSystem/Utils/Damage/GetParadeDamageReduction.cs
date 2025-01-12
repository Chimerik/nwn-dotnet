using Anvil.API;
using NWN.Native.API;

namespace NWN.Systems
{
  public static partial class NativeUtils
  {
    public static int GetParadeDamageReduction(CNWSCreature creature, bool isCritical)
    {
      if(!isCritical && creature is not null && creature.m_ScriptVars.GetInt(CreatureUtils.ManoeuvreTypeVariableExo) == CustomSkill.WarMasterParade)
      {
        int superiorityDice = creature.m_ScriptVars.GetInt(CreatureUtils.ManoeuvreDiceVariableExo);
        int superiorityRoll = NwRandom.Roll(Utils.random, superiorityDice);
        int dexMod = GetAbilityModifier(creature, Anvil.API.Ability.Dexterity);
        int damageReduction = superiorityRoll + dexMod;

        creature.m_ScriptVars.DestroyInt(CreatureUtils.ManoeuvreTypeVariableExo);
        creature.m_ScriptVars.DestroyInt(CreatureUtils.ManoeuvreDiceVariableExo);

        LogUtils.LogMessage($"Parade - Réduction de dégâts : {superiorityRoll} (1d{superiorityDice}) + {dexMod} = {damageReduction}", LogUtils.LogType.Combat);
        BroadcastNativeServerMessage($"Parade - Réduction de dégâts : {superiorityRoll} (1d{superiorityDice}) + {dexMod} = {damageReduction}", creature);
        return damageReduction;
      }

      return 0;
    }
  }
}
