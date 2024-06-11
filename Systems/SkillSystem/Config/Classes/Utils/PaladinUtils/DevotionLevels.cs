using Anvil.API;
using static NWN.Systems.PlayerSystem;
using static NWN.Systems.PlayerSystem.Player;
using static NWN.Systems.SkillSystem;

namespace NWN.Systems
{
  public static partial class Paladin
  {
    public static void HandleDevotionLevelUp(Player player, int level)
    {
      switch (level)
      {
        case 3: 
          
          new StrRef(12).SetPlayerOverride(player.oid, "Serment de Dévotion");
          player.oid.SetTextureOverride("paladin", "devotion");

          player.learnableSkills.TryAdd(CustomSkill.DevotionArmeSacree, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.DevotionArmeSacree], player));
          player.learnableSkills[CustomSkill.DevotionArmeSacree].LevelUp(player);
          player.learnableSkills[CustomSkill.DevotionArmeSacree].source.Add(Category.Class);

          player.learnableSkills.TryAdd(CustomSkill.DevotionSaintesRepresailles, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.DevotionSaintesRepresailles], player));
          player.learnableSkills[CustomSkill.DevotionSaintesRepresailles].LevelUp(player);
          player.learnableSkills[CustomSkill.DevotionSaintesRepresailles].source.Add(Category.Class);

          player.learnableSkills.TryAdd(CustomSkill.DevotionRenvoiDesImpies, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.DevotionRenvoiDesImpies], player));
          player.learnableSkills[CustomSkill.DevotionRenvoiDesImpies].LevelUp(player);
          player.learnableSkills[CustomSkill.DevotionRenvoiDesImpies].source.Add(Category.Class);

          break;

        case 5:

          

          break;

        case 7:

          if (!player.windows.TryGetValue("hunterTactiqueDefensiveSelection", out var def)) player.windows.Add("hunterTactiqueDefensiveSelection", new HunterTactiqueDefensiveSelectionWindow(player));
          else ((HunterTactiqueDefensiveSelectionWindow)def).CreateWindow();

          break;

        case 11:

          player.learnableSkills.TryAdd(CustomSkill.ChasseurVolee, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.ChasseurVolee], player));
          player.learnableSkills[CustomSkill.ChasseurVolee].LevelUp(player);
          player.learnableSkills[CustomSkill.ChasseurVolee].source.Add(Category.Class);

          break;

        case 15:

          if (!player.windows.TryGetValue("hunterDefenseSuperieureSelection", out var defsup)) player.windows.Add("hunterDefenseSuperieureSelection", new HunterDefenseSuperieureSelectionWindow(player));
          else ((HunterDefenseSuperieureSelectionWindow)defsup).CreateWindow();

          break;
      }
    }
  }
}
