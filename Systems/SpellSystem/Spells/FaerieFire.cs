using System.Collections.Generic;
using Anvil.API;
using Anvil.API.Events;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static void FaerieFire(SpellEvents.OnSpellCast onSpellCast, SpellEntry spellEntry)
    {
      if (onSpellCast.Caster is not NwCreature oCaster)
        return;

      SpellUtils.SignalEventSpellCast(onSpellCast.TargetObject, oCaster, onSpellCast.Spell.SpellType);
      SpellConfig.SavingThrowFeedback feedback = new();
      List<NwGameObject> targetList = new();
      int spellDC = SpellUtils.GetCasterSpellDC(oCaster, onSpellCast.SpellCastClass.SpellCastingAbility);
      
      onSpellCast.TargetLocation.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpDustExplosion));

      foreach (NwCreature target in onSpellCast.TargetLocation.GetObjectsInShapeByType<NwCreature>(Shape.Cube, spellEntry.aoESize, false))
      {
        int advantage = CreatureUtils.GetCreatureAbilityAdvantage(target, spellEntry.savingThrowAbility, spellEntry, SpellConfig.SpellEffectType.Invalid, oCaster);

        if (advantage < -900)
          continue;

        int totalSave = SpellUtils.GetSavingThrowRoll(target, spellEntry.savingThrowAbility, spellDC, advantage, feedback, true);
        bool saveFailed = totalSave < spellDC;

        SpellUtils.SendSavingThrowFeedbackMessage(oCaster, target, feedback, advantage, spellDC, totalSave, saveFailed, spellEntry.savingThrowAbility);

        if (saveFailed)
        {
          ApplyFaerieFireEffect(target, spellEntry);
          targetList.Add(target);
        }
      }

      EffectSystem.ApplyConcentrationEffect(oCaster, onSpellCast.Spell.Id, targetList, spellEntry.duration);
    }
    public static void ApplyFaerieFireEffect(NwCreature target, SpellEntry spellEntry)
    {
      target.ApplyEffect(EffectDuration.Temporary, EffectSystem.faerieFireEffect, NwTimeSpan.FromRounds(spellEntry.duration));
      target.OnEffectApply += EffectSystem.CheckFaerieFire;

      foreach (var eff in target.ActiveEffects)
        if (eff.EffectType == EffectType.Invisibility || eff.EffectType == EffectType.ImprovedInvisibility)
          target.RemoveEffect(eff);
    }
  }
}
