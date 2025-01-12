using System.Linq;
using Anvil.API;
using NWN.Native.API;

namespace NWN.Systems
{
  public static partial class NativeUtils
  {
    public static void HandleFougueMartiale(CNWSCreature attacker, CNWSObject currentTarget, CNWSCombatRound round, string attackerName, string targetName)
    {
      if(attacker.m_appliedEffects.Any(e => e.m_sCustomTag.ToString() == EffectSystem.ActionSurgeEffectTag))
      {
        EffectUtils.RemoveTaggedNativeEffect(attacker, EffectSystem.ActionSurgeEffectTag);
        BroadcastNativeServerMessage($"{attackerName.ColorString(ColorConstants.Cyan)} déclenche sa fougue sur {targetName}", attacker);

        for (int i = 0; i < attacker.m_pStats.GetAttacksPerRound(); i++)
        {
          round.AddCleaveAttack(currentTarget.m_idSelf);
          LogUtils.LogMessage($"Attaque supplémentaire - Fougue Martiale", LogUtils.LogType.Combat);
        }
      }
    }
  }
}
