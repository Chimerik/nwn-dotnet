using System.Diagnostics.Metrics;
using Anvil.API;
using static NWN.Systems.SkillSystem;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      private void ApplyWoodElfPackage()
      {
        if (learnableSkills.TryAdd(CustomSkill.PerceptionProficiency, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.PerceptionProficiency])))
          learnableSkills[CustomSkill.PerceptionProficiency].LevelUp(this);

        learnableSkills[CustomSkill.PerceptionProficiency].source.Add(Category.Race);

        if (learnableSkills.TryAdd(CustomSkill.StealthProficiency, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.StealthProficiency])))
          learnableSkills[CustomSkill.StealthProficiency].LevelUp(this);

        learnableSkills[CustomSkill.StealthProficiency].source.Add(Category.Race);

        if (learnableSkills.TryAdd(CustomSkill.LongSwordProficiency, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.LongSwordProficiency])))
          learnableSkills[CustomSkill.LongSwordProficiency].LevelUp(this);

        learnableSkills[CustomSkill.LongSwordProficiency].source.Add(Category.Race);

        if (learnableSkills.TryAdd(CustomSkill.ShortSwordProficiency, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.ShortSwordProficiency])))
          learnableSkills[CustomSkill.ShortSwordProficiency].LevelUp(this);

        learnableSkills[CustomSkill.ShortSwordProficiency].source.Add(Category.Race);

        if (learnableSkills.TryAdd(CustomSkill.LongBowProficiency, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.LongBowProficiency])))
          learnableSkills[CustomSkill.LongBowProficiency].LevelUp(this);

        learnableSkills[CustomSkill.LongBowProficiency].source.Add(Category.Race);

        if (learnableSkills.TryAdd(CustomSkill.ShortBowProficiency, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.ShortBowProficiency])))
          learnableSkills[CustomSkill.ShortBowProficiency].LevelUp(this);

        learnableSkills[CustomSkill.ShortBowProficiency].source.Add(Category.Race);

        ApplyElvenSleepImmunity();
        ApplyWoodElfSpeed();
      }
    }
  }
}
