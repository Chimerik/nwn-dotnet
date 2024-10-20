using Anvil.API;
using NWN.Core;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static void VortexDechaine(NwGameObject oCaster, Location targetLocation, NwSpell spell, SpellEntry spellEntry)
    {
      SpellUtils.SignalEventSpellCast(oCaster, oCaster, spell.SpellType);
      NWScript.AssignCommand(oCaster, () => targetLocation.ApplyEffect(EffectDuration.Temporary, EffectSystem.VortexDechaine, NwTimeSpan.FromRounds(spellEntry.duration)));
    }
  }
}
