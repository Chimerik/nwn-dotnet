using System.Linq;
using Anvil.API;

namespace NWN.Systems
{
  public static partial class SpellUtils
  {
    public static void CheckDispelConcentration(NwCreature caster, NwSpell spell, SpellEntry spellEntry)
    {
      if (!(spell.Id == CustomSpell.LameArdente && caster.GetObjectVariable<LocalVariableInt>(EffectSystem.ConcentrationSpellIdString).Value == CustomSpell.LameArdente)
          && spellEntry.requiresConcentration
          && caster.ActiveEffects.Any(e => e.Tag == EffectSystem.ConcentrationEffectTag))
        DispelConcentrationEffects(caster);
    }
  }
}
