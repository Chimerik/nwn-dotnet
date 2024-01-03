using Anvil.API;
using NWN.Core;
using NWN.Native.API;

namespace NWN.Systems
{
  public static partial class NativeUtils
  {
    public static void HandleDiversion(CNWSCreature attacker, CNWSCombatAttackData data, CNWSCreature target)
    {
      if (attacker.m_ScriptVars.GetInt(CreatureUtils.ManoeuvreTypeVariableExo) != CustomSkill.WarMasterDiversion)
        return;

      switch (data.m_nAttackResult)
      {
        case 1:
        case 3: ExpireDiversion(attacker, target); break;
      }
    }
    private static async void ExpireDiversion(CNWSCreature attacker, CNWSCreature target)
    {
      attacker.m_ScriptVars.DestroyInt(CreatureUtils.ManoeuvreTypeVariableExo);
      attacker.m_ScriptVars.DestroyInt(CreatureUtils.ManoeuvreDiceVariableExo);

      target.m_ScriptVars.SetInt(CreatureUtils.ManoeuvreDiversionVariableExo, 1);
      attacker.m_ScriptVars.SetInt(CreatureUtils.ManoeuvreDiversionExpiredVariableExo, 1);

      await NwTask.Delay(NwTimeSpan.FromRounds(1));

      target.m_ScriptVars.DestroyInt(CreatureUtils.ManoeuvreDiversionVariableExo);
      attacker.m_ScriptVars.DestroyInt(CreatureUtils.ManoeuvreDiversionExpiredVariableExo);
    }
  }
}
