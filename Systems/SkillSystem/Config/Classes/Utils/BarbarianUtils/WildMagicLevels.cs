using Anvil.API;
using static NWN.Systems.PlayerSystem;
using static NWN.Systems.PlayerSystem.Player;
using static NWN.Systems.SkillSystem;

namespace NWN.Systems
{
  public static partial class Barbarian
  {
    public static void HandleWildMagicLevelUp(Player player, int level)
    {
      switch (level)
      {
        case 3:

          new StrRef(5213).SetPlayerOverride(player.oid, "Magie Sauvage");

          player.learnableSkills.TryAdd(CustomSkill.WildMagicSense, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.WildMagicSense], player));
          player.learnableSkills[CustomSkill.WildMagicSense].LevelUp(player);
          player.learnableSkills[CustomSkill.WildMagicSense].source.Add(Category.Class);

          player.learnableSkills.TryAdd(CustomSkill.WildMagicTeleportation, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.WildMagicTeleportation], player));
          player.learnableSkills[CustomSkill.WildMagicTeleportation].LevelUp(player);
          player.learnableSkills[CustomSkill.WildMagicTeleportation].source.Add(Category.Class);

          break;

        case 6:

          player.learnableSkills.TryAdd(CustomSkill.WildMagicMagieGalvanisanteBienfait, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.WildMagicMagieGalvanisanteBienfait], player));
          player.learnableSkills[CustomSkill.WildMagicMagieGalvanisanteBienfait].LevelUp(player);
          player.learnableSkills[CustomSkill.WildMagicMagieGalvanisanteBienfait].source.Add(Category.Class);

          player.learnableSkills.TryAdd(CustomSkill.WildMagicMagieGalvanisanteRecuperation, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.WildMagicMagieGalvanisanteRecuperation], player));
          player.learnableSkills[CustomSkill.WildMagicMagieGalvanisanteRecuperation].LevelUp(player);
          player.learnableSkills[CustomSkill.WildMagicMagieGalvanisanteRecuperation].source.Add(Category.Class);

          break;

        case 14:

          // TODO : table de priorité des effets de magie sauvage

          break;
      }
    }
  }
}
