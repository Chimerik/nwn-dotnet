using Anvil.API;
using static NWN.Systems.PlayerSystem;
using static NWN.Systems.SkillSystem;

namespace NWN.Systems
{
  public static partial class Monk
  {
    public static void HandleOmbreLevelUp(Player player, int level)
    {
      switch (level)
      {
        case 3: 
          
          new StrRef(10).SetPlayerOverride(player.oid, "Voie de l'Ombre");
          player.oid.SetTextureOverride("monk", "monk_shadow");

          player.LearnClassSkill(CustomSkill.MonkTenebres);
          player.LearnClassSkill(CustomSkill.MonkPassageSansTrace);
          player.LearnClassSkill(CustomSkill.MonkSilence);
          player.LearnClassSkill(CustomSkill.IllusionMineure);

          break;

        case 5:

          player.learnableSkills.TryAdd(CustomSkill.MonkLinceulDombre, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.MonkLinceulDombre], player));
          player.learnableSkills[CustomSkill.MonkLinceulDombre].LevelUp(player);
          player.learnableSkills[CustomSkill.MonkLinceulDombre].source.Add(Category.Class);

          break;

        case 6:

          player.learnableSkills.TryAdd(CustomSkill.MonkFouleeDombre, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.MonkFouleeDombre], player));
          player.learnableSkills[CustomSkill.MonkFouleeDombre].LevelUp(player);
          player.learnableSkills[CustomSkill.MonkFouleeDombre].source.Add(Category.Class);

          break;

        case 11:

          player.learnableSkills.TryAdd(CustomSkill.MonkFrappeDombre, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.MonkFrappeDombre], player));
          player.learnableSkills[CustomSkill.MonkFrappeDombre].LevelUp(player);
          player.learnableSkills[CustomSkill.MonkFrappeDombre].source.Add(Category.Class);

          break;

        case 17:

          player.learnableSkills.TryAdd(CustomSkill.MonkOpportuniste, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.MonkOpportuniste], player));
          player.learnableSkills[CustomSkill.MonkOpportuniste].LevelUp(player);
          player.learnableSkills[CustomSkill.MonkOpportuniste].source.Add(Category.Class);

          break;
      }
    }
  }
}
