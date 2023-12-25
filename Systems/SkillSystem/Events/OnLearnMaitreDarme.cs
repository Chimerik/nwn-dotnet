using System.Collections.Generic;
using System.Linq;
using Anvil.API;
using static NWN.Systems.PlayerSystem.Player;

namespace NWN.Systems
{
  public static partial class SkillSystem
  {
    public static bool OnLearnMaitreDarme(PlayerSystem.Player player, int customSkillId)
    {
      foreach (var learnable in player.learnableSkills.Values.Where(s => s.category == Category.WeaponProficiency && s.currentLevel < 1))
        learnable.acquiredPoints += learnable.pointsToNextLevel / 4;

      if (!player.windows.TryGetValue("weaponBonusChoice", out var value)) player.windows.Add("weaponBonusChoice", new WeaponBonusChoiceWindow(player));
      else ((WeaponBonusChoiceWindow)value).CreateWindow();

      return true;
    }
  }
}
