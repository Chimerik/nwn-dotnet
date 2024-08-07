﻿using Anvil.API;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static void BurningHands(NwGameObject oCaster, NwSpell spell, SpellEntry spellEntry, NwGameObject oTarget, NwClass casterClass, Location targetLocation, NwFeat feat = null)
    {     
      SpellUtils.SignalEventSpellCast(oTarget, oCaster, spell.SpellType);
      int damageDice = SpellUtils.GetSpellDamageDiceNumber(oCaster, spell);

      if (oCaster is NwCreature castingCreature && feat is not null && feat.Id == CustomSkill.MonkFrappeDesCendres)
      {
        castingCreature.IncrementRemainingFeatUses(feat.FeatType);
        FeatUtils.DecrementKi(castingCreature, 2);
        casterClass = NwClass.FromClassId(CustomClass.Monk);

        if (castingCreature.KnowsFeat((Feat)CustomSkill.MonkIncantationElementaire))
          damageDice += 1;
      }

      SpellConfig.SavingThrowFeedback feedback = new();
      int spellDC = SpellUtils.GetCasterSpellDC(oCaster, spell, casterClass.SpellCastingAbility);
      bool evocateur = oCaster is NwCreature caster && caster.KnowsFeat((Feat)CustomSkill.EvocateurFaconneurDeSorts);

      foreach (NwCreature target in targetLocation.GetObjectsInShapeByType<NwCreature>(Shape.SpellCone, 5, false, oCaster.Location.Position))
      {
        if (evocateur && oCaster is NwCreature casterCreature && !casterCreature.IsReactionTypeHostile(target))
          continue;

        int advantage = CreatureUtils.GetCreatureAbilityAdvantage(target, spellEntry.savingThrowAbility, spellEntry, SpellConfig.SpellEffectType.Invalid, oCaster, 1);

        if (advantage < -900)
          continue;

        int totalSave = SpellUtils.GetSavingThrowRoll(target, spellEntry.savingThrowAbility, spellDC, advantage, feedback, true);
        bool saveFailed = totalSave < spellDC;

        SpellUtils.SendSavingThrowFeedbackMessage(oCaster, target, feedback, advantage, spellDC, totalSave, saveFailed, spellEntry.savingThrowAbility);
        SpellUtils.DealSpellDamage(target, oCaster.CasterLevel, spellEntry, damageDice, oCaster, 1, saveFailed);
      }
    }
  }
}
