using NWN.API;
using NWN.API.Constants;
using NWN.API.Events;

namespace NWN.Systems
{
  static class SpellFailure
  {
    public static void ApplyEffectToTarget(NwCreature oTarget)
    {
      oTarget.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpNegativeEnergy));
      oTarget.OnSpellCast += SpellFailureMalus;
    }
    public static void RemoveEffectFromTarget(NwCreature oTarget)
    {
      oTarget.OnSpellCast -= SpellFailureMalus;
    }
    private static void SpellFailureMalus(OnSpellCast onSpellCast)
    {
      if (NwRandom.Roll(Utils.random, 100, 1) < 6)
      {
        onSpellCast.PreventSpellCast = true;

        if (onSpellCast.Caster is NwCreature { IsPlayerControlled: true } oPC)
          oPC.ControllingPlayer.SendServerMessage("Votre sort échoue en raison du handicap d'échec des sorts.", Color.RED);
      }
    }
  }
}
