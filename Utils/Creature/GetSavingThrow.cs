using System.Linq;
using Anvil.API;

namespace NWN.Systems
{
  public static partial class CreatureUtils
  {
    public static SavingThrowResult GetSavingThrow(NwGameObject attacker, NwCreature target, Ability ability, int saveDC, SpellEntry spellEntry = null, SpellConfig.SpellEffectType effectType = SpellConfig.SpellEffectType.Invalid)
    {
      if (spellEntry is not null && attacker is NwCreature caster)
      {
        if(NwSpell.FromSpellId(spellEntry.RowIndex).SpellSchool == SpellSchool.Evocation && caster.KnowsFeat((Feat)CustomSkill.EvocateurFaconneurDeSorts)
          && !caster.IsReactionTypeHostile(target))
          return SavingThrowResult.Immune;

        if (caster.ActiveEffects.Any(e => e.Tag == EffectSystem.MetamagieEffectTag && e.IntParams[5] == CustomSkill.EnsoPrudence))
        {
          EffectUtils.RemoveTaggedParamEffect(caster, CustomSkill.EnsoPrudence, EffectSystem.MetamagieEffectTag);
          return SavingThrowResult.Immune;
        }
      }      

      SpellConfig.SavingThrowFeedback feedback = new();
      int advantage = GetCreatureAbilityAdvantage(target, ability, spellEntry, effectType, attacker);
      
      if (advantage < -900)
        return SavingThrowResult.Immune;

      int totalSave = SpellUtils.GetSavingThrowRoll(target, ability, saveDC, advantage, feedback);
      SavingThrowResult saveResult = (SavingThrowResult)(totalSave >= saveDC).ToInt();

      SpellUtils.SendSavingThrowFeedbackMessage(attacker, target, feedback, advantage, saveDC, totalSave, saveResult, ability);

      switch(ability)
      {
        case Ability.Strength:
        case Ability.Constitution:
          target.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpFortitudeSavingThrowUse));
          break;

        case Ability.Dexterity:
          target.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpReflexSaveThrowUse));
          break;

        case Ability.Intelligence:
        case Ability.Wisdom:
        case Ability.Charisma:
          target.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpWillSavingThrowUse));
          break;
      }

      return saveResult;
    }
  }
}
