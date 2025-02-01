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
        learnable.acquiredPoints += (learnable.pointsToNextLevel - learnable.acquiredPoints) / 4;

      player.oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_IN_WEAPON_MASTER_CHOICE_FEAT").Value = 1;

      if (!player.windows.TryGetValue("maitreDarmesSelection", out var value)) player.windows.Add("maitreDarmesSelection", new MaitreDarmesSelectionWindow(player));
      else ((MaitreDarmesSelectionWindow)value).CreateWindow();

      return true;
    }
  }
}
