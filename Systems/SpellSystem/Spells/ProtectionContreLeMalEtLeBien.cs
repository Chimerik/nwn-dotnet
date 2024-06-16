using System.Collections.Generic;
using Anvil.API;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static List<NwGameObject> ProtectionContreLeMalEtLeBien(NwGameObject oCaster, NwSpell spell, SpellEntry spellEntry, NwGameObject oTarget)
    {
      SpellUtils.SignalEventSpellCast(oTarget, oCaster, spell.SpellType);

      oTarget.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(spellEntry.damageVFX));
      oTarget.ApplyEffect(EffectDuration.Temporary, EffectSystem.ProtectionContreLeMalEtLeBien, NwTimeSpan.FromRounds(spellEntry.duration));

      return new List<NwGameObject> { oTarget };
    }
  }
}
