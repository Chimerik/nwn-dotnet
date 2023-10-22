using static NWN.Systems.SkillSystem;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      private void ApplyDrowHalfElfPackage()
      {
        if (learnableSkills.TryAdd(CustomSkill.LightArmorProficiency, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.LightArmorProficiency])))
          learnableSkills[CustomSkill.LightArmorProficiency].LevelUp(this);

        learnableSkills[CustomSkill.LightArmorProficiency].source.Add(Category.Race);

        if (learnableSkills.TryAdd(CustomSkill.ShieldProficiency, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.ShieldProficiency])))
          learnableSkills[CustomSkill.ShieldProficiency].LevelUp(this);

        learnableSkills[CustomSkill.ShieldProficiency].source.Add(Category.Race);

        if (learnableSkills.TryAdd(CustomSkill.SpearProficiency, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.SpearProficiency])))
          learnableSkills[CustomSkill.SpearProficiency].LevelUp(this);

        learnableSkills[CustomSkill.SpearProficiency].source.Add(Category.Race);

        if (learnableSkills.TryAdd(CustomSkill.Elfique, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.Elfique])))
          learnableSkills[CustomSkill.Elfique].LevelUp(this);

        learnableSkills[CustomSkill.Elfique].source.Add(Category.Race);

        if (learnableSkills.TryAdd(CustomSkill.HighElfLanguage, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.HighElfLanguage])))
          learnableSkills[CustomSkill.HighElfLanguage].LevelUp(this);

        learnableSkills[CustomSkill.HighElfLanguage].source.Add(Category.Race);

        // TODO : implémenter magie drow
      }
    }
  }
}
