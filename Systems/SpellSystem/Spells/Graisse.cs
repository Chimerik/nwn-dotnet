using Anvil.API;
using NWN.Core;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static void Graisse(NwGameObject oCaster, NwSpell spell, SpellEntry spellEntry, Location targetLocation, NwClass casterClass)
    {
      if (oCaster is not NwCreature caster)
        return;

      SpellUtils.SignalEventSpellCast(oCaster, oCaster, spell.SpellType);
      oCaster.GetObjectVariable<LocalVariableInt>($"_SPELL_CASTING_ABILITY_{spell.Id}").Value = (int)casterClass.SpellCastingAbility;
      NWScript.AssignCommand(caster, () => targetLocation.ApplyEffect(EffectDuration.Temporary, EffectSystem.Graisse, SpellUtils.GetSpellDuration(oCaster, spellEntry)));
    }
  }
}
