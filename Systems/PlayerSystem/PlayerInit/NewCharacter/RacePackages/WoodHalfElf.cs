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
        if (learnableSkills.TryAdd(CustomSkill.LightArmorProficiency, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.LightArmorProficiency], this)))
          learnableSkills[CustomSkill.LightArmorProficiency].LevelUp(this);

        learnableSkills[CustomSkill.LightArmorProficiency].source.Add(Category.Race);

        if (learnableSkills.TryAdd(CustomSkill.ShieldProficiency, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.ShieldProficiency], this)))
          learnableSkills[CustomSkill.ShieldProficiency].LevelUp(this);

        learnableSkills[CustomSkill.ShieldProficiency].source.Add(Category.Race);

        if (learnableSkills.TryAdd(CustomSkill.SpearProficiency, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.SpearProficiency], this)))
          learnableSkills[CustomSkill.SpearProficiency].LevelUp(this);

        learnableSkills[CustomSkill.SpearProficiency].source.Add(Category.Race);

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

        // TODO : Ascendance féerique : Avantage sur les jets de sauvegarde contre les effets de charme - Penser à ajouter l'avantage au moment de refaire les sorts avec Charme
      }
    }
  }
}
