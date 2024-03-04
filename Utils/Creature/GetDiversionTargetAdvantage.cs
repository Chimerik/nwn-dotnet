using Anvil.API;
using NWN.Native.API;

namespace NWN.Systems
{
  public static partial class CreatureUtils
  {
    public static bool GetDiversionTargetAdvantage(CNWSCreature attacker, CNWSCreature target)
    {
      if(!target.m_ScriptVars.GetInt(ManoeuvreDiversionVariableExo).ToBool() || attacker.m_ScriptVars.GetInt(ManoeuvreDiversionExpiredVariableExo).ToBool())
        return false;

      ExpireDiversion(attacker);

      LogUtils.LogMessage("Avantage - Cible affectée par Diversion", LogUtils.LogType.Combat);
      return true;
    }
    private static async void ExpireDiversion(CNWSCreature attacker)
    {
      attacker.m_ScriptVars.SetInt(ManoeuvreDiversionExpiredVariableExo, 1);
      await NwTask.Delay(NwTimeSpan.FromRounds(1));
      attacker.m_ScriptVars.DestroyInt(ManoeuvreDiversionExpiredVariableExo);
    }
  }
}
