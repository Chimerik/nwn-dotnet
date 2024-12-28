using Anvil.API;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static void GreleDepines(NwGameObject oCaster, NwSpell spell, SpellEntry spellEntry, NwClass casterClass)
    {
      if (oCaster is not NwCreature caster)
        return;

      SpellUtils.SignalEventSpellCast(oCaster, oCaster, spell.SpellType);
      oCaster.ApplyEffect(EffectDuration.Temporary, EffectSystem.GreleDepines(caster, casterClass.SpellCastingAbility), SpellUtils.GetSpellDuration(oCaster, spellEntry));
    }
  }
}
