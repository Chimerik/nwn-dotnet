
using Anvil.API;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static void Blessure(NwGameObject oCaster, NwSpell spell, SpellEntry spellEntry, NwGameObject oTarget, NwClass casterClass)
    {
      if (oCaster is not NwCreature caster || oTarget is not NwCreature target)
        return;

      SpellUtils.SignalEventSpellCast(oTarget, oCaster, spell.SpellType);
      int spellDC = SpellUtils.GetCasterSpellDC(caster, spell, casterClass.SpellCastingAbility);

      SpellUtils.DealSpellDamage(target, oCaster.CasterLevel, spellEntry, SpellUtils.GetSpellDamageDiceNumber(oCaster, spell),
        oCaster, spell.GetSpellLevelForClass(casterClass.ClassType), CreatureUtils.GetSavingThrow(oCaster, target, spellEntry.savingThrowAbility, spellDC, spellEntry));

      target.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpNegativeEnergy));
    }
  }
}
