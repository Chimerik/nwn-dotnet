using Anvil.API;
using Anvil.API.Events;
using System.Collections.Generic;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static void TrueStrike(SpellEvents.OnSpellCast onSpellCast, SpellEntry spellEntry)
    {
      if (onSpellCast.Caster is not NwCreature caster)
        return;

      SpellUtils.SignalEventSpellCast(onSpellCast.TargetObject, caster, onSpellCast.Spell.SpellType);
      onSpellCast.TargetObject.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpHeadOdd));
      onSpellCast.TargetObject.ApplyEffect(EffectDuration.Temporary, EffectSystem.trueStrikeEffect, NwTimeSpan.FromRounds(spellEntry.duration));

      EffectSystem.ApplyConcentrationEffect(caster, onSpellCast.Spell.Id, new List<NwGameObject> { caster }, spellEntry.duration);
    }
  }
}
