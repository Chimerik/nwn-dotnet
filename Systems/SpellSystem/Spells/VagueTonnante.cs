﻿using Anvil.API;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static void VagueTonnante(NwGameObject oCaster, NwSpell spell, SpellEntry spellEntry, NwClass casterClass, NwFeat feat = null)
    {
      if (oCaster is not NwCreature caster)
        return;

      int damageDice = SpellUtils.GetSpellDamageDiceNumber(oCaster, spell);

      if (feat is not null && feat.Id == CustomSkill.MonkPoingDesQuatreTonnerres)
      {
        caster.IncrementRemainingFeatUses(feat.FeatType);
        FeatUtils.DecrementKi(caster, 2);
        casterClass = NwClass.FromClassId(CustomClass.Monk);

        if (caster.KnowsFeat((Feat)CustomSkill.MonkIncantationElementaire))
          damageDice += 1;
      }

      SpellUtils.SignalEventSpellCast(oCaster, oCaster, spell.SpellType);
      int spellDC = SpellUtils.GetCasterSpellDC(oCaster, spell, casterClass.SpellCastingAbility);
      bool evocateur = caster.KnowsFeat((Feat)CustomSkill.EvocateurFaconneurDeSorts);

      caster.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.FnfSoundBurst));

      foreach (NwCreature target in oCaster.Location.GetObjectsInShapeByType<NwCreature>(Shape.Cube, spellEntry.aoESize, false))
      {
        if (target == caster || (evocateur && !caster.IsReactionTypeHostile(target)))
          continue;

        SavingThrowResult saveResult = CreatureUtils.GetSavingThrowResult(target, spellEntry.savingThrowAbility, caster, spellDC);
        SpellUtils.DealSpellDamage(target, oCaster.CasterLevel, spellEntry, damageDice, oCaster, 1, saveResult);

        if (saveResult == SavingThrowResult.Failure)
          EffectSystem.ApplyKnockdown(target, caster, casterClass.SpellCastingAbility, spellEntry.savingThrowAbility, EffectSystem.Destabilisation, true);

        target.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpSonic));
        target.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpBlindDeafM));
        target.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.FnfScreenBump));
      }
    }
  }
}
