using NWN.Native.API;
using NWN.Systems;

namespace NWN
{
  public static partial class CreatureUtils
  {
    public static int GetJeuDeJambeAttackerDisadvantage(CNWSCreature target)
    {
      if (target.m_ScriptVars.GetInt(ManoeuvreTypeVariableExo) == CustomSkill.WarMasterJeuDeJambe)
      {
        LogUtils.LogMessage($"Désavantage - Attaque de mêlée sur une cible en mode Jeu de Jambe", LogUtils.LogType.Combat);
        return -1;
      }
      else
        return 0;
    }
  }
}
