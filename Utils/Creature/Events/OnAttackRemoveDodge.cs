using Anvil.API.Events;
using NWN.Systems;

namespace NWN
{
  public static partial class CreatureUtils
  {
    public static void OnAttackRemoveDodge(OnCreatureAttack onAttack)
    {
      foreach (var eff in onAttack.Attacker.ActiveEffects)
        if (eff.Tag == EffectSystem.DodgeEffectTag)
          onAttack.Attacker.RemoveEffect(eff);

      onAttack.Attacker.OnCreatureAttack -= OnAttackRemoveDodge;
    }
  }
}
