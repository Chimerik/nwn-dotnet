using System.Linq;
using Anvil.API;

namespace NWN.Systems
{
  public static partial class CreatureUtils
  {
    public static SavingThrowResult GetSavingThrowResult(NwCreature target, Ability ability, NwGameObject attacker = null, int saveDC = -1, SpellEntry spellEntry = null, SpellConfig.SpellEffectType effectType = SpellConfig.SpellEffectType.Invalid)
    {
      if (spellEntry is not null && attacker is not null && attacker is NwCreature caster)
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

      int advantage = GetCreatureSavingThrowAdvantage(target, ability, spellEntry, effectType, attacker);

      if (saveDC > 0 && advantage < -900)
        return SavingThrowResult.Immune;

      SpellConfig.SavingThrowFeedback feedback = new();

      int totalSave = SpellUtils.GetSavingThrowRoll(target, ability, saveDC, advantage, feedback, spellEntry);
      SavingThrowResult saveResult = (SavingThrowResult)(totalSave >= saveDC).ToInt();
      ModuleSystem.Log.Info(saveResult);
      if (saveResult == SavingThrowResult.Failure && target.ActiveEffects.Any(e => e.Tag == EffectSystem.InflexibleEffectTag))
      {
        StringUtils.DisplayStringToAllPlayersNearTarget(target, "Inflexible", StringUtils.gold, true, true);
        int fighterLevel = FighterUtils.GetFighterLevel(target);
        totalSave = SpellUtils.GetSavingThrowRoll(target, ability, saveDC, advantage, feedback, spellEntry) + fighterLevel;
        feedback.proficiencyBonus += fighterLevel;
        saveResult = (SavingThrowResult)(totalSave >= saveDC).ToInt();

        target.DecrementRemainingFeatUses((Feat)CustomSkill.FighterInflexible);
        EffectUtils.RemoveTaggedEffect(target, EffectSystem.InflexibleEffectTag);
      }

      if(saveDC > 0)
        SpellUtils.SendSavingThrowFeedbackMessage(attacker, target, feedback, advantage, saveDC, totalSave, saveResult, ability, effectType);
      else
      {
        string advantageString = advantage == 0 ? "" : advantage > 0 ? " (Avantage)".ColorString(StringUtils.gold) : " (Désavantage)".ColorString(ColorConstants.Red);
        StringUtils.BroadcastRollToPlayersInRange(target, $"{target.Name.ColorString(ColorConstants.Cyan)}{advantageString} - JDS {StringUtils.TranslateAttributeToFrench(ability)} - {StringUtils.ToWhitecolor(feedback.saveRoll)} + {StringUtils.ToWhitecolor(feedback.proficiencyBonus)} = {StringUtils.ToWhitecolor(feedback.saveRoll + feedback.proficiencyBonus)}", ColorConstants.Orange);
      }

      switch (ability)
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
