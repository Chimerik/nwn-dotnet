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
        var spellSchool = NwSpell.FromSpellId(spellEntry.RowIndex).SpellSchool;

        if(spellSchool == SpellSchool.Evocation && caster.KnowsFeat((Feat)CustomSkill.EvocateurFaconneurDeSorts)
          && !caster.IsReactionTypeHostile(target))
          return SavingThrowResult.Immune;

        if (caster.ActiveEffects.Any(e => e.Tag == EffectSystem.MetamagieEffectTag && e.IntParams[5] == CustomSkill.EnsoPrudence))
        {
          EffectUtils.RemoveTaggedParamEffect(caster, CustomSkill.EnsoPrudence, EffectSystem.MetamagieEffectTag);
          return SavingThrowResult.Immune;
        }

        if (spellSchool == SpellSchool.Illusion && target.KnowsFeat((Feat)CustomSkill.OeilDeSorciere))
          return SavingThrowResult.Success;

        if(spellEntry.RowIndex == (int)Spell.Sleep && (Utils.In(target.Race.RacialType, RacialType.Elf, RacialType.Undead, RacialType.Construct)
          || target.ActiveEffects.Any(e => e.EffectType == EffectType.Immunity && e.IntParams[1] == 13)))
          return SavingThrowResult.Immune;
      }

      SpellConfig.SavingThrowFeedback feedback = new();
      int advantage = GetCreatureAbilityAdvantage(target, ability, spellEntry, effectType, attacker);
      
      if (advantage < -900)
        return SavingThrowResult.Immune;

      int totalSave = SpellUtils.GetSavingThrowRoll(target, ability, saveDC, advantage, feedback);
      SavingThrowResult saveResult = (SavingThrowResult)(totalSave >= saveDC).ToInt();

      SpellUtils.SendSavingThrowFeedbackMessage(attacker, target, feedback, advantage, saveDC, totalSave, saveResult, ability, effectType);

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
