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
          
          new StrRef(16).SetPlayerOverride(player.oid, "Voie de l'Ombre");

          player.learnableSkills.TryAdd(CustomSkill.MonkTenebres, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.MonkTenebres], player));
          player.learnableSkills[CustomSkill.MonkTenebres].LevelUp(player);
          player.learnableSkills[CustomSkill.MonkTenebres].source.Add(Category.Class);

          player.learnableSkills.TryAdd(CustomSkill.MonkDarkVision, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.MonkDarkVision], player));
          player.learnableSkills[CustomSkill.MonkDarkVision].LevelUp(player);
          player.learnableSkills[CustomSkill.MonkDarkVision].source.Add(Category.Class);

          player.learnableSkills.TryAdd(CustomSkill.MonkPassageSansTrace, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.MonkPassageSansTrace], player));
          player.learnableSkills[CustomSkill.MonkPassageSansTrace].LevelUp(player);
          player.learnableSkills[CustomSkill.MonkPassageSansTrace].source.Add(Category.Class);

          player.learnableSkills.TryAdd(CustomSkill.MonkSilence, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.MonkSilence], player));
          player.learnableSkills[CustomSkill.MonkSilence].LevelUp(player);
          player.learnableSkills[CustomSkill.MonkSilence].source.Add(Category.Class);

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
      }
    }
  }
}
