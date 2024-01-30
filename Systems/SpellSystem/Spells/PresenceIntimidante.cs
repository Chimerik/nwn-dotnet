using System.Linq;
using Anvil.API;
using Anvil.API.Events;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static void PresenceIntimidante(SpellEvents.OnSpellCast onSpellCast, PlayerSystem.Player player)
    {
      if (onSpellCast.Caster is not NwCreature caster)
        return;

      if (!caster.ActiveEffects.Any(e => e.Tag == EffectSystem.PresenceIntimidanteAuraEffectTag))
      {
        SpellUtils.SignalEventSpellCast(onSpellCast.TargetObject, caster, onSpellCast.Spell.SpellType);
        caster.ApplyEffect(EffectDuration.Temporary, EffectSystem.presenceIntimidante, NwTimeSpan.FromRounds(10));
      }
      else
        EffectUtils.RemoveTaggedEffect(caster, EffectSystem.PresenceIntimidanteAuraEffectTag);
    }
  }
}
