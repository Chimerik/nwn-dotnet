using Anvil.API;
using static NWN.Systems.PlayerSystem;
using static NWN.Systems.SkillSystem;

namespace NWN.Systems
{
  public static partial class Wizard
  {
    public static void HandleEnchantementLevelUp(Player player, int level)
    {
      switch (level)
      {
        case 2: 
          
          new StrRef(20).SetPlayerOverride(player.oid, "Enchanteur");
          player.oid.SetTextureOverride("wizard", "enchantement");

          player.learnableSkills.TryAdd(CustomSkill.EnchantementRegardHypnotique, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.EnchantementRegardHypnotique], player));
          player.learnableSkills[CustomSkill.EnchantementRegardHypnotique].LevelUp(player);
          player.learnableSkills[CustomSkill.EnchantementRegardHypnotique].source.Add(Category.Class);

          break;

        case 6:

          player.learnableSkills.TryAdd(CustomSkill.EnchantementCharmeInstinctif, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.EnchantementCharmeInstinctif], player));
          player.learnableSkills[CustomSkill.EnchantementCharmeInstinctif].LevelUp(player);
          player.learnableSkills[CustomSkill.EnchantementCharmeInstinctif].source.Add(Category.Class);
          
          break;

        case 10:

          player.learnableSkills.TryAdd(CustomSkill.EnchantementPartage, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.EnchantementPartage], player));
          player.learnableSkills[CustomSkill.EnchantementPartage].LevelUp(player);
          player.learnableSkills[CustomSkill.EnchantementPartage].source.Add(Category.Class);

          break;

        case 14:

          player.learnableSkills.TryAdd(CustomSkill.EnchantementAlterationMemorielle, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.EnchantementAlterationMemorielle], player));
          player.learnableSkills[CustomSkill.EnchantementAlterationMemorielle].LevelUp(player);
          player.learnableSkills[CustomSkill.EnchantementAlterationMemorielle].source.Add(Category.Class);

          break;
      }
    }
  }
}
