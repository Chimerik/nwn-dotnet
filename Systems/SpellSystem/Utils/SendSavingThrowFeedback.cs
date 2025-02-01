using System.Linq;
using Anvil.API;

namespace NWN.Systems
{
  public static partial class SpellUtils
  {
    public static void SendSavingThrowFeedbackMessage(NwGameObject oCaster, NwCreature target, SpellConfig.SavingThrowFeedback feedback, int advantage, int spellDC, int totalSave, SavingThrowResult saveResult, Ability ability, SpellConfig.SpellEffectType effectType)
    {
      bool saveFailed = saveResult == SavingThrowResult.Failure;
      string advantageString = advantage == 0 ? "" : advantage > 0 ? " (Avantage)".ColorString(StringUtils.gold) : " (Désavantage)".ColorString(ColorConstants.Red);
      string hitString = saveFailed ? "ECHEC".ColorString(ColorConstants.Red) : "REUSSI".ColorString(StringUtils.brightGreen);
      Color hitColor = saveFailed ? ColorConstants.Red : StringUtils.brightGreen;

      string effectTypeString = effectType != SpellConfig.SpellEffectType.Invalid ? $" ({effectType})" : "";
      string rollString = $"JDS {StringUtils.TranslateAttributeToFrench(ability)}{effectTypeString}{advantageString} {StringUtils.IntToColor(feedback.saveRoll, hitColor)} + {StringUtils.IntToColor(feedback.proficiencyBonus, hitColor)} = {StringUtils.IntToColor(totalSave, hitColor)} vs DD {StringUtils.IntToColor(spellDC, hitColor)}";

      if (target != oCaster)
        target.LoginPlayer?.SendServerMessage($"{target.Name.ColorString(ColorConstants.Cyan)} - {rollString} {hitString}".ColorString(ColorConstants.Orange));
      
      if (oCaster is NwCreature caster)
      {
        if (caster.IsPlayerControlled)
          caster.ControllingPlayer.SendServerMessage($"{target.Name.ColorString(ColorConstants.Cyan)} - {rollString} {hitString}".ColorString(ColorConstants.Orange));
        else
          caster.Master?.LoginPlayer?.SendServerMessage($"{target.Name.ColorString(ColorConstants.Cyan)} - {rollString} {hitString}".ColorString(ColorConstants.Orange));

        if (saveFailed && caster.KnowsFeat((Feat)CustomSkill.WildMagicMagieGalvanisanteBienfait)
          && caster.Classes.Any(c => c.Class.ClassType == ClassType.Barbarian && c.Level > 9)
          && caster.ActiveEffects.Any(e => e.Tag == EffectSystem.BarbarianRageEffectTag))
        {
          var reaction = caster.ActiveEffects.FirstOrDefault(e => e.Tag == EffectSystem.ReactionEffectTag);

          if (reaction is not null)
          {
            BarbarianUtils.DispelWildMagicEffects(caster);
            FeatSystem.HandleWildMagicRage(caster);
            caster.RemoveEffect(reaction);
          }
        }
      }

      switch(ability)
      {
        case Ability.Strength:
        case Ability.Dexterity: target.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpReflexSaveThrowUse)); break;
        case Ability.Constitution: target.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpFortitudeSavingThrowUse)); break;
        case Ability.Intelligence:
        case Ability.Wisdom:
        case Ability.Charisma: target.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpWillSavingThrowUse)); break;
      }

      LogUtils.LogMessage($"{target.Name} - {rollString} {hitString}".StripColors(), LogUtils.LogType.Combat);
    }
  }
}
