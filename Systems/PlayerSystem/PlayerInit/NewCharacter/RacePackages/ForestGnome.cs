using Anvil.API;
using static NWN.Systems.SkillSystem;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      private void ApplyForestGnomePackage()
      {
        if (learnableSkills.TryAdd(CustomSkill.Gnome, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.Gnome], this)))
          learnableSkills[CustomSkill.Gnome].LevelUp(this);
        learnableSkills[CustomSkill.Gnome].source.Add(Category.Race);

        if (learnableSkills.TryAdd(CustomSkill.IllusionMineure, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.IllusionMineure], this)))
          learnableSkills[CustomSkill.IllusionMineure].LevelUp(this);
        learnableSkills[CustomSkill.IllusionMineure].source.Add(Category.Race);
      }
    }
  }
}
