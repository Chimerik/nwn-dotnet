using System.Collections.Generic;
using Anvil.API;
using NWN.Core;
using NWN.Core.NWNX;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static List<NwGameObject> RayonDeLune(NwGameObject oCaster, NwSpell spell, SpellEntry spellEntry, NwGameObject target)
    {
      SpellUtils.SignalEventSpellCast(oCaster, oCaster, spell.SpellType);
      NWScript.AssignCommand(oCaster, () => target.ApplyEffect(EffectDuration.Temporary, EffectSystem.RayonDeLuneAura, SpellUtils.GetSpellDuration(oCaster, spellEntry)));
      
      return new List<NwGameObject>() { target };
    }
  }
}
