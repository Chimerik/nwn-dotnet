using Anvil.API.Events;
using Anvil.API;
using System.Linq;

namespace NWN.Systems
{
  public static partial class DruideUtils
  {
    public static void OnAttackBriseArmure(OnCreatureAttack onAttack)
    {
      switch(onAttack.AttackResult)
      {
        case AttackResult.Hit:
        case AttackResult.CriticalHit:
        case AttackResult.AutomaticHit:

          if (!onAttack.Target.ActiveEffects.Any(e => e.Tag == "_BRISE_ARMURE_EFFECT"))
          {
            var eff = Effect.ACDecrease(2, ACBonus.Natural);
            eff.Tag = "_BRISE_ARMURE_EFFECT";

            onAttack.Target.ApplyEffect(EffectDuration.Temporary, eff, NwTimeSpan.FromRounds(1));
            onAttack.Target.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpBreach));
          }

          break;
      }
    }
  }
}
