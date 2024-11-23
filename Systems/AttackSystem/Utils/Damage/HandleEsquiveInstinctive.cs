using Anvil.API;
using NWN.Native.API;

namespace NWN.Systems
{
  public static partial class NativeUtils
  {
    public static int HandleEsquiveInstinctive(CNWSCreature creature)
    {
      if (creature.m_ScriptVars.GetInt(CreatureUtils.ReactionVariableExo) < 1
        || !creature.m_pStats.HasFeat(CustomSkill.EsquiveInstinctive).ToBool())
        return 1;
      
      creature.m_ScriptVars.SetInt(CreatureUtils.ReactionVariableExo, creature.m_ScriptVars.GetInt(CreatureUtils.ReactionVariableExo) - 1);
      BroadcastNativeServerMessage("Esquive Instinctive", creature);
      LogUtils.LogMessage("Esquive Instinctive : dégâts divisés par 2", LogUtils.LogType.Combat);
      return 2;
    }
  }
}
