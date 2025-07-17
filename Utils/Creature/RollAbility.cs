using Anvil.API;

namespace NWN.Systems
{
  public static partial class CreatureUtils
  {
    public static bool RollAbility(NwCreature creature, Ability ability, int DC = -1, int skill = -1)
    {
      int advantage = GetCreatureAbilityAdvantage(creature, ability, skill);
      int score = GetSkillScore(creature, ability, skill);
      int roll = GetSkillRoll(creature, skill, advantage, score, DC);

      if (DC > 0)
      {
        int totalSave = roll + score;
        bool saveFailed = totalSave < DC;

        SendAbilityCheckFeedback(creature, roll, score, advantage, DC, totalSave, saveFailed, skill);
        return saveFailed;
      }
      else
      {
        string advantageString = advantage == 0 ? "" : advantage > 0 ? " (Avantage)".ColorString(StringUtils.gold) : " (Désavantage)".ColorString(ColorConstants.Red);
        string skillString = skill < 0 ? "" : $" ({SkillSystem.learnableDictionary[skill].name.Replace(" - Maîtrise", "")})";
        StringUtils.BroadcastRollToPlayersInRange(creature, $"{creature.Name.ColorString(ColorConstants.Cyan)}{advantageString} - Jet de {StringUtils.TranslateAttributeToFrench(ability)}{skillString} - {StringUtils.ToWhitecolor(roll)} + {StringUtils.ToWhitecolor(score)} = {StringUtils.ToWhitecolor(roll + score)}", ColorConstants.Orange);
      }

      return true;
    }
  }
}
