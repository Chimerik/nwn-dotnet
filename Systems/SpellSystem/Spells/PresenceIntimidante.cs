using System.Linq;
using Anvil.API;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static void PresenceIntimidante(NwGameObject oCaster, NwSpell spell)
    {
      if (!oCaster.ActiveEffects.Any(e => e.Tag == EffectSystem.PresenceIntimidanteAuraEffectTag))
      {
        SpellUtils.SignalEventSpellCast(oCaster, oCaster, spell.SpellType);
        oCaster.ApplyEffect(EffectDuration.Temporary, EffectSystem.presenceIntimidante, NwTimeSpan.FromRounds(10));
      }
      else
        EffectUtils.RemoveTaggedEffect(oCaster, EffectSystem.PresenceIntimidanteAuraEffectTag);
    }
  }
}
