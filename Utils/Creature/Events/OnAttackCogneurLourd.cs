using System.Linq;
using Anvil.API;
using Anvil.API.Events;
using NWN.Systems;

namespace NWN
{
  public static partial class CreatureUtils
  {
    public static void OnAttackCogneurLourd(OnCreatureAttack onAttack)
    {
      if (onAttack.Attacker.GetObjectVariable<LocalVariableInt>(EffectSystem.CogneurLourdBonusAttackEffectTag).HasValue)
      {
        StringUtils.DisplayStringToAllPlayersNearTarget(onAttack.Attacker, "Cogneur Lourd", ColorConstants.Orange, true);
        onAttack.Attacker.GetObjectVariable<LocalVariableLocation>(EffectSystem.CogneurLourdBonusAttackEffectTag).Delete();
        onAttack.Attacker.ApplyEffect(EffectDuration.Temporary, EffectSystem.cogneurLourdEffect, NwTimeSpan.FromRounds(1));
      }
    }
  }
}
