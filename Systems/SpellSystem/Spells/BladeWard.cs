using Anvil.API;
using Anvil.API.Events;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static void BladeWard(SpellEvents.OnSpellCast onSpellCast, SpellEntry spellEntry)
    {
      if (onSpellCast.Caster is not NwCreature oCaster)
        return;

      SpellUtils.SignalEventSpellCast(onSpellCast.TargetObject, oCaster, onSpellCast.Spell.SpellType);

      onSpellCast.Caster.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpAcBonus));
      onSpellCast.Caster.ApplyEffect(EffectDuration.Temporary, Effect.LinkEffects(Effect.VisualEffect(VfxType.DurCessatePositive),
        Effect.DamageImmunityIncrease(DamageType.Bludgeoning, 50), Effect.DamageImmunityIncrease(DamageType.Piercing, 50), 
        Effect.DamageImmunityIncrease(DamageType.Slashing, 50)), NwTimeSpan.FromRounds(spellEntry.duration));
    }
  }
}
