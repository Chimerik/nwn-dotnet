using Anvil.API;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static void Fletrissement(NwGameObject oCaster, NwSpell spell, SpellEntry spellEntry, NwGameObject oTarget, NwClass casterClass)
    {
      int spellDC = 10;

      SpellUtils.SignalEventSpellCast(oTarget, oCaster, spell.SpellType);

      if (oTarget is not NwCreature target || Utils.In(target.Race.RacialType, RacialType.Construct, RacialType.Undead))
      {
        oTarget.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpMagicResistanceUse));
        return;
      }

      if (oCaster is NwCreature caster)
      {
        spellDC = SpellUtils.GetCasterSpellDC(caster, spell, casterClass.SpellCastingAbility);
      }

      oTarget.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpPoisonL));
      SpellUtils.DealSpellDamage(target, oCaster.CasterLevel, spellEntry, SpellUtils.GetSpellDamageDiceNumber(oCaster, spell), oCaster, spell.GetSpellLevelForClass(casterClass.ClassType), 
        CreatureUtils.GetSavingThrowResult(target, spellEntry.savingThrowAbility, oCaster, spellDC, spellEntry));
    }
  }
}
