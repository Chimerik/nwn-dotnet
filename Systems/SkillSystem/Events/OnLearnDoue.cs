using System.Linq;

namespace NWN.Systems
{
  public static partial class SkillSystem
  {
    public static bool OnLearnDoue(PlayerSystem.Player player, int customSkillId)
    {
      foreach (var learnable in player.learnableSkills.Values.Where(s => s.category == Category.Skill && s.currentLevel < 1))
        learnable.acquiredPoints += learnable.pointsToNextLevel / 2;

      return true;
    }
  }
}
