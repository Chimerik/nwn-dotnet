using Anvil.API;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static void Sanctuaire(NwGameObject oCaster, NwSpell spell, SpellEntry spellEntry, NwGameObject oTarget, NwClass castingClass)
    {
      SpellUtils.SignalEventSpellCast(oTarget, oCaster, spell.SpellType);

      if (oTarget is not NwCreature target)
        return;

      target.ApplyEffect(EffectDuration.Temporary, EffectSystem.Sanctuaire(target, spell, SpellUtils.GetCasterSpellDC(oCaster, spell, castingClass.SpellCastingAbility)), SpellUtils.GetSpellDuration(oCaster, spellEntry));
    }
  }
}
