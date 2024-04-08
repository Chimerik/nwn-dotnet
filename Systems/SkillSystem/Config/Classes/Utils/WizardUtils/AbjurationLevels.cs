using Anvil.API;
using static NWN.Systems.PlayerSystem;
using static NWN.Systems.SkillSystem;

namespace NWN.Systems
{
  public static partial class Wizard
  {
    public static void HandleAbjurationLevelUp(Player player, int level)
    {
      switch (level)
      {
        case 2: 
          
          new StrRef(20).SetPlayerOverride(player.oid, "Abjurateur");
          player.oid.SetTextureOverride("wizard", "abjuration");

          player.learnableSkills.TryAdd(CustomSkill.AbjurationWard, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.AbjurationWard], player));
          player.learnableSkills[CustomSkill.AbjurationWard].LevelUp(player);
          player.learnableSkills[CustomSkill.AbjurationWard].source.Add(Category.Class);

          break;

        case 6:

          player.learnableSkills.TryAdd(CustomSkill.AbjurationWardProjetee, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.AbjurationWardProjetee], player));
          player.learnableSkills[CustomSkill.AbjurationWardProjetee].LevelUp(player);
          player.learnableSkills[CustomSkill.AbjurationWardProjetee].source.Add(Category.Class);

          break;

        case 10:

          player.learnableSkills.TryAdd(CustomSkill.AbjurationImproved, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.AbjurationImproved], player));
          player.learnableSkills[CustomSkill.AbjurationImproved].LevelUp(player);
          player.learnableSkills[CustomSkill.AbjurationImproved].source.Add(Category.Class);

          break;

        case 14:

          player.learnableSkills.TryAdd(CustomSkill.AbjurationSpellResistance, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.AbjurationSpellResistance], player));
          player.learnableSkills[CustomSkill.AbjurationSpellResistance].LevelUp(player);
          player.learnableSkills[CustomSkill.AbjurationSpellResistance].source.Add(Category.Class);

          break;
      }
    }
  }
}
