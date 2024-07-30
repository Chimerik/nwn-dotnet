
using Anvil.API;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static void BouleDeFeu(NwGameObject oCaster, NwSpell spell, SpellEntry spellEntry, NwGameObject oTarget, NwClass casterClass, Location targetLocation, NwFeat feat = null)
    {
      if (oCaster is NwCreature castingCreature && feat is not null && feat.Id == CustomSkill.MonkFlammesDuPhenix)
      {
        castingCreature.IncrementRemainingFeatUses(feat.FeatType);
        FeatUtils.DecrementKi(castingCreature, 4);
        casterClass = NwClass.FromClassId(CustomClass.Monk);
      }

      SpellUtils.SignalEventSpellCast(oTarget, oCaster, spell.SpellType);
      SpellConfig.SavingThrowFeedback feedback = new();
      int spellDC = SpellUtils.GetCasterSpellDC(oCaster, spell, casterClass.SpellCastingAbility);
      bool evocateur = oCaster is NwCreature caster && caster.KnowsFeat((Feat)CustomSkill.EvocateurFaconneurDeSorts);

      foreach (NwCreature target in targetLocation.GetObjectsInShapeByType<NwCreature>(Shape.Sphere, spellEntry.aoESize, false))
      {
        if (evocateur && oCaster is NwCreature casterCreature && !casterCreature.IsReactionTypeHostile(target))
          continue;

        int advantage = CreatureUtils.GetCreatureAbilityAdvantage(target, spellEntry.savingThrowAbility, spellEntry, SpellConfig.SpellEffectType.Invalid, oCaster, spell.GetSpellLevelForClass(casterClass.ClassType));

        if (advantage < -900)
          continue;

        int totalSave = SpellUtils.GetSavingThrowRoll(target, spellEntry.savingThrowAbility, spellDC, advantage, feedback, true);
        bool saveFailed = totalSave < spellDC;

        SpellUtils.SendSavingThrowFeedbackMessage(oCaster, target, feedback, advantage, spellDC, totalSave, saveFailed, spellEntry.savingThrowAbility);
        SpellUtils.DealSpellDamage(target, oCaster.CasterLevel, spellEntry, SpellUtils.GetSpellDamageDiceNumber(oCaster, spell), oCaster, spell.GetSpellLevelForClass(casterClass), saveFailed);
      }

      targetLocation.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.FnfFireball));
    }
  }
}
