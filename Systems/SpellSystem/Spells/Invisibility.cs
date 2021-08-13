using Anvil.API;
using Anvil.API.Events;

namespace NWN.Systems
{
  class Invisibility
  {
    public Invisibility(SpellEvents.OnSpellCast onSpellCast)
    {
      if (!(onSpellCast.Caster is NwCreature { IsPlayerControlled: true } oCaster))
        return;

      int nCasterLevel = oCaster.LastSpellCasterLevel;

      SpellUtils.SignalEventSpellCast(onSpellCast.TargetObject, oCaster, onSpellCast.Spell, false);

      int nDuration = nCasterLevel;
      Effect eInvis = Effect.Invisibility(InvisibilityType.Normal);
      Effect eDur = Effect.VisualEffect(VfxType.DurCessatePositive);
      Effect eLink = Effect.LinkEffects(eInvis, eDur);

      eLink = Effect.LinkEffects(eLink, Effect.AreaOfEffect(193, null, "invi_hb"));  // 193 = AoE 20 m

      if (onSpellCast.MetaMagicFeat == MetaMagic.Extend)
        nDuration = nDuration * 2; //Duration is +100%

      onSpellCast.TargetObject.ApplyEffect(EffectDuration.Temporary, eInvis, NwTimeSpan.FromRounds(nDuration));
    }
  }
}
