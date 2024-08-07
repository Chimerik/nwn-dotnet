﻿using Anvil.API;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static void FlameStrike(NwGameObject oCaster, NwSpell spell, SpellEntry spellEntry, NwGameObject oTarget, NwClass casterClass, Location targetLocation)
    {     
      SpellUtils.SignalEventSpellCast(oTarget, oCaster, spell.SpellType);
      SpellConfig.SavingThrowFeedback feedback = new();
      int spellDC = SpellUtils.GetCasterSpellDC(oCaster, spell, casterClass.SpellCastingAbility);
      bool evocateur = oCaster is NwCreature caster && caster.KnowsFeat((Feat)CustomSkill.EvocateurFaconneurDeSorts);

      targetLocation.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpDivineStrikeFire));

      foreach (NwCreature target in targetLocation.GetObjectsInShapeByType<NwCreature>(Shape.Sphere, spellEntry.aoESize, false))
      {
        if (evocateur && oCaster is NwCreature casterCreature && !casterCreature.IsReactionTypeHostile(target))
          continue;

        int advantage = CreatureUtils.GetCreatureAbilityAdvantage(target, spellEntry.savingThrowAbility, spellEntry, SpellConfig.SpellEffectType.Invalid, oCaster, spell.GetSpellLevelForClass(casterClass.ClassType));
        target.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpFlameS));

        if (advantage < -900)
          continue;

        int totalSave = SpellUtils.GetSavingThrowRoll(target, spellEntry.savingThrowAbility, spellDC, advantage, feedback, true);
        bool saveFailed = totalSave < spellDC;

        SpellUtils.SendSavingThrowFeedbackMessage(oCaster, target, feedback, advantage, spellDC, totalSave, saveFailed, spellEntry.savingThrowAbility);
        SpellUtils.DealSpellDamage(target, oCaster.CasterLevel, spellEntry, SpellUtils.GetSpellDamageDiceNumber(oCaster, spell), oCaster, spell.GetSpellLevelForClass(casterClass), saveFailed);
      }
    }
  }
}
