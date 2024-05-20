using Anvil.API;
using NWN.Native.API;

namespace NWN.Systems
{
  public static partial class CreatureUtils
  {
    public static bool GetEsquiveDuTraqueurDisadvantage(CNWSCreature target)
    {
      if (!target.m_pStats.HasFeat(CustomSkill.TraqueurEsquive).ToBool() || target.m_ScriptVars.GetInt(EsquiveDuTraqueurVariableExo).ToBool())
        return false;

      target.m_ScriptVars.SetInt(EsquiveDuTraqueurVariableExo, 1);
      LogUtils.LogMessage("Désavantage - Esquive du Traqueur", LogUtils.LogType.Combat);

      return true;
    }
  }
}
