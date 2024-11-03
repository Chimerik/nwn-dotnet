using Anvil.API;
using System.Collections.Generic;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static List<NwGameObject> TrueStrike(NwGameObject oCaster, NwSpell spell, SpellEntry spellEntry)
    {
      SpellUtils.SignalEventSpellCast(oCaster, oCaster, spell.SpellType);
      oCaster.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpHeadOdd));
      oCaster.ApplyEffect(EffectDuration.Temporary, EffectSystem.TrueStrike, NwTimeSpan.FromRounds(spellEntry.duration));

      return new List<NwGameObject> { oCaster };
    }
  }
}
