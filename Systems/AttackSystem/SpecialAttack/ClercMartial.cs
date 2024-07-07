using Anvil.API;
using NWN.Native.API;

namespace NWN.Systems
{
  public static partial class NativeUtils
  {
    public static void HandleClercMartial(CNWSCreature attacker, CNWSObject target, CNWSCombatRound round, string attackerName)
    {
      if (!attacker.m_ScriptVars.GetInt(CreatureUtils.ClercMartialVariableExo).ToBool())
        return;
      
      attacker.m_ScriptVars.DestroyInt(CreatureUtils.ClercMartialVariableExo);
      BroadcastNativeServerMessage($"{attackerName.ColorString(ColorConstants.Cyan)} - Clerc Martial", attacker);
      round.AddWhirlwindAttack(target.m_idSelf, 1);
      LogUtils.LogMessage("Attaque supplémentaire - Clerc Martial", LogUtils.LogType.Combat);
    }
  }
}
