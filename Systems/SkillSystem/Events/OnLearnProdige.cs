using System.Linq;
using static NWN.Systems.PlayerSystem.Player;

namespace NWN.Systems
{
  public static partial class SkillSystem
  {
    public static bool OnLearnProdige(PlayerSystem.Player player, int customSkillId)
    {
      foreach (var learnable in player.learnableSkills.Values.Where(s => (s.category == Category.Language 
      || s.category == Category.Skill || s.category == Category.Expertise) && s.currentLevel < 1))
        learnable.acquiredPoints += learnable.pointsToNextLevel / 4;

      return true;
    }
  }
}
