using System.Collections.Generic;
using Anvil.API;
using NWN.Core;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static List<NwGameObject> BouclierDeLaFoi(NwGameObject oCaster, NwSpell spell, SpellEntry spellEntry, NwGameObject oTarget)
    {
      SpellUtils.SignalEventSpellCast(oCaster, oTarget, spell.SpellType);
      List<NwGameObject> targets = SpellUtils.GetSpellTargets(oCaster, oTarget, spellEntry, true);

      foreach (var target in targets)
      {
        NWScript.AssignCommand(oCaster, () => target.ApplyEffect(EffectDuration.Temporary, EffectSystem.BouclierDeLaFoi, SpellUtils.GetSpellDuration(oCaster, spellEntry)));
        target.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpAcBonus));
      } 
      
      return targets;
    }
  }
}
