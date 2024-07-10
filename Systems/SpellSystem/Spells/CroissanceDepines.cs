using System.Collections.Generic;
using Anvil.API;
using NWN.Core;
using NWN.Core.NWNX;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static List<NwGameObject> CroissanceDepines(NwGameObject oCaster, NwSpell spell, SpellEntry spellEntry, Location targetLocation)
    {
      SpellUtils.SignalEventSpellCast(oCaster, oCaster, spell.SpellType);

      NWScript.AssignCommand(oCaster, () => targetLocation.ApplyEffect(EffectDuration.Temporary, EffectSystem.CroissanceDepinesAoE, NwTimeSpan.FromRounds(spellEntry.duration)));
    
      return new List<NwGameObject>() { UtilPlugin.GetLastCreatedObject(11).ToNwObject<NwAreaOfEffect>() };
    }
  }
}
