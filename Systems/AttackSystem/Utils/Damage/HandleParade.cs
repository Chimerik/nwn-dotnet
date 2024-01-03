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
        int damageReduction = NwRandom.Roll(Utils.random, creature.m_ScriptVars.GetInt(CreatureUtils.ManoeuvreDiceVariableExo)) + creature.m_pStats.m_nDexterityModifier;

        creature.m_ScriptVars.DestroyInt(CreatureUtils.ManoeuvreTypeVariableExo);
        creature.m_ScriptVars.DestroyInt(CreatureUtils.ManoeuvreDiceVariableExo);
        
        return damageReduction;
      }

      return 0;
    }
  }
}
