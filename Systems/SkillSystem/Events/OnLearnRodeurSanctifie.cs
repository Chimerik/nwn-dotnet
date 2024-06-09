namespace NWN.Systems
{
  public static partial class SkillSystem
  {
    public static bool OnLearnRodeurSanctifie(PlayerSystem.Player player, int customSkillId)
    {
      if (player.learnableSkills.TryAdd(CustomSkill.SacredFlame, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.SacredFlame], player)))
        player.learnableSkills[CustomSkill.SacredFlame].LevelUp(player);
      player.learnableSkills[CustomSkill.SacredFlame].source.Add(Category.Class);

      if (player.learnableSkills.TryAdd(CustomSkill.ReligionProficiency, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.ReligionProficiency], player)))
        player.learnableSkills[CustomSkill.ReligionProficiency].LevelUp(player);
      player.learnableSkills[CustomSkill.ReligionProficiency].source.Add(Category.Class);

      return true;
    }
  }
}
