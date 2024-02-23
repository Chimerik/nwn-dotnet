using Anvil.API;
using Anvil.API.Events;

namespace NWN.Systems
{
  class Flare
  {
    public Flare(SpellEvents.OnSpellCast onSpellCast)
    {
      if (!(onSpellCast.Caster is NwCreature { IsPlayerControlled: true } oCaster))
        return;

      int nCasterLevel = oCaster.CasterLevel;

      SpellUtils.SignalEventSpellCast(onSpellCast.TargetObject, oCaster, onSpellCast.Spell.SpellType);

      Effect eVis = Effect.VisualEffect(VfxType.ImpFlameS);
      onSpellCast.TargetObject.ApplyEffect(EffectDuration.Instant, eVis);

      if (oCaster.CheckResistSpell(onSpellCast.TargetObject) == ResistSpellResult.Failed
        && onSpellCast.TargetObject.RollSavingThrow(SavingThrow.Fortitude, onSpellCast.SaveDC, SavingThrowType.Spell) == SavingThrowResult.Failure)
      {
        Effect eBad = Effect.AttackDecrease(1 + nCasterLevel / 6);
        onSpellCast.TargetObject.ApplyEffect(EffectDuration.Temporary, eBad, NwTimeSpan.FromRounds(10 + nCasterLevel));
      }
    }
  }
}
