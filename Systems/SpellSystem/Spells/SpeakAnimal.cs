using Anvil.API;
using Anvil.API.Events;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static void SpeakAnimal(SpellEvents.OnSpellCast onSpellCast, SpellEntry spellEntry)
    {
      if (onSpellCast.Caster is not NwCreature caster)
        return;

      SpellUtils.SignalEventSpellCast(onSpellCast.TargetObject, caster, onSpellCast.Spell.SpellType);
      caster.ApplyEffect(EffectDuration.Temporary, EffectSystem.speakAnimalEffect, NwTimeSpan.FromRounds(spellEntry.duration));
    }
  }
}
