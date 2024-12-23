

using Anvil.API;
using System.Collections.Generic;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static List<NwGameObject> Resistance(NwGameObject oCaster, NwSpell spell, SpellEntry spellEntry, NwGameObject oTarget)
    {
      SpellUtils.SignalEventSpellCast(oTarget, oCaster, spell.SpellType);

      EffectUtils.RemoveTaggedEffect(oTarget, EffectSystem.ResistanceEffectTag);
      EffectUtils.RemoveTaggedParamEffect(oTarget, EffectSystem.CooldownEffectTag, CustomSpell.Resistance, CustomSpell.ResistanceAcide, CustomSpell.ResistanceContondant, CustomSpell.ResistanceElec, CustomSpell.ResistanceFeu, CustomSpell.ResistanceFroid, CustomSpell.ResistancePercant, CustomSpell.ResistancePoison, CustomSpell.ResistanceTranchant);

      oCaster.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpGoodHelp));
      oCaster.ApplyEffect(EffectDuration.Temporary, EffectSystem.Resistance(oTarget, spell), NwTimeSpan.FromRounds(spellEntry.duration));

      return new List<NwGameObject> { oTarget };
    }
  }
}

