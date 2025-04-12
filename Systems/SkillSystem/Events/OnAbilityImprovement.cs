using static NWN.Systems.PlayerSystem.Player;

namespace NWN.Systems
{
  public static partial class SkillSystem
  {
    public static bool OnAbilityImprovement(PlayerSystem.Player player, int customSkillId)
    {
      if (!player.windows.TryGetValue("abilityImprovement", out var value)) player.windows.Add("abilityImprovement", new AbilityImprovementWindow(player));
      else ((AbilityImprovementWindow)value).CreateWindow();

      return true;
    }
  }
}
