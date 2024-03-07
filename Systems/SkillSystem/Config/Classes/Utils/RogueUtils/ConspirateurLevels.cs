using Anvil.API;
using static NWN.Systems.PlayerSystem;
using static NWN.Systems.SkillSystem;

namespace NWN.Systems
{
  public static partial class Rogue
  {
    public static void HandleConspirateurLevelUp(Player player, int level)
    {
      switch (level)
      {
        case 3: 
          
          new StrRef(16).SetPlayerOverride(player.oid, "Conspirateur");

          player.learnableSkills.TryAdd(CustomSkill.ConspirateurMaitriseTactique, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.ConspirateurMaitriseTactique], player));
          player.learnableSkills[CustomSkill.ConspirateurMaitriseTactique].LevelUp(player);
          player.learnableSkills[CustomSkill.ConspirateurMaitriseTactique].source.Add(Category.Class);

          break;

        case 13:

          player.learnableSkills.TryAdd(CustomSkill.ConspirateurRedirection, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.ConspirateurRedirection], player));
          player.learnableSkills[CustomSkill.ConspirateurRedirection].LevelUp(player);
          player.learnableSkills[CustomSkill.ConspirateurRedirection].source.Add(Category.Class);

          break;
      }
    }
  }
}
