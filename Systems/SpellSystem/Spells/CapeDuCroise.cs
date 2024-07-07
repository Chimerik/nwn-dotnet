using System.Collections.Generic;
using Anvil.API;
using NWN.Core;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static List<NwGameObject> CapeDuCroise(NwGameObject oCaster, NwSpell spell, SpellEntry spellEntry)
    {
      SpellUtils.SignalEventSpellCast(oCaster, oCaster, spell.SpellType);
      NWScript.AssignCommand(oCaster, () => oCaster.ApplyEffect(EffectDuration.Temporary, EffectSystem.CapeDuCroiseAura, NwTimeSpan.FromRounds(spellEntry.duration)));
      
      return new List<NwGameObject>() { oCaster };
    }
  }
}
