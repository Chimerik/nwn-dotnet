using System.Diagnostics.Metrics;
using Anvil.API;
using static NWN.Systems.PlayerSystem;
using static NWN.Systems.SkillSystem;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      private void ApplyWoodHalfElfPackage()
      {
        if (learnableSkills.TryAdd(CustomSkill.Elfique, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.Elfique], this)))
          learnableSkills[CustomSkill.Elfique].LevelUp(this);

        learnableSkills[CustomSkill.Elfique].source.Add(Category.Race);

        if (learnableSkills.TryAdd(CustomSkill.HighElfLanguage, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.HighElfLanguage], this)))
          learnableSkills[CustomSkill.HighElfLanguage].LevelUp(this);

        learnableSkills[CustomSkill.HighElfLanguage].source.Add(Category.Race);

        if (learnableSkills.TryAdd(CustomSkill.StealthProficiency, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.StealthProficiency], this)))
          learnableSkills[CustomSkill.StealthProficiency].LevelUp(this);

        learnableSkills[CustomSkill.StealthProficiency].source.Add(Category.Race);

        this.oid.LoginCreature.AddFeat(Feat.HardinessVersusEnchantments);
        ApplyWoodElfSpeed();
      }
    }
  }
}
