using Anvil.API;
using Anvil.API.Events;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static void PoisonSpray(SpellEvents.OnSpellCast onSpellCast, SpellEntry spellEntry)
    {
      if (onSpellCast.Caster is not NwCreature oCaster || onSpellCast.TargetObject is not NwCreature target)
        return;

      SpellUtils.SignalEventSpellCast(onSpellCast.TargetObject, oCaster, onSpellCast.Spell.SpellType);
      SpellConfig.SavingThrowFeedback feedback = new();
      int spellDC = SpellUtils.GetCasterSpellDC(oCaster, onSpellCast.Spell);

      target.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpPoisonS));

      int advantage = CreatureUtils.GetCreatureAbilityAdvantage(target, spellEntry, SpellConfig.SpellEffectType.Poison);
      int totalSave = SpellUtils.GetSavingThrowRoll(target, spellEntry, advantage, feedback);
      bool saveFailed = totalSave < spellDC;

      SpellUtils.SendSavingThrowFeedbackMessage(oCaster, target, feedback, advantage, spellDC, totalSave, saveFailed, spellEntry.savingThrowAbility);

      if (saveFailed) 
        SpellUtils.DealSpellDamage(target, oCaster.LastSpellCasterLevel, spellEntry, SpellUtils.GetSpellDamageDiceNumber(oCaster, onSpellCast.Spell));
    }
  }
}
