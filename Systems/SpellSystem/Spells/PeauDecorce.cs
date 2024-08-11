using System.Collections.Generic;
using Anvil.API;
using NWN.Core;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static List<NwGameObject> PeauDecorce(NwGameObject oCaster, NwSpell spell, SpellEntry spellEntry, NwGameObject oTarget)
    {
      SpellUtils.SignalEventSpellCast(oCaster, oCaster, spell.SpellType);

      NWScript.AssignCommand(oCaster, () => oTarget.ApplyEffect(EffectDuration.Temporary, EffectSystem.PeauDecorce, SpellUtils.GetSpellDuration(oCaster, spellEntry)));
      
      return new List<NwGameObject>() { oTarget };
    }
  }
}
