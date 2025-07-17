using Anvil.API;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static void SonneLeGlas(NwGameObject oCaster, NwSpell spell, SpellEntry spellEntry, NwGameObject oTarget, NwClass casterClass, NwFeat feat)
    {
      if (oCaster is not NwCreature caster || oTarget is not NwCreature target)
        return;

      int spellDC = SpellUtils.GetCasterSpellDC(caster, spell, SpellUtils.GetSpellCastAbility(oCaster, casterClass, feat));
      bool tourPuissant = caster.KnowsFeat((Feat)CustomSkill.EvocateurToursPuissants);

      SpellUtils.SignalEventSpellCast(oTarget, oCaster, spell.SpellType);

      SavingThrowResult saveResult = CreatureUtils.GetSavingThrowResult(target, spellEntry.savingThrowAbility, oCaster, spellDC, spellEntry);
      
      if (tourPuissant || saveResult == SavingThrowResult.Failure)
        SpellUtils.DealSpellDamage(target, oCaster.CasterLevel, spellEntry, SpellUtils.GetSpellDamageDiceNumber(oCaster, spell),
          oCaster, spell.GetSpellLevelForClass(casterClass.ClassType), saveResult, damageDice: target.HP < target.MaxHP ? 12 : 0);

      target.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpNegativeEnergy));
    }
  }
}
