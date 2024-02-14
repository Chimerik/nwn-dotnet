using Anvil.API;
using NWN.Native.API;

namespace NWN.Systems
{
  public static partial class NativeUtils
  {
    public static void HandleBersekerRepresaillesBonusAttack(CNWSCreature attacker, CNWSCombatRound round, CNWSCombatAttackData data, string attackerName)
    {
      if (!data.m_bRangedAttack.ToBool() && attacker.m_pStats.HasFeat(CustomSkill.BersekerRepresailles).ToBool())
      {
        var target = NWNXLib.AppManager().m_pServerExoApp.GetCreatureByGameObjectID(attacker.m_ScriptVars.GetObject(CreatureUtils.BersekerRepresaillesVariableExo));

        if (target is null)
          return;

        string targetName = $"{target.GetFirstName().GetSimple(0)} {target.GetLastName().GetSimple(0)}".ColorString(ColorConstants.Cyan);
        BroadcastNativeServerMessage($"{attackerName.ColorString(ColorConstants.Cyan)} représailles contre {targetName}", attacker);

        round.AddWhirlwindAttack(target.m_idSelf, 1);
        LogUtils.LogMessage($"Attaque supplémentaire - Représailles", LogUtils.LogType.Combat);
      }
    }
  }
}
