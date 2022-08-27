using Anvil.API;
using Anvil.API.Events;

namespace NWN.Systems
{
  class Virtue
  {
    public Virtue(SpellEvents.OnSpellCast onSpellCast)
    {
      if (!(onSpellCast.Caster is NwCreature { IsPlayerControlled: true } oCaster))
        return;

      int nCasterLevel = oCaster.LastSpellCasterLevel;

      SpellUtils.SignalEventSpellCast(onSpellCast.TargetObject, oCaster, onSpellCast.Spell.SpellType, false);

      int nDuration = nCasterLevel;
      Effect eVis = Effect.VisualEffect(VfxType.ImpHolyAid);
      Effect eHP = Effect.TemporaryHitpoints(1);
      Effect eDur = Effect.VisualEffect(VfxType.DurCessatePositive);
      Effect eLink = Effect.LinkEffects(eHP, eDur);

      if (onSpellCast.MetaMagicFeat == MetaMagic.Extend)
        nDuration = nDuration * 2; //Duration is +100%

      onSpellCast.TargetObject.ApplyEffect(EffectDuration.Temporary, eLink, NwTimeSpan.FromRounds(nDuration));
      onSpellCast.TargetObject.ApplyEffect(EffectDuration.Instant, eVis);

      if (onSpellCast.MetaMagicFeat == MetaMagic.None)
      {
        SpellUtils.RestoreSpell(oCaster, onSpellCast.Spell.SpellType);
      }
    }
  }
}
