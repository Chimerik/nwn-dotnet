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

          break;

        case 6:

          if (!player.windows.TryGetValue("aspectTotemSelection", out var aspect)) player.windows.Add("aspectTotemSelection", new AspectTotemSelectionWindow(player));
          else ((AspectTotemSelectionWindow)aspect).CreateWindow();

          break;

        case 10:

          player.learnableSkills.TryAdd(CustomSkill.TotemCommunionAvecLaNature, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.TotemCommunionAvecLaNature], player));
          player.learnableSkills[CustomSkill.TotemCommunionAvecLaNature].LevelUp(player);
          player.learnableSkills[CustomSkill.TotemCommunionAvecLaNature].source.Add(Category.Class);

          break;
      }
    }
  }
}
