namespace NWN.Systems
{
  public static partial class SkillSystem
  {
    public static bool OnLearnRangerUrbanTracker(PlayerSystem.Player player, int customSkillId)
    {
      if(player.learnableSkills.TryAdd(CustomSkill.SleightOfHandProficiency, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.SleightOfHandProficiency], player)))
        player.learnableSkills[CustomSkill.SleightOfHandProficiency].LevelUp(player);
      player.learnableSkills[CustomSkill.SleightOfHandProficiency].source.Add(Category.Class);

      return true;
    }
  }
}
