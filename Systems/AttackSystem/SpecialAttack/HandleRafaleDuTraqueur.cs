using Anvil.API;
using NWN.Native.API;

namespace NWN.Systems
{
  public static partial class NativeUtils
  {
    public static void HandleRafaleDuTraqueur(CNWSCreature attacker, CNWSObject target, CNWSCombatRound round, string attackerName, string targetName)
    {
      if (!attacker.m_pStats.HasFeat(CustomSkill.TraqueurRafale).ToBool() || attacker.m_ScriptVars.GetInt(CreatureUtils.RafaleDuTraqueurVariableExo).ToBool())
        return;
      
      attacker.m_ScriptVars.SetInt(CreatureUtils.RafaleDuTraqueurVariableExo, 1);
      BroadcastNativeServerMessage($"{attackerName.ColorString(ColorConstants.Cyan)} rafale {targetName.ColorString(ColorConstants.Cyan)}", attacker);
      round.AddWhirlwindAttack(target.m_idSelf, 1);
      LogUtils.LogMessage("Attaque supplémentaire - Rafale du Traqueur", LogUtils.LogType.Combat);
    }
  }
}
