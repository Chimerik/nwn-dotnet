using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string KnockdownEffectTag = "_KNOCKDOWN_EFFECT";
    public static Effect knockdown
    {
      get
      {
        Effect eff = Effect.Knockdown();
        eff.Tag = KnockdownEffectTag;
        eff.SubType = EffectSubType.Supernatural;
        return eff;
      }
    }
    public static void ApplyKnockdown(NwCreature caster, NwCreature target)
    {
      int casterAthletics = CreatureUtils.GetSkillScore(caster, Ability.Strength, CustomSkill.AthleticsProficiency);
      int targetAthletics = CreatureUtils.GetSkillScore(target, Ability.Strength, CustomSkill.AthleticsProficiency);
      int targetAcrobatics = CreatureUtils.GetSkillScore(target, Ability.Dexterity, CustomSkill.AcrobaticsProficiency);
      int targetScore = targetAthletics > targetAcrobatics ? targetAthletics : targetAcrobatics;
      Ability targetAbility = targetAthletics > targetAcrobatics ? Ability.Strength : Ability.Dexterity;

      int casterAdvantage = CreatureUtils.GetCreatureAbilityAdvantage(caster, Ability.Strength);
      int targetAdvantage = CreatureUtils.GetCreatureAbilityAdvantage(target, targetAbility);

      int casterRoll = Utils.RollAdvantage(casterAdvantage);
      int targetRoll = Utils.RollAdvantage(targetAdvantage);

      bool saveFailed = casterRoll + casterAthletics > targetRoll + targetScore;

      if (saveFailed)
        target.ApplyEffect(EffectDuration.Temporary, knockdown, NwTimeSpan.FromRounds(1));

      string casterAdvantageString = casterAdvantage == 0 ? "" : casterAdvantage > 0 ? " (Avantage)".ColorString(StringUtils.gold) : " (Désavantage)".ColorString(ColorConstants.Red);
      string targetAdvantageString = targetAdvantage == 0 ? "" : targetAdvantage > 0 ? " (Avantage)".ColorString(StringUtils.gold) : " (Désavantage)".ColorString(ColorConstants.Red);
      string hitString = saveFailed ? "ECHEC".ColorString(ColorConstants.Red) : "REUSSI".ColorString(StringUtils.brightGreen);
      Color hitColor = saveFailed ? ColorConstants.Red : StringUtils.brightGreen;

      string rollString = $"JDS {StringUtils.TranslateAttributeToFrench(targetAbility)}{targetAdvantageString} {StringUtils.IntToColor(targetRoll, hitColor)} + {StringUtils.IntToColor(targetScore, hitColor)} = {StringUtils.IntToColor(targetRoll + targetScore, hitColor)} vs DD {StringUtils.IntToColor(casterRoll + casterAthletics, hitColor)}";

      caster.LoginPlayer?.SendServerMessage($"{target.Name.ColorString(ColorConstants.Cyan)} - {rollString} {hitString}".ColorString(ColorConstants.Orange));
      target.LoginPlayer?.SendServerMessage($"{caster.Name.ColorString(ColorConstants.Cyan)} - {targetAdvantageString}{rollString} {hitString}".ColorString(ColorConstants.Orange));
    }
  }
}
