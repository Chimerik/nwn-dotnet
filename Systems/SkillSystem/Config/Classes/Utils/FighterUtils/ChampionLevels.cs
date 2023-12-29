using static NWN.Systems.PlayerSystem;
using static NWN.Systems.PlayerSystem.Player;
using static NWN.Systems.SkillSystem;

namespace NWN.Systems
{
  public static partial class Fighter
  {
    public static void HandleChampionLevelUp(Player player, int level)
    {
      switch (level)
      {
        case 3:

          player.learnableSkills.TryAdd(CustomSkill.FighterChampionImprovedCritical, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.FighterChampionImprovedCritical]));
          player.learnableSkills[CustomSkill.FighterChampionImprovedCritical].LevelUp(player);
          player.learnableSkills[CustomSkill.FighterChampionImprovedCritical].source.Add(Category.Class);

          break;

        case 7:

          player.learnableSkills.TryAdd(CustomSkill.FighterChampionRemarkableAthlete, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.FighterChampionRemarkableAthlete]));
          player.learnableSkills[CustomSkill.FighterChampionRemarkableAthlete].LevelUp(player);
          player.learnableSkills[CustomSkill.FighterChampionRemarkableAthlete].source.Add(Category.Class);

          break;

        case 10:

          player.learnableSkills.TryAdd(CustomSkill.FighterChampionBonusCombatStyle, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.FighterChampionBonusCombatStyle]));
          player.learnableSkills[CustomSkill.FighterChampionBonusCombatStyle].LevelUp(player);
          player.learnableSkills[CustomSkill.FighterChampionBonusCombatStyle].source.Add(Category.Class);

          if (!player.windows.TryGetValue("martialInitiateChoice", out var value)) player.windows.Add("martialInitiateChoice", new MartialInitiateChoiceWindow(player, player.oid.LoginCreature.Level));
          else ((MartialInitiateChoiceWindow)value).CreateWindow(player.oid.LoginCreature.Level);

          break;

        case 15:

          player.learnableSkills.TryAdd(CustomSkill.FighterChampionImprovedCritical, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.FighterChampionImprovedCritical]));
          player.learnableSkills[CustomSkill.FighterChampionImprovedCritical].LevelUp(player);

          break;

        case 18:

          player.learnableSkills.TryAdd(CustomSkill.FighterChampionUltimeSurvivant, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.FighterChampionUltimeSurvivant]));
          player.learnableSkills[CustomSkill.FighterChampionUltimeSurvivant].LevelUp(player);
          player.learnableSkills[CustomSkill.FighterChampionUltimeSurvivant].source.Add(Category.Class);

          player.oid.LoginCreature.OnHeartbeat -= FighterUtils.OnHeartbeatUltimeSurvivant;
          player.oid.LoginCreature.OnHeartbeat += FighterUtils.OnHeartbeatUltimeSurvivant;

          break;
      }
    }
  }
}
