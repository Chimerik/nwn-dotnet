using Anvil.API;
using static NWN.Systems.SkillSystem;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      private void ApplyRockGnomePackage()
      {
        if (learnableSkills.TryAdd(CustomSkill.Gnome, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.Gnome], this)))
          learnableSkills[CustomSkill.Gnome].LevelUp(this);
        learnableSkills[CustomSkill.Gnome].source.Add(Category.Race);

        if (learnableSkills.TryAdd(CustomSkill.Prestidigitation, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.Prestidigitation], this)))
          learnableSkills[CustomSkill.Prestidigitation].LevelUp(this);
        learnableSkills[CustomSkill.Prestidigitation].source.Add(Category.Race);

        // TODO : Penser à doubler le bonus de maîtrise sur les jets d'Histoire
      }
    }
  }
}
