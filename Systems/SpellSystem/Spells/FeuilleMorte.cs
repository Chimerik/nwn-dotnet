
using Anvil.API;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static void FeuilleMorte(NwGameObject oCaster, NwSpell spell, SpellEntry spellEntry)
    {
      SpellUtils.SignalEventSpellCast(oCaster, oCaster, spell.SpellType);
      oCaster.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpHaste));
    }
  }
}
