using System.Linq;
using Anvil.API;
using Anvil.API.Events;

namespace NWN.Systems
{
  static class Slow
  {
    public static void ApplyEffectToTarget(NwCreature oTarget)
    {
      oTarget.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpSlow));
      Effect slow = Effect.Slow();
      slow.Tag = "CUSTOM_EFFECT_SLOW_MALUS";
      slow.SubType = EffectSubType.Supernatural;
      Effect msDecrease = Effect.MovementSpeedDecrease(10);
      slow.Tag = "CUSTOM_EFFECT_SLOW_MALUS";
      slow.SubType = EffectSubType.Supernatural;
      oTarget.ApplyEffect(EffectDuration.Permanent, slow);
      oTarget.ApplyEffect(EffectDuration.Permanent, msDecrease);
      oTarget.OnSpellCastAt -= SlowMalusCure;
      oTarget.OnSpellCastAt += SlowMalusCure;
    }
    public static void RemoveEffectFromTarget(NwCreature oTarget)
    {
      foreach (Effect eff in oTarget.ActiveEffects.Where(e => e.Tag == "CUSTOM_EFFECT_SLOW_MALUS"))
        oTarget.RemoveEffect(eff);

      oTarget.OnSpellCastAt -= SlowMalusCure;
    }
    public static void SlowMalusCure(CreatureEvents.OnSpellCastAt onSpellCastAt)
    {
      switch (onSpellCastAt.Spell)
      {
        case Spell.LesserRestoration:
        case Spell.Restoration:
        case Spell.GreaterRestoration:
        case Spell.RemoveCurse:
        case Spell.Haste:
        case Spell.MassHaste:

          foreach (Effect eff in onSpellCastAt.Creature.ActiveEffects.Where(e => e.Tag == "_ARENA_MALUS_SLOW"))
            onSpellCastAt.Creature.RemoveEffect(eff);

          onSpellCastAt.Creature.OnSpellCastAt -= SlowMalusCure;
          break;
      }
    }
  }
}
