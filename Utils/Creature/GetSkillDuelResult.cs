using System.Collections.Generic;
using Anvil.API;
using NWN.Systems;

namespace NWN
{
  public static partial class CreatureUtils
  {
    public static bool GetSkillDuelResult(NwCreature attacker, NwCreature target, List<Ability> attackerAbilities,
      List<Ability> targetAbilities, List<int> attackerSkills, List<int> targetSkills, SpellConfig.SpellEffectType effectType = SpellConfig.SpellEffectType.Invalid)
    {
      int attackerScore = 0;
      Ability attackerAbility = Ability.Strength;
      int i = 0;

      foreach(Ability ability in attackerAbilities) 
      {
        int tempScore = GetSkillScore(attacker, ability, attackerSkills[i]);
        tempScore = tempScore > attackerScore ? tempScore : attackerScore;
        attackerAbility = tempScore > attackerScore ? attackerAbilities[i] : attackerAbility;
        i++;
      }

      int targetScore = 0;
      Ability targetAbility = Ability.Strength;
      i = 0;

      foreach (Ability ability in targetAbilities)
      {
        int tempScore = GetSkillScore(target, ability, targetSkills[i]);
        tempScore = tempScore > targetScore ? tempScore : targetScore;
        targetAbility = tempScore > targetScore ? targetAbilities[i] : targetAbility;
        i++;
      }

      int attackerAdvantage = GetCreatureAbilityAdvantage(attacker, attackerAbility);
      int targetAdvantage = GetCreatureAbilityAdvantage(target, targetAbility, effectType: SpellConfig.SpellEffectType.Knockdown);

      int attackerRoll = Utils.RollAdvantage(attackerAdvantage);
      int targetRoll = Utils.RollAdvantage(targetAdvantage);

      bool saveFailed = targetRoll + targetScore < attackerRoll + attackerScore;

      string attackerAdvantageString = attackerAdvantage == 0 ? "" : attackerAdvantage > 0 ? " (Avantage)".ColorString(StringUtils.gold) : " (Désavantage)".ColorString(ColorConstants.Red);
      string targetAdvantageString = targetAdvantage == 0 ? "" : targetAdvantage > 0 ? " (Avantage)".ColorString(StringUtils.gold) : " (Désavantage)".ColorString(ColorConstants.Red);
      string hitString = saveFailed ? "ECHEC".ColorString(ColorConstants.Red) : "REUSSI".ColorString(StringUtils.brightGreen);
      Color hitColor = saveFailed ? ColorConstants.Red : StringUtils.brightGreen;

      string rollString = $"JDS {StringUtils.TranslateAttributeToFrench(targetAbility)}{targetAdvantageString} {StringUtils.IntToColor(targetRoll, hitColor)} + {StringUtils.IntToColor(targetScore, hitColor)} = {StringUtils.IntToColor(targetRoll + targetScore, hitColor)} vs DD {StringUtils.IntToColor(attackerRoll + attackerScore, hitColor)}";

      attacker.LoginPlayer?.SendServerMessage($"{target.Name.ColorString(ColorConstants.Cyan)} - {attackerAdvantageString}{rollString} {hitString}".ColorString(ColorConstants.Orange));
      target.LoginPlayer?.SendServerMessage($"{attacker.Name.ColorString(ColorConstants.Cyan)} - {targetAdvantageString}{rollString} {hitString}".ColorString(ColorConstants.Orange));


      return saveFailed;
    }
  }
}
