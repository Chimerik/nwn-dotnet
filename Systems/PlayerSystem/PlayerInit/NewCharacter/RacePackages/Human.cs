using static NWN.Systems.SkillSystem;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      private void ApplyHumanPackage(int bonusSelection)
      {
        if (learnableSkills.TryAdd(CustomSkill.HumanVersatility, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.HumanVersatility], this)))
          learnableSkills[CustomSkill.HumanVersatility].LevelUp(this);

        learnableSkills[CustomSkill.HumanVersatility].source.Add(Category.Race);

        if (bonusSelection > -1)
        {
          if (learnableSkills.TryAdd(bonusSelection, new LearnableSkill((LearnableSkill)learnableDictionary[bonusSelection], this)))
            learnableSkills[bonusSelection].LevelUp(this);

          learnableSkills[bonusSelection].source.Add(Category.Race);
        }
      }
    }
  }
}
