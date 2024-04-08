using Anvil.API;
using static NWN.Systems.PlayerSystem;
using static NWN.Systems.SkillSystem;

namespace NWN.Systems
{
  public static partial class Wizard
  {
    public static void HandleDivinationLevelUp(Player player, int level)
    {
      switch (level)
      {
        case 2: 
          
          new StrRef(20).SetPlayerOverride(player.oid, "Devin");
          player.oid.SetTextureOverride("wizard", "divination");

          player.learnableSkills.TryAdd(CustomSkill.DivinationPresage, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.DivinationPresage], player));
          player.learnableSkills[CustomSkill.DivinationPresage].LevelUp(player);
          player.learnableSkills[CustomSkill.DivinationPresage].source.Add(Category.Class);

          break;

        case 6:

          player.learnableSkills.TryAdd(CustomSkill.DivinationExpert, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.DivinationExpert], player));
          player.learnableSkills[CustomSkill.DivinationExpert].LevelUp(player);
          player.learnableSkills[CustomSkill.DivinationExpert].source.Add(Category.Class);

          break;

        case 10:

          player.learnableSkills.TryAdd(CustomSkill.DivinationSeeInvisibility, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.DivinationSeeInvisibility], player));
          player.learnableSkills[CustomSkill.DivinationSeeInvisibility].LevelUp(player);
          player.learnableSkills[CustomSkill.DivinationSeeInvisibility].source.Add(Category.Class);

          player.learnableSkills.TryAdd(CustomSkill.DivinationDarkVision, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.DivinationDarkVision], player));
          player.learnableSkills[CustomSkill.DivinationDarkVision].LevelUp(player);
          player.learnableSkills[CustomSkill.DivinationDarkVision].source.Add(Category.Class);

          player.learnableSkills.TryAdd(CustomSkill.DivinationSeeEthereal, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.DivinationSeeEthereal], player));
          player.learnableSkills[CustomSkill.DivinationSeeEthereal].LevelUp(player);
          player.learnableSkills[CustomSkill.DivinationSeeEthereal].source.Add(Category.Class);

          break;

        case 14:

          player.learnableSkills.TryAdd(CustomSkill.DivinationPresageSuperieur, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.DivinationPresageSuperieur], player));
          player.learnableSkills[CustomSkill.DivinationPresageSuperieur].LevelUp(player);
          player.learnableSkills[CustomSkill.DivinationPresageSuperieur].source.Add(Category.Class);

          break;
      }
    }
  }
}
