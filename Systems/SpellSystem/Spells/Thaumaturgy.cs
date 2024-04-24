using Anvil.API;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static void Thaumaturgy(NwGameObject oCaster, NwSpell spell)
    {
      SpellUtils.SignalEventSpellCast(oCaster, oCaster, spell.SpellType);
      oCaster.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpImproveAbilityScore));
    }
  }
}
