﻿using NWN.Native.API;

namespace NWN.Systems
{
  public static partial class NativeUtils
  {
    public static int HandleEsquiveInstinctive(CNWSCreature creature)
    {
      if(creature.m_ScriptVars.GetInt(CreatureUtils.ReactionVariableExo) > 0)
      {
        if(creature.m_pStats.GetClassLevel((byte)ClassType.Rogue) > 4)
        {
          creature.m_ScriptVars.SetInt(CreatureUtils.ReactionVariableExo, creature.m_ScriptVars.GetInt(CreatureUtils.ReactionVariableExo) - 1);
          BroadcastNativeServerMessage("Esquive Instinctive", creature);
          LogUtils.LogMessage("Esquive Instinctive : dégâts divisés par 2", LogUtils.LogType.Combat);
          return 2;
        }
      }

      return 1;
    }
  }
}
