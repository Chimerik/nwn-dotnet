
using System.Collections.Generic;
using Anvil.API;
using NWN.Core;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static List<NwGameObject> ImmolationVolontaire(NwGameObject oCaster, NwSpell spell, SpellEntry spellEntry)
    {
      SpellUtils.SignalEventSpellCast(oCaster, oCaster, spell.SpellType);
      oCaster.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpPulseFire));
      NWScript.AssignCommand(oCaster, () => oCaster.ApplyEffect(EffectDuration.Temporary, EffectSystem.Hate, NwTimeSpan.FromRounds(spellEntry.duration)));

      return new List<NwGameObject>() { oCaster };
    }
  }
}
