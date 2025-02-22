

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
      EffectUtils.RemoveTaggedSpellEffect(oTarget, EffectSystem.CooldownEffectTag, NwSpell.FromSpellId(CustomSpell.Resistance), 
        NwSpell.FromSpellId(CustomSpell.ResistanceAcide), NwSpell.FromSpellId(CustomSpell.ResistanceContondant), 
        NwSpell.FromSpellId(CustomSpell.ResistanceElec), NwSpell.FromSpellId(CustomSpell.ResistanceFeu), 
        NwSpell.FromSpellId(CustomSpell.ResistanceFroid), NwSpell.FromSpellId(CustomSpell.ResistancePercant), 
        NwSpell.FromSpellId(CustomSpell.ResistancePoison), NwSpell.FromSpellId(CustomSpell.ResistanceTranchant));

      oCaster.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpGoodHelp));
      oCaster.ApplyEffect(EffectDuration.Temporary, EffectSystem.Resistance(oTarget, spell), NwTimeSpan.FromRounds(spellEntry.duration));

      return new List<NwGameObject> { oTarget };
    }
  }
}

