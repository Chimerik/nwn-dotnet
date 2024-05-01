using System.Linq;
using Anvil.API;

namespace NWN.Systems
{
  public static partial class SpellUtils
  {
    public static void CheckDispelConcentration(NwCreature caster, NwSpell spell, SpellEntry spellEntry)
    {
      if (!(spell.Id == CustomSpell.FlameBlade && caster.GetObjectVariable<LocalVariableInt>(EffectSystem.ConcentrationSpellIdString).Value == CustomSpell.FlameBlade) // TODO : Si on recast Flame Blade, alors on ne compte pas un nouvel emplacement de sort
              && spellEntry.requiresConcentration
              && caster.ActiveEffects.Any(e => e.Tag == EffectSystem.ConcentrationEffectTag))
        SpellUtils.DispelConcentrationEffects(caster);
    }
  }
}
