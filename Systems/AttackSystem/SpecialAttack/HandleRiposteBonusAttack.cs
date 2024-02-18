using Anvil.API;
using NWN.Core;
using NWN.Native.API;

namespace NWN.Systems
{
  public static partial class NativeUtils
  {
    public static void HandleRiposteBonusAttack(CNWSCreature attacker, CNWSCombatRound round, CNWSCombatAttackData data, string attackerName)
    {
      if (!data.m_bRangedAttack.ToBool() && attacker.m_ScriptVars.GetObject(CreatureUtils.ManoeuvreRiposteVariableExo) != NWScript.OBJECT_INVALID)
      {
        var target = NWNXLib.AppManager().m_pServerExoApp.GetCreatureByGameObjectID(attacker.m_ScriptVars.GetObject(CreatureUtils.ManoeuvreRiposteVariableExo));

        if (target is null)
          return;

        string targetName = $"{target.GetFirstName().GetSimple(0)} {target.GetLastName().GetSimple(0)}".ColorString(ColorConstants.Cyan);
        BroadcastNativeServerMessage($"{attackerName.ColorString(ColorConstants.Cyan)} riposte contre {targetName}", attacker);

        round.AddWhirlwindAttack(target.m_idSelf, 1);
        LogUtils.LogMessage($"Attaque supplémentaire - Riposte", LogUtils.LogType.Combat);
      }

    }
  }
}
