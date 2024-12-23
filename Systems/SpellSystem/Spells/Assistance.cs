using Anvil.API;
using System.Collections.Generic;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static List<NwGameObject> Assistance(NwGameObject oCaster, NwSpell spell, SpellEntry spellEntry, NwGameObject oTarget)
    {
      SpellUtils.SignalEventSpellCast(oTarget, oCaster, spell.SpellType);

      EffectUtils.RemoveTaggedEffect(oTarget, EffectSystem.AssistanceEffectTag);

      oCaster.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpGoodHelp));
      oCaster.ApplyEffect(EffectDuration.Temporary, EffectSystem.Assistance, NwTimeSpan.FromRounds(spellEntry.duration));

      return new List<NwGameObject> { oTarget };
    }
  }
}
