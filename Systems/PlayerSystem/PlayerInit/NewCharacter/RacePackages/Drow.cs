using System.Diagnostics.Metrics;
using Anvil.API;
using static NWN.Systems.SkillSystem;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      private void ApplyDrowPackage()
      {
        if (learnableSkills.TryAdd(CustomSkill.Profond, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.Profond])))
          learnableSkills[CustomSkill.Profond].LevelUp(this);

        learnableSkills[CustomSkill.Profond].source.Add(Category.Race);

        if (learnableSkills.TryAdd(CustomSkill.RapierProficiency, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.RapierProficiency])))
          learnableSkills[CustomSkill.RapierProficiency].LevelUp(this);

        learnableSkills[CustomSkill.RapierProficiency].source.Add(Category.Race);

        if (learnableSkills.TryAdd(CustomSkill.ShortSwordProficiency, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.ShortSwordProficiency])))
          learnableSkills[CustomSkill.ShortSwordProficiency].LevelUp(this);

        learnableSkills[CustomSkill.ShortSwordProficiency].source.Add(Category.Race);

        // TODO : Magie Drow
      }
    }
  }
}
