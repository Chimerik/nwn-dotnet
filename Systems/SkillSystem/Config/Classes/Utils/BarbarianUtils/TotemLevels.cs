using Anvil.API;
using static NWN.Systems.PlayerSystem;
using static NWN.Systems.PlayerSystem.Player;
using static NWN.Systems.SkillSystem;

namespace NWN.Systems
{
  public static partial class Barbarian
  {
    public static void HandleTotemLevelUp(Player player, int level)
    {
      switch (level)
      {
        case 3:

          new StrRef(5213).SetPlayerOverride(player.oid, "Totem");
          player.oid.SetTextureOverride("barbarian", "totem");

          player.learnableSkills.TryAdd(CustomSkill.TotemSpeakAnimal, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.TotemSpeakAnimal], player));
          player.learnableSkills[CustomSkill.TotemSpeakAnimal].LevelUp(player);
          player.learnableSkills[CustomSkill.TotemSpeakAnimal].source.Add(Category.Class);

          player.learnableSkills.TryAdd(CustomSkill.TotemSensAnimal, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.TotemSensAnimal], player));
          player.learnableSkills[CustomSkill.TotemSensAnimal].LevelUp(player);
          player.learnableSkills[CustomSkill.TotemSensAnimal].source.Add(Category.Class);

          player.learnableSkills.TryAdd(CustomSkill.TotemRage, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.TotemRage], player));
          player.learnableSkills[CustomSkill.TotemRage].LevelUp(player);
          player.learnableSkills[CustomSkill.TotemRage].source.Add(Category.Class);

          break;

        case 6:

          player.learnableSkills.TryAdd(CustomSkill.TotemAspectSauvage, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.TotemAspectSauvage], player));
          player.learnableSkills[CustomSkill.TotemAspectSauvage].LevelUp(player);
          player.learnableSkills[CustomSkill.TotemAspectSauvage].source.Add(Category.Class);

          break;

        case 10:

          player.learnableSkills.TryAdd(CustomSkill.TotemCommunionAvecLaNature, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.TotemCommunionAvecLaNature], player));
          player.learnableSkills[CustomSkill.TotemCommunionAvecLaNature].LevelUp(player);
          player.learnableSkills[CustomSkill.TotemCommunionAvecLaNature].source.Add(Category.Class);

          break;

        case 14:

          player.learnableSkills.TryAdd(CustomSkill.TotemPuissanceSauvage, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.TotemPuissanceSauvage], player));
          player.learnableSkills[CustomSkill.TotemPuissanceSauvage].LevelUp(player);
          player.learnableSkills[CustomSkill.TotemPuissanceSauvage].source.Add(Category.Class);

          break;
      }
    }
  }
}
