using Anvil.API;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static void VisionNocturne(NwGameObject oCaster, NwSpell spell, SpellEntry spellEntry, NwGameObject oTarget)
    {
      SpellUtils.SignalEventSpellCast(oTarget, oCaster, spell.SpellType);

      oTarget.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpMagicalVision));
      oTarget.ApplyEffect(EffectDuration.Temporary, Effect.LinkEffects(Effect.Icon(CustomEffectIcon.VisionNocturne), Effect.VisualEffect(VfxType.DurMagicalSight)), SpellUtils.GetSpellDuration(oCaster, spellEntry));
    }
  }
}
