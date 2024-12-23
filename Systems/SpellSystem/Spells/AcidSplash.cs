using Anvil.API;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static void AcidSplash(NwGameObject oCaster, NwSpell spell, SpellEntry spellEntry, Location targetLocation, NwClass casterClass)
    {
      int spellDC = 10;
      bool tourPuissant = false;

      SpellUtils.SignalEventSpellCast(oCaster, oCaster, spell.SpellType);

      if (oCaster is NwCreature caster)
      {
        spellDC = SpellUtils.GetCasterSpellDC(caster, spell, casterClass.SpellCastingAbility);
        tourPuissant = caster.KnowsFeat((Feat)CustomSkill.EvocateurToursPuissants);
      }
      
      targetLocation.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.FnfGasExplosionAcid));

      foreach (NwCreature target in targetLocation.GetObjectsInShapeByType<NwCreature>(Shape.Sphere, spellEntry.aoESize, false))
      {
        SavingThrowResult saveResult = CreatureUtils.GetSavingThrow(oCaster, target, spellEntry.savingThrowAbility, spellDC, spellEntry);

        if (saveResult == SavingThrowResult.Failure || tourPuissant) 
          SpellUtils.DealSpellDamage(target, oCaster.CasterLevel, spellEntry, SpellUtils.GetSpellDamageDiceNumber(oCaster, spell), oCaster, spell.GetSpellLevelForClass(casterClass.ClassType), saveResult);
      }
    }
  }
}
