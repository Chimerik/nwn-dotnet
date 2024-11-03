using static NWN.Systems.SkillSystem;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      private void ApplyDrowHalfElfPackage()
      { 
        if (learnableSkills.TryAdd(CustomSkill.Elfique, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.Elfique], this)))
          learnableSkills[CustomSkill.Elfique].LevelUp(this);

        learnableSkills[CustomSkill.Elfique].source.Add(Category.Race);

        if (learnableSkills.TryAdd(CustomSkill.HighElfLanguage, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.HighElfLanguage], this)))
          learnableSkills[CustomSkill.HighElfLanguage].LevelUp(this);

        learnableSkills[CustomSkill.HighElfLanguage].source.Add(Category.Race);

        if (learnableSkills.TryAdd(CustomSkill.LightDrow, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.LightDrow], this)))
          learnableSkills[CustomSkill.LightDrow].LevelUp(this);

        learnableSkills[CustomSkill.LightDrow].source.Add(Category.Race);
      }
    }
  }
}
