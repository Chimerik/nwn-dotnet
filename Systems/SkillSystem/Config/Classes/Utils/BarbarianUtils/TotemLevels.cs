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

          player.learnableSkills.TryAdd(CustomSkill.TotemSpeakAnimal, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.TotemSpeakAnimal], player));
          player.learnableSkills[CustomSkill.TotemSpeakAnimal].LevelUp(player);
          player.learnableSkills[CustomSkill.TotemSpeakAnimal].source.Add(Category.Class);

          player.learnableSkills.TryAdd(CustomSkill.TotemSensAnimal, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.TotemSensAnimal], player));
          player.learnableSkills[CustomSkill.TotemSensAnimal].LevelUp(player);
          player.learnableSkills[CustomSkill.TotemSensAnimal].source.Add(Category.Class);

          if (!player.windows.TryGetValue("espritTotemSelection", out var value)) player.windows.Add("espritTotemSelection", new EspritTotemSelectionWindow(player));
          else ((EspritTotemSelectionWindow)value).CreateWindow();

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

        case 14:

          if (!player.windows.TryGetValue("lienTotemSelection", out var lien)) player.windows.Add("lienTotemSelection", new LienTotemSelectionWindow(player));
          else ((LienTotemSelectionWindow)lien).CreateWindow();

          break;
      }
    }
  }
}
