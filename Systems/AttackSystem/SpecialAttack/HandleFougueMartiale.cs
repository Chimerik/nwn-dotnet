using System.Linq;
using System.Numerics;
using Anvil.API;
using NWN.Core;
using NWN.Native.API;
using ClassType = NWN.Native.API.ClassType;

namespace NWN.Systems
{
  public static partial class NativeUtils
  {
    public static void HandleFougueMartiale(CNWSCreature attacker, CNWSObject currentTarget, CNWSCombatRound round, string attackerName, string targetName)
    {
      if(attacker.m_appliedEffects.Any(e => e.m_sCustomTag.CompareNoCase(EffectSystem.ActionSurgeEffectExoTag).ToBool()))
      {
        EffectUtils.RemoveTaggedEffect(attacker, EffectSystem.ActionSurgeEffectExoTag);
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
