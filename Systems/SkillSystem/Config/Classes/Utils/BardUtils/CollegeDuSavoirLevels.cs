using System.Collections.Generic;
using System.Linq;
using Anvil.API;
using static NWN.Systems.PlayerSystem;
using static NWN.Systems.PlayerSystem.Player;
using static NWN.Systems.SkillSystem;

namespace NWN.Systems
{
  public static partial class Bard
  {
    public static void HandleCollegeDuSavoirLevelUp(Player player, int level)
    {
      switch (level)
      {
        case 3: 
          
          new StrRef(2).SetPlayerOverride(player.oid, "Collège du Savoir");
          player.oid.SetTextureOverride("bard", "college_lore");

          List<int> skillList = new();

          foreach(var skill in learnableDictionary.Where(l => l.Value is LearnableSkill learnable && learnable.category == Category.Skill))
            skillList.Add(skill.Key);

          if (!player.windows.TryGetValue("skillProficiencySelection", out var skill3)) player.windows.Add("skillProficiencySelection", new SkillProficiencySelectionWindow(player, skillList, 3));
          else ((SkillProficiencySelectionWindow)skill3).CreateWindow(skillList, 3);

          player.learnableSkills.TryAdd(CustomSkill.MotsCinglants, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.MotsCinglants], player));
          player.learnableSkills[CustomSkill.MotsCinglants].LevelUp(player);
          player.learnableSkills[CustomSkill.MotsCinglants].source.Add(Category.Class);

          break;

        case 6:

          if (!player.windows.TryGetValue("bardMagicalSecretSelection", out var secret10)) player.windows.Add("bardMagicalSecretSelection", new BardMagicalSecretSelectionWindow(player, 2));
          else ((BardMagicalSecretSelectionWindow)secret10).CreateWindow(2);

          break;
      }
    }
  }
}
