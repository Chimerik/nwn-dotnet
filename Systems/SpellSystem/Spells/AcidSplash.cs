using Anvil.API;
using Anvil.API.Events;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static void AcidSplash(SpellEvents.OnSpellCast onSpellCast, SpellEntry spellEntry)
    {
      if (onSpellCast.Caster is not NwCreature oCaster)
        return;

      SpellUtils.SignalEventSpellCast(onSpellCast.TargetObject, oCaster, onSpellCast.Spell.SpellType);
      SpellConfig.SavingThrowFeedback feedback = new();
      int spellDC = SpellUtils.GetCasterSpellDC(oCaster, onSpellCast.SpellCastClass.SpellCastingAbility);
      bool tourPuissant = oCaster.KnowsFeat((Feat)CustomSkill.EvocateurToursPuissants);
      
      onSpellCast.TargetLocation.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.FnfGasExplosionAcid));

      foreach (NwCreature target in onSpellCast.TargetLocation.GetObjectsInShapeByType<NwCreature>(Shape.Sphere, spellEntry.aoESize, false))
      {
        int advantage = CreatureUtils.GetCreatureAbilityAdvantage(target, spellEntry.savingThrowAbility, spellEntry, SpellConfig.SpellEffectType.Invalid, oCaster);

        if (advantage < -900)
          continue;

        int totalSave = SpellUtils.GetSavingThrowRoll(target, spellEntry.savingThrowAbility, spellDC, advantage, feedback, true);
        bool saveFailed = totalSave < spellDC;

        SpellUtils.SendSavingThrowFeedbackMessage(oCaster, target, feedback, advantage, spellDC, totalSave, saveFailed, spellEntry.savingThrowAbility);

        if (saveFailed || tourPuissant) 
          SpellUtils.DealSpellDamage(target, oCaster.CasterLevel, spellEntry, SpellUtils.GetSpellDamageDiceNumber(oCaster, onSpellCast.Spell), oCaster, onSpellCast.Spell.GetSpellLevelForClass(onSpellCast.SpellCastClass), saveFailed);
      }
    }
  }
}
