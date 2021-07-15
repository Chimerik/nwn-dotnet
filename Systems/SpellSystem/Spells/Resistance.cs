using Anvil.API;
using Anvil.API.Events;

namespace NWN.Systems
{
  class Resistance
  {
    public Resistance(SpellEvents.OnSpellCast onSpellCast)
    {
      if (!(onSpellCast.Caster is NwCreature { IsPlayerControlled: true } oCaster))
        return;

      int nCasterLevel = oCaster.LastSpellCasterLevel;

      SpellUtils.SignalEventSpellCast(onSpellCast.TargetObject, oCaster, onSpellCast.Spell, false);

      Effect eVis = Effect.VisualEffect(VfxType.ImpHeadHoly);
      Effect eDur = Effect.VisualEffect(VfxType.DurCessatePositive);

      int nBonus = 1 + nCasterLevel / 6; //Saving throw bonus to be applied
      int nDuration = 2 + nCasterLevel / 6; // Turns

      if (onSpellCast.MetaMagicFeat == MetaMagic.Extend)
        nDuration = nDuration * 2;

      Effect eSave = Effect.SavingThrowIncrease(SavingThrow.All, nBonus);
      Effect eLink = Effect.LinkEffects(eSave, eDur);

      onSpellCast.TargetObject.ApplyEffect(EffectDuration.Temporary, eLink, NwTimeSpan.FromRounds(nDuration));
      onSpellCast.TargetObject.ApplyEffect(EffectDuration.Instant, eVis);

      if (onSpellCast.MetaMagicFeat == MetaMagic.None)
        SpellUtils.RestoreSpell(oCaster, onSpellCast.Spell);
    }
  }
}
