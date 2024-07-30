using Anvil.API;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static void Fracassement(NwGameObject oCaster, NwSpell spell, SpellEntry spellEntry, NwClass casterClass, Location targetLocation, NwFeat feat = null)
    {
      if (oCaster is not NwCreature caster)
        return;

      int damageDice = SpellUtils.GetSpellDamageDiceNumber(oCaster, spell);

      if (feat is not null && feat.Id == CustomSkill.MonkGongDuSommet)
      {
        caster.IncrementRemainingFeatUses(feat.FeatType);
        FeatUtils.DecrementKi(caster, 3);
        casterClass = NwClass.FromClassId(CustomClass.Monk);

        if (caster.KnowsFeat((Feat)CustomSkill.MonkIncantationElementaire))
          damageDice += 1;
      }

      SpellUtils.SignalEventSpellCast(oCaster, oCaster, spell.SpellType);
      SpellConfig.SavingThrowFeedback feedback = new();
      int spellDC = SpellUtils.GetCasterSpellDC(oCaster, spell, casterClass.SpellCastingAbility);
      bool evocateur = caster.KnowsFeat((Feat)CustomSkill.EvocateurFaconneurDeSorts);

      targetLocation.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.FnfSoundBurst));

      foreach (NwCreature target in targetLocation.GetObjectsInShapeByType<NwCreature>(Shape.Sphere, spellEntry.aoESize, false))
      {
        if (evocateur && oCaster is NwCreature casterCreature && !casterCreature.IsReactionTypeHostile(target))
          continue;

        SpellUtils.DealSpellDamage(target, oCaster.CasterLevel, spellEntry, damageDice, oCaster, 2, CreatureUtils.GetSavingThrow(caster, target, spellEntry.savingThrowAbility, spellDC));

        target.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpHeadNature));
        target.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.FnfScreenBump));
      }
    }
  }
}
