using System.Linq;
using Anvil.API;
using Anvil.API.Events;
using NWN.Systems;

namespace NWN
{
  public static partial class CreatureUtils
  {
    public static void OnAttackCharge(OnCreatureAttack onAttack)
    {
      if(!onAttack.Attacker.ActiveEffects.Any(e => e.Tag == EffectSystem.SprintEffectTag))
      {
        onAttack.Attacker.OnCreatureAttack -= OnAttackCharge;
        return;
      }

      if (onAttack.Attacker.GetObjectVariable<LocalVariableLocation>("_CHARGER_ACTIVATED").HasValue)
      {
        StringUtils.DisplayStringToAllPlayersNearTarget(onAttack.Attacker, "Bonus de charge", ColorConstants.Orange, true);
        onAttack.Attacker.GetObjectVariable<LocalVariableLocation>("_CHARGER_INITIAL_LOCATION").Delete();
        onAttack.Attacker.GetObjectVariable<LocalVariableLocation>("_CHARGER_ACTIVATED").Delete();
        onAttack.Attacker.OnCreatureAttack -= OnAttackCharge;
      }
    }
  }
}
