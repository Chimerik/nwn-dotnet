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
        if (learnableSkills.TryAdd(CustomSkill.Elfique, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.Elfique], this)))
          learnableSkills[CustomSkill.Elfique].LevelUp(this);

        learnableSkills[CustomSkill.Elfique].source.Add(Category.Race);

        if (learnableSkills.TryAdd(CustomSkill.PerceptionProficiency, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.PerceptionProficiency], this)))
          learnableSkills[CustomSkill.PerceptionProficiency].LevelUp(this);

        learnableSkills[CustomSkill.PerceptionProficiency].source.Add(Category.Race);

        if (learnableSkills.TryAdd(CustomSkill.StealthProficiency, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.StealthProficiency], this)))
          learnableSkills[CustomSkill.StealthProficiency].LevelUp(this);

        learnableSkills[CustomSkill.StealthProficiency].source.Add(Category.Race);

        if (learnableSkills.TryAdd(CustomSkill.LongSwordProficiency, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.LongSwordProficiency], this)))
          learnableSkills[CustomSkill.LongSwordProficiency].LevelUp(this);

        learnableSkills[CustomSkill.LongSwordProficiency].source.Add(Category.Race);

        if (learnableSkills.TryAdd(CustomSkill.ShortSwordProficiency, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.ShortSwordProficiency], this)))
          learnableSkills[CustomSkill.ShortSwordProficiency].LevelUp(this);

        learnableSkills[CustomSkill.ShortSwordProficiency].source.Add(Category.Race);

        if (learnableSkills.TryAdd(CustomSkill.LongBowProficiency, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.LongBowProficiency], this)))
          learnableSkills[CustomSkill.LongBowProficiency].LevelUp(this);

        learnableSkills[CustomSkill.LongBowProficiency].source.Add(Category.Race);

        if (learnableSkills.TryAdd(CustomSkill.ShortBowProficiency, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.ShortBowProficiency], this)))
          learnableSkills[CustomSkill.ShortBowProficiency].LevelUp(this);

        learnableSkills[CustomSkill.ShortBowProficiency].source.Add(Category.Race);

        this.oid.LoginCreature.AddFeat(Feat.HardinessVersusEnchantments);
        ApplyElvenSleepImmunity();
        ApplyWoodElfSpeed();
      }
    }
  }
}
