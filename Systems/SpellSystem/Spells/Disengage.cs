using Anvil.API;
using Anvil.API.Events;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static void Disengage(SpellEvents.OnSpellCast onSpellCast)
    {
      if (onSpellCast.Caster is not NwCreature caster)
        return;

      SpellUtils.SignalEventSpellCast(onSpellCast.TargetObject, caster, onSpellCast.Spell.SpellType);
      caster.ApplyEffect(EffectDuration.Temporary, EffectSystem.disengageEffect, NwTimeSpan.FromRounds(1));
    }
  }
}
