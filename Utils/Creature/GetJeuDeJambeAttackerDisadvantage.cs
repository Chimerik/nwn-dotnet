using NWN.Native.API;
using NWN.Systems;

namespace NWN
{
  public static partial class CreatureUtils
  {
    public static int GetJeuDeJambeAttackerDisadvantage(CNWSCreature target)
    {
      return target.m_ScriptVars.GetInt(ManoeuvreTypeVariableExo) == CustomSkill.WarMasterJeuDeJambe ? -1 : 0;
    }
  }
}
