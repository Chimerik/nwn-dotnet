using System.Collections.Generic;
using System.Linq;
using Anvil.API;
using Anvil.API.Events;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    private static void Invisibility(SpellEvents.OnSpellCast onSpellCast, SpellEntry spellEntry)
    {
      if (!(onSpellCast.Caster is NwCreature { IsPlayerControlled: true } caster))
        return;

      SpellUtils.SignalEventSpellCast(onSpellCast.TargetObject, caster, onSpellCast.Spell.SpellType, false);

      onSpellCast.TargetObject.ApplyEffect(EffectDuration.Temporary, Effect.LinkEffects(Effect.Invisibility(InvisibilityType.Normal), Effect.VisualEffect(VfxType.DurCessatePositive)), NwTimeSpan.FromRounds(spellEntry.duration));

      EffectSystem.ApplyConcentrationEffect(caster, onSpellCast.Spell.Id, new List<NwGameObject> { onSpellCast.TargetObject }, spellEntry.duration);
    }
  }
}
