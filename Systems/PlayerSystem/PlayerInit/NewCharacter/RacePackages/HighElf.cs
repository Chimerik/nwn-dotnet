using System.Collections.Generic;
using System.Diagnostics.Metrics;
using Anvil.API;
using static NWN.Systems.SkillSystem;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      private void ApplyHighElfPackage(int bonusSelection)
      {
        if (learnableSkills.TryAdd(CustomSkill.Elfique, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.Elfique])))
          learnableSkills[CustomSkill.Elfique].LevelUp(this);

        learnableSkills[CustomSkill.Elfique].source.Add(Category.Race);

        if (learnableSkills.TryAdd(CustomSkill.HighElfLanguage, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.HighElfLanguage])))
          learnableSkills[CustomSkill.HighElfLanguage].LevelUp(this);

        learnableSkills[CustomSkill.HighElfLanguage].source.Add(Category.Race);

        if (learnableSkills.TryAdd(CustomSkill.PerceptionProficiency, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.PerceptionProficiency])))
          learnableSkills[CustomSkill.PerceptionProficiency].LevelUp(this);

        learnableSkills[CustomSkill.PerceptionProficiency].source.Add(Category.Race);

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

        if (bonusSelection > -1)
        {        
          NwFeat feat = NwSpell.FromSpellId(bonusSelection).FeatReference;
          if (learnableSkills.TryAdd(feat.Id, new LearnableSkill((LearnableSkill)learnableDictionary[feat.Id])))
            learnableSkills[feat.Id].LevelUp(this);

          learnableSkills[feat.Id].source.Add(Category.Race);
        }

        ApplyElvenSleepImmunity();

        // TODO : Ascendance féerique : Avantage sur les jets de sauvegarde contre les effets de charme - Penser à ajouter l'avantage au moment de refaire les sorts avec Charme
        // TODO : Choisit un tour de magie supplémentaire (int)
          // Transformer les cantrips en feat
          // Afficher la liste des cantrips et permettre d'en sélectionner un
      }
    }
  }
}
