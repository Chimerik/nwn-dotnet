using System.Linq;
using Anvil.API;
using Anvil.API.Events;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static void OnSpellCastMagieDeCombat(OnSpellCast onCast)
    {
      if (onCast.Caster is not NwCreature caster || caster.ActiveEffects.Any(e => e.Tag == EffectSystem.MagieDeCombatEffectTag))
        return;

      caster.ApplyEffect(EffectDuration.Temporary, EffectSystem.MagieDeCombat, NwTimeSpan.FromRounds(1));
    }
  }
}
