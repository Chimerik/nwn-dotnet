using System.Collections.Generic;
using Anvil.API;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static List<NwGameObject> Invisibility(NwGameObject oCaster, NwSpell spell, SpellEntry spellEntry, NwGameObject oTarget)
    {
      SpellUtils.SignalEventSpellCast(oTarget, oCaster, spell.SpellType, false);
      oTarget.ApplyEffect(EffectDuration.Temporary, Effect.LinkEffects(Effect.Invisibility(InvisibilityType.Normal), Effect.VisualEffect(VfxType.DurCessatePositive)), NwTimeSpan.FromRounds(spellEntry.duration));

      return new List<NwGameObject> { oTarget };
    }
  }
}
