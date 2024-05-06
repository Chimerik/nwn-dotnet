using System.Collections.Generic;
using Anvil.API;

namespace NWN.Systems
{
  public static partial class CreatureUtils
  {
    public static bool GetSkillDuelResult(NwCreature attacker, NwCreature target, List<Ability> attackerAbilities,
      List<Ability> targetAbilities, List<int> attackerSkills, List<int> targetSkills, SpellConfig.SpellEffectType effectType = SpellConfig.SpellEffectType.Invalid,
      bool silentThrow = false)
    {
      int attackerScore = 0;
      int attackerSkill = 0;
      Ability attackerAbility = Ability.Strength;
      int i = 0;

      foreach(Ability ability in attackerAbilities) 
      {
        int tempScore = GetSkillScore(attacker, ability, attackerSkills[i]);

        if(tempScore > attackerScore)
        {
          attackerAbility = attackerAbilities[i];
          attackerSkill = attackerSkills[i];
          attackerScore = tempScore;
        }
        
        i++;
      }

      int targetScore = 0;
      int targetSkill = 0;
      Ability targetAbility = Ability.Strength;
      i = 0;

      foreach (Ability ability in targetAbilities)
      {
        int tempScore = GetSkillScore(target, ability, targetSkills[i]);

        if (tempScore > targetScore)
        {
          targetAbility = targetAbilities[i];
          targetSkill = targetSkills[i];
          targetScore = tempScore;
        }

        i++;
      }

      int attackerAdvantage = GetCreatureAbilityAdvantage(attacker, attackerAbility, effectType: effectType);
      int targetAdvantage = GetCreatureAbilityAdvantage(target, targetAbility, effectType: effectType);

      if (attackerSkill == CustomSkill.StealthProficiency)
      {
        List<string> effLink = new();
        foreach (var eff in attacker.ActiveEffects)
          if (!eff.LinkId.Contains(eff.LinkId) && eff.EffectType == EffectType.SkillIncrease && eff.IntParams[0] == 8)
          {
            attackerScore += eff.IntParams[1];
            effLink.Add(eff.LinkId);
          }
      }

      if (targetSkill == CustomSkill.StealthProficiency)
      {
        List<string> effLink = new();

        foreach (var eff in target.ActiveEffects)
          if (!eff.LinkId.Contains(eff.LinkId) && eff.EffectType == EffectType.SkillIncrease && eff.IntParams[0] == 8)
          {
            targetScore += eff.IntParams[1];
            effLink.Add(eff.LinkId);
          }
      }

      int attackerRoll = GetSkillRoll(attacker, attackerSkill, attackerAdvantage, attackerScore, 0);
      int targetRoll = GetSkillRoll(target, targetSkill, targetAdvantage, targetScore, attackerRoll + attackerScore);
      bool saveFailed = targetRoll + targetScore < attackerRoll + attackerScore;

      string attackerAdvantageString = attackerAdvantage == 0 ? "" : attackerAdvantage > 0 ? " (Avantage)".ColorString(StringUtils.gold) : " (Désavantage)".ColorString(ColorConstants.Red);
      string targetAdvantageString = targetAdvantage == 0 ? "" : targetAdvantage > 0 ? " (Avantage)".ColorString(StringUtils.gold) : " (Désavantage)".ColorString(ColorConstants.Red);
      string hitString = saveFailed ? "REUSSI".ColorString(StringUtils.brightGreen) : "ECHEC".ColorString(ColorConstants.Red);
      Color hitColor = saveFailed ? ColorConstants.Red : StringUtils.brightGreen;

      string rollString = $"{attacker.Name.ColorString(ColorConstants.Cyan)} {SkillSystem.learnableDictionary[attackerSkill].name.Replace(" - Maîtrise", "")} ({StringUtils.TranslateAttributeToFrench(attackerAbility)}){attackerAdvantageString} " +
        $"{StringUtils.IntToColor(attackerRoll, hitColor)} + {StringUtils.IntToColor(attackerScore, hitColor)} = {StringUtils.IntToColor(attackerRoll + attackerScore, hitColor)} " +
        $"vs {target.Name.ColorString(ColorConstants.Cyan)} {SkillSystem.learnableDictionary[targetSkill].name.Replace(" - Maîtrise", "")} ({StringUtils.TranslateAttributeToFrench(targetAbility)}){targetAdvantageString} " +
        $"{StringUtils.IntToColor(targetRoll, hitColor)} + {StringUtils.IntToColor(targetScore, hitColor)} = {StringUtils.IntToColor(targetRoll + targetScore, hitColor)}";

      if(!silentThrow)
        attacker.LoginPlayer?.SendServerMessage($"{rollString} {hitString}".ColorString(ColorConstants.Orange));
      
      target.LoginPlayer?.SendServerMessage($"{rollString} {(saveFailed ? "ECHEC".ColorString(ColorConstants.Red) : "REUSSI".ColorString(StringUtils.brightGreen))}".ColorString(ColorConstants.Orange));

      LogUtils.LogMessage($"{rollString} {hitString}".StripColors(), LogUtils.LogType.Combat);

      return saveFailed;
    }
  }
}
