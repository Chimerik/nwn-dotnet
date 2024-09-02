using System.Linq;
using Anvil.API;

namespace NWN.Systems
{
  public static partial class SpellUtils
  {
    public static void SendSavingThrowFeedbackMessage(NwGameObject oCaster, NwCreature target, SpellConfig.SavingThrowFeedback feedback, int advantage, int spellDC, int totalSave, SavingThrowResult saveResult, Ability ability)
    {
      bool saveFailed = saveResult == SavingThrowResult.Failure;
      string advantageString = advantage == 0 ? "" : advantage > 0 ? " (Avantage)".ColorString(StringUtils.gold) : " (Désavantage)".ColorString(ColorConstants.Red);
      string hitString = saveFailed ? "ECHEC".ColorString(ColorConstants.Red) : "REUSSI".ColorString(StringUtils.brightGreen);
      Color hitColor = saveFailed ? ColorConstants.Red : StringUtils.brightGreen;

      string rollString = $"JDS {StringUtils.TranslateAttributeToFrench(ability)}{advantageString} {StringUtils.IntToColor(feedback.saveRoll, hitColor)} + {StringUtils.IntToColor(feedback.proficiencyBonus, hitColor)} = {StringUtils.IntToColor(totalSave, hitColor)} vs DD {StringUtils.IntToColor(spellDC, hitColor)}";

      if (target != oCaster)
        target.LoginPlayer?.SendServerMessage($"{oCaster.Name.ColorString(ColorConstants.Cyan)} - {rollString} {hitString}".ColorString(ColorConstants.Orange));

      if (oCaster is NwCreature caster)
      {
        if(caster.IsPlayerControlled)
          caster.LoginPlayer.SendServerMessage($"{target.Name.ColorString(ColorConstants.Cyan)} - {rollString} {hitString}".ColorString(ColorConstants.Orange));
        else
          caster.Master?.LoginPlayer?.SendServerMessage($"{target.Name.ColorString(ColorConstants.Cyan)} - {rollString} {hitString}".ColorString(ColorConstants.Orange));

        if (saveFailed && caster.KnowsFeat((Feat)CustomSkill.WildMagicMagieGalvanisanteBienfait)
          && caster.Classes.Any(c => c.Class.ClassType == ClassType.Barbarian && c.Level > 9)
          && caster.GetObjectVariable<LocalVariableInt>(CreatureUtils.ReactionVariable).Value > 0
          && caster.ActiveEffects.Any(e => e.Tag == EffectSystem.BarbarianRageEffectTag))
        {
          BarbarianUtils.DispelWildMagicEffects(caster);
          FeatSystem.HandleWildMagicRage(caster);
          caster.GetObjectVariable<LocalVariableInt>(CreatureUtils.ReactionVariable).Value -= 1;
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

      LogUtils.LogMessage($"{target.Name} - {advantageString}{rollString} {hitString}".StripColors(), LogUtils.LogType.Combat);
    }
  }
}
