using Anvil.API;
using NWN.Core;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static void Vol(NwGameObject oCaster, NwSpell spell, SpellEntry spellEntry)
    {
      SpellUtils.SignalEventSpellCast(oCaster, oCaster, spell.SpellType);      
      oCaster.ApplyEffect(EffectDuration.Temporary, EffectSystem.Vol, NwTimeSpan.FromRounds(spellEntry.duration));
    }
  }
}
