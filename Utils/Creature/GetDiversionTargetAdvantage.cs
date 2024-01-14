using Anvil.API;
using NWN.Native.API;

namespace NWN
{
  public static partial class CreatureUtils
  {
    public static int GetDiversionTargetAdvantage(CNWSCreature attacker, CNWSCreature target)
    {
      if(!target.m_ScriptVars.GetInt(ManoeuvreDiversionVariableExo).ToBool() || attacker.m_ScriptVars.GetInt(ManoeuvreDiversionExpiredVariableExo).ToBool())
        return 0;

      ExpireDiversion(attacker);

      LogUtils.LogMessage("Avantage - Cible affectée par Diversion", LogUtils.LogType.Combat);
      return 1;
    }
    private static async void ExpireDiversion(CNWSCreature attacker)
    {
      attacker.m_ScriptVars.SetInt(ManoeuvreDiversionExpiredVariableExo, 1);
      await NwTask.Delay(NwTimeSpan.FromRounds(1));
      attacker.m_ScriptVars.DestroyInt(ManoeuvreDiversionExpiredVariableExo);
    }
  }
}
