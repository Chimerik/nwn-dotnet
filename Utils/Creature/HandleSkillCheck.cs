using Anvil.API;

namespace NWN.Systems
{
  public static partial class CreatureUtils
  {
    public static bool HandleSkillCheck(NwCreature creature, int skill, Ability ability, int DC)
    {
      SpellConfig.SavingThrowFeedback feedback = new();
      int advantage = GetCreatureAbilityAdvantage(creature, ability);
      int score = GetSkillScore(creature, ability, skill);
      int roll = GetSkillRoll(creature, skill, advantage, score, DC);
      int totalSave = roll + score;
      bool saveFailed = totalSave < DC;

      SendSkillCheckFeedback(creature, roll, score, advantage, DC, totalSave, saveFailed, SkillSystem.learnableDictionary[skill].name);

      return saveFailed;
    }
  }
}
