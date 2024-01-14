using NWN.Native.API;
using NWN.Systems;

namespace NWN
{
  public static partial class CreatureUtils
  {
    public static int GetFeinteAttackerAdvantage(CNWSCreature attacker)
    {
      if(attacker.m_ScriptVars.GetInt(ManoeuvreTypeVariableExo) == CustomSkill.WarMasterFeinte 
        && attacker.m_ScriptVars.GetInt(BonusActionVariableExo) > 0)
      {
        attacker.m_ScriptVars.SetInt(BonusActionVariableExo, attacker.m_ScriptVars.GetInt(BonusActionVariableExo) - 1);
        LogUtils.LogMessage("Avantage - Feinte", LogUtils.LogType.Combat);
        return 1;
      }

      return 0;
    }
  }
}
