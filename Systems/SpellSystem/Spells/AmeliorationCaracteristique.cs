

using Anvil.API;
using System.Collections.Generic;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static List<NwGameObject> AmeliorationCaracteristique(NwGameObject oCaster, NwSpell spell, SpellEntry spellEntry, NwGameObject oTarget)
    {
      SpellUtils.SignalEventSpellCast(oTarget, oCaster, spell.SpellType);

      EffectUtils.RemoveTaggedEffect(oTarget, EffectSystem.AmeliorationCaracteristiqueEffectTag);

      oCaster.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpImproveAbilityScore));
      oCaster.ApplyEffect(EffectDuration.Temporary, EffectSystem.AmeliorationCaracteristique(oTarget, spellEntry), NwTimeSpan.FromRounds(spellEntry.duration));

      return new List<NwGameObject> { oTarget };
    }
  }
}

