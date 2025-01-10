using Anvil.API;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static void DetectionDeLinvisibilite(NwGameObject oCaster, NwSpell spell, SpellEntry spellEntry)
    {
      SpellUtils.SignalEventSpellCast(oCaster, oCaster, spell.SpellType);

      oCaster.ApplyEffect(EffectDuration.Temporary, Effect.LinkEffects(Effect.SeeInvisible(), Effect.VisualEffect(VfxType.DurMagicalSight)), SpellUtils.GetSpellDuration(oCaster, spellEntry));
    }
  }
}
