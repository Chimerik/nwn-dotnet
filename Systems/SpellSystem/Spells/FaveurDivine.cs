using Anvil.API;
using NWN.Core;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static void FaveurDivine(NwGameObject oCaster, NwSpell spell, SpellEntry spellEntry)
    {
      SpellUtils.SignalEventSpellCast(oCaster, oCaster, spell.SpellType);

      NWScript.AssignCommand(oCaster, () => oCaster.ApplyEffect(EffectDuration.Temporary, EffectSystem.FaveurDivine, SpellUtils.GetSpellDuration(oCaster, spellEntry)));
      oCaster.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpHeadHoly));
      oCaster.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.FnfLosHoly10));
    }
  }
}
