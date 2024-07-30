using System.Security.Cryptography;
using Anvil.API;
using static NWN.Systems.PlayerSystem;
using static NWN.Systems.PlayerSystem.Player;
using static NWN.Systems.SkillSystem;

namespace NWN.Systems
{
  public static partial class Monk
  {
    public static void HandleElementsLevelUp(Player player, int level)
    {
      switch (level)
      {
        case 3: 
          
          new StrRef(10).SetPlayerOverride(player.oid, "Voie des Éléments");
          player.oid.SetTextureOverride("monk", "monk_elements");

          player.learnableSkills.TryAdd(CustomSkill.MonkHarmony, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.MonkHarmony], player));
          player.learnableSkills[CustomSkill.MonkHarmony].LevelUp(player);
          player.learnableSkills[CustomSkill.MonkHarmony].source.Add(Category.Class);

          if (!player.windows.TryGetValue("techniqueElementaireSelection", out var tech)) player.windows.Add("techniqueElementaireSelection", new TechniqueElementaireSelectionWindow(player, 3));
          else ((TechniqueElementaireSelectionWindow)tech).CreateWindow(3);

          break;

        case 6:

          if (!player.windows.TryGetValue("techniqueElementaireSelection", out var tech6)) player.windows.Add("techniqueElementaireSelection", new TechniqueElementaireSelectionWindow(player, 1));
          else ((TechniqueElementaireSelectionWindow)tech6).CreateWindow(1);

          break;

        case 9:

          player.learnableSkills.TryAdd(CustomSkill.MonkIncantationElementaire, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.MonkIncantationElementaire], player));
          player.learnableSkills[CustomSkill.MonkIncantationElementaire].LevelUp(player);
          player.learnableSkills[CustomSkill.MonkIncantationElementaire].source.Add(Category.Class);

          if (!player.windows.TryGetValue("techniqueElementaireSelection", out var tech9)) player.windows.Add("techniqueElementaireSelection", new TechniqueElementaireSelectionWindow(player, 1));
          else ((TechniqueElementaireSelectionWindow)tech9).CreateWindow(1);

          break;

        case 11:

          if (!player.windows.TryGetValue("techniqueElementaireSelection", out var tech11)) player.windows.Add("techniqueElementaireSelection", new TechniqueElementaireSelectionWindow(player, 1));
          else ((TechniqueElementaireSelectionWindow)tech11).CreateWindow(1);

          break;

        case 17:

          if (!player.windows.TryGetValue("techniqueElementaireSelection", out var tech17)) player.windows.Add("techniqueElementaireSelection", new TechniqueElementaireSelectionWindow(player, 1));
          else ((TechniqueElementaireSelectionWindow)tech17).CreateWindow(1);

          break;
      }
    }
  }
}
