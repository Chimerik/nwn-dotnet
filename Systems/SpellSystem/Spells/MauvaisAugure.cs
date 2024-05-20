using Anvil.API;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static void MauvaisAugure(NwGameObject oCaster, NwSpell spell, SpellEntry spellEntry, NwGameObject oTarget)
    {
      if (oTarget is not NwCreature target)
        return;

      SpellUtils.SignalEventSpellCast(oTarget, oCaster, spell.SpellType);
      SpellConfig.SavingThrowFeedback feedback = new();
      int spellDC = SpellUtils.GetCasterSpellDC(oCaster, spell, Ability.Wisdom);
      int advantage = CreatureUtils.GetCreatureAbilityAdvantage(target, spellEntry.savingThrowAbility, spellEntry, SpellConfig.SpellEffectType.Invalid, oCaster);

      int totalSave = SpellUtils.GetSavingThrowRoll(target, spellEntry.savingThrowAbility, spellDC, advantage, feedback, true);
      bool saveFailed = totalSave < spellDC;

      if (saveFailed)
      {
        target.ApplyEffect(EffectDuration.Temporary, EffectSystem.MauvaisAugure, NwTimeSpan.FromRounds(2));
        target.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpPulseNegative));
      }
      else
        target.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ComHitNegative));

      SpellUtils.SendSavingThrowFeedbackMessage(oCaster, target, feedback, advantage, spellDC, totalSave, saveFailed, spellEntry.savingThrowAbility);
      SpellUtils.DealSpellDamage(target, oCaster.CasterLevel, spellEntry, SpellUtils.GetSpellDamageDiceNumber(oCaster, spell), oCaster, 0, saveFailed);
    }
  }
}
