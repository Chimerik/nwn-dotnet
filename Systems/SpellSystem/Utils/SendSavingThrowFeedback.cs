using System.Linq;
using Anvil.API;

namespace NWN.Systems
{
  public static partial class SpellUtils
  {
    public static void SendSavingThrowFeedbackMessage(NwCreature caster, NwCreature target, SpellConfig.SavingThrowFeedback feedback, int advantage, int spellDC, int totalSave, bool saveFailed, Ability ability)
    {
      string advantageString = advantage == 0 ? "" : advantage > 0 ? " (Avantage)".ColorString(StringUtils.gold) : " (Désavantage)".ColorString(ColorConstants.Red);
      string hitString = saveFailed ? "ECHEC".ColorString(ColorConstants.Red) : "REUSSI".ColorString(StringUtils.brightGreen);
      Color hitColor = saveFailed ? ColorConstants.Red : StringUtils.brightGreen;

      string rollString = $"JDS {StringUtils.TranslateAttributeToFrench(ability)}{advantageString} {StringUtils.IntToColor(feedback.saveRoll, hitColor)} + {StringUtils.IntToColor(feedback.proficiencyBonus, hitColor)} = {StringUtils.IntToColor(totalSave, hitColor)} vs DD {StringUtils.IntToColor(spellDC, hitColor)}";

      caster.LoginPlayer?.SendServerMessage($"{target.Name.ColorString(ColorConstants.Cyan)} - {rollString} {hitString}".ColorString(ColorConstants.Orange));

      if (target != caster)
        target.LoginPlayer?.SendServerMessage($"{caster.Name.ColorString(ColorConstants.Cyan)} - {advantageString}{rollString} {hitString}".ColorString(ColorConstants.Orange));

      if(saveFailed && caster.KnowsFeat(NwFeat.FromFeatId(CustomSkill.WildMagicMagieGalvanisanteBienfait)) 
        && caster.GetClassInfo(NwClass.FromClassType(ClassType.Barbarian))?.Level > 9
        && caster.GetObjectVariable<LocalVariableInt>(CreatureUtils.ReactionVariable).Value > 0
        && caster.ActiveEffects.Any(e => e.Tag == EffectSystem.BarbarianRageEffectTag))
      {
        BarbarianUtils.DispelWildMagicEffects(caster);
        SpellSystem.HandleWildMagicRage(caster);
        caster.GetObjectVariable<LocalVariableInt>(CreatureUtils.ReactionVariable).Value -= 1;
      }
    }
  }
}
