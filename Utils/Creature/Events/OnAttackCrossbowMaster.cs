using System.Linq;
using Anvil.API;
using Anvil.API.Events;
using NWN.Systems;

namespace NWN
{
  public static partial class CreatureUtils
  {
    public static void OnAttackCrossbowMaster(OnCreatureAttack onAttack)
    {
      if (onAttack.Attacker.GetObjectVariable<LocalVariableInt>(EffectSystem.CrossbowMasterEffectTag).HasValue)
      {
        StringUtils.DisplayStringToAllPlayersNearTarget(onAttack.Attacker, "Maître arbalétrier", ColorConstants.Orange, true);
        onAttack.Attacker.GetObjectVariable<LocalVariableLocation>(EffectSystem.CrossbowMasterEffectTag).Delete();
        onAttack.Attacker.ApplyEffect(EffectDuration.Temporary, EffectSystem.crossbowMasterEffect, NwTimeSpan.FromRounds(1));
      }
    }
  }
}
