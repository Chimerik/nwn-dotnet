using Anvil.API;
using Anvil.API.Events;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static void BladeWard(NwGameObject oCaster, NwSpell spell, SpellEntry spellEntry, NwGameObject oTarget)
    {
      SpellUtils.SignalEventSpellCast(oTarget, oCaster, spell.SpellType);

      oCaster.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpAcBonus));
      oCaster.ApplyEffect(EffectDuration.Temporary, Effect.LinkEffects(Effect.VisualEffect(VfxType.DurCessatePositive),
        Effect.DamageImmunityIncrease(DamageType.Bludgeoning, 50), Effect.DamageImmunityIncrease(DamageType.Piercing, 50), 
        Effect.DamageImmunityIncrease(DamageType.Slashing, 50)), NwTimeSpan.FromRounds(spellEntry.duration));
    }
  }
}
