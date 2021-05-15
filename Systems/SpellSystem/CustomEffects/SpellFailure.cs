using NWN.API;
using NWN.API.Constants;
using NWN.API.Events;

namespace NWN.Systems
{
  class SpellFailure
  {
    public SpellFailure(NwCreature oTarget, bool apply = true)
    {
      if (apply)
        ApplyEffectToTarget(oTarget);
      else
        RemoveEffectFromTarget(oTarget);
    }
    private void ApplyEffectToTarget(NwCreature oTarget)
    {
      oTarget.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpNegativeEnergy));
      oTarget.OnSpellCast += SpellFailureMalus;
    }
    private void RemoveEffectFromTarget(NwCreature oTarget)
    {
      oTarget.OnSpellCast -= SpellFailureMalus;
    }
    private void SpellFailureMalus(OnSpellCast onSpellCast)
    {
      if (NwRandom.Roll(Utils.random, 100, 1) < 6)
      {
        onSpellCast.PreventSpellCast = true;
        ((NwPlayer)onSpellCast.Caster).SendServerMessage("Votre sort échoue en raison du handicap d'échec des sorts.", Color.RED);
      }
    }
  }
}
