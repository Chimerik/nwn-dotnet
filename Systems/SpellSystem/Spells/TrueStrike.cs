using Anvil.API;
using Anvil.API.Events;
using System.Collections.Generic;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static void TrueStrike(NwGameObject oCaster, NwSpell spell, SpellEntry spellEntry)
    {
      SpellUtils.SignalEventSpellCast(oCaster, oCaster, spell.SpellType);
      oCaster.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpHeadOdd));
      oCaster.ApplyEffect(EffectDuration.Temporary, EffectSystem.trueStrikeEffect, NwTimeSpan.FromRounds(spellEntry.duration));

      if(oCaster is NwCreature caster)
        EffectSystem.ApplyConcentrationEffect(caster, spell.Id, new List<NwGameObject> { oCaster }, spellEntry.duration);
    }
  }
}
