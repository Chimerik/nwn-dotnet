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

      NWScript.AssignCommand(oCaster, () => oTarget.ApplyEffect(EffectDuration.Temporary, EffectSystem.BouclierDeLaFoi, SpellUtils.GetSpellDuration(oCaster, spellEntry)));
      oTarget.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpAcBonus));
         
      return new List<NwGameObject>() { oTarget };
    }
  }
}
