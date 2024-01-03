using Anvil.API;
using NWN.Native.API;

namespace NWN.Systems
{
  public static partial class NativeUtils
  {
    public static void HandleRiposteBonusAttack(CNWSCreature attacker, CNWSCombatRound round, CNWSCombatAttackData data)
    {
      if (!data.m_bRangedAttack.ToBool() && attacker.m_nCurrentAction == (ushort)Action.AttackObject
         && attacker.m_ScriptVars.GetInt(CreatureUtils.ManoeuvreRiposteVariableExo).ToBool())
      {
        var target = NWNXLib.AppManager().m_pServerExoApp.GetCreatureByGameObjectID(attacker.m_ScriptVars.GetObject(CreatureUtils.ManoeuvreRiposteVariableExo));

        if (target is null)
          return;

        round.AddCleaveAttack(target.m_idSelf);
      }

    }
  }
}
