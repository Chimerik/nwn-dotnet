using Anvil.API;
using static NWN.Systems.PlayerSystem;
using static NWN.Systems.SkillSystem;

namespace NWN.Systems
{
  public static partial class Rogue
  {
    public static void HandleAssassinLevelUp(Player player, int level)
    {
      switch (level)
      {
        case 3: 
          
          new StrRef(16).SetPlayerOverride(player.oid, "Assassin");
          player.oid.SetTextureOverride("rogue", "assassin");

          player.learnableSkills.TryAdd(CustomSkill.AssassinAssassinate, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.AssassinAssassinate], player));
          player.learnableSkills[CustomSkill.AssassinAssassinate].LevelUp(player);
          player.learnableSkills[CustomSkill.AssassinAssassinate].source.Add(Category.Class);

          break;
      }
    }
  }
}
