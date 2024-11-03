using System.Diagnostics.Metrics;
using Anvil.API;
using static NWN.Systems.SkillSystem;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      private void ApplyHighHalfElfPackage(int bonusSelection)
      {
        if (learnableSkills.TryAdd(CustomSkill.Elfique, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.Elfique], this)))
          learnableSkills[CustomSkill.Elfique].LevelUp(this);

        learnableSkills[CustomSkill.Elfique].source.Add(Category.Race);

        if (learnableSkills.TryAdd(CustomSkill.HighElfLanguage, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.HighElfLanguage], this)))
          learnableSkills[CustomSkill.HighElfLanguage].LevelUp(this);

        learnableSkills[CustomSkill.HighElfLanguage].source.Add(Category.Race);

        if (bonusSelection > -1)
        {
          NwFeat feat = NwFeat.FromFeatId(bonusSelection);
          if (learnableSkills.TryAdd(feat.Id, new LearnableSkill((LearnableSkill)learnableDictionary[feat.Id], this)))
            learnableSkills[feat.Id].LevelUp(this);

          learnableSkills[feat.Id].source.Add(Category.Race);
        }

        this.oid.LoginCreature.AddFeat(Feat.HardinessVersusEnchantments);
        ApplyElvenSleepImmunity();
      }
    }
  }
}
