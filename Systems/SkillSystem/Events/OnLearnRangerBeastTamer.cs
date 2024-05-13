namespace NWN.Systems
{
  public static partial class SkillSystem
  {
    public static bool OnLearnRangerBeastTamer(PlayerSystem.Player player, int customSkillId)
    {
      if(player.learnableSkills.TryAdd(CustomSkill.AnimalHandlingProficiency, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.AnimalHandlingProficiency], player)))
        player.learnableSkills[CustomSkill.AnimalHandlingProficiency].LevelUp(player);
      player.learnableSkills[CustomSkill.AnimalHandlingProficiency].source.Add(Category.Class);

      if (player.learnableSkills.TryAdd(CustomSkill.RangerBeastTamerCallFamiliar, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.RangerBeastTamerCallFamiliar], player)))
        player.learnableSkills[CustomSkill.RangerBeastTamerCallFamiliar].LevelUp(player);
      player.learnableSkills[CustomSkill.RangerBeastTamerCallFamiliar].source.Add(Category.Class);

      return true;
    }
  }
}
