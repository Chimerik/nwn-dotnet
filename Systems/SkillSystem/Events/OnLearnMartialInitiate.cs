using static NWN.Systems.PlayerSystem.Player;

namespace NWN.Systems
{
  public static partial class SkillSystem
  {
    public static bool OnLearnMartialInitiate(PlayerSystem.Player player, int customSkillId)
    {
      if (!player.windows.TryGetValue("fightingStyleSelection", out var style)) player.windows.Add("fightingStyleSelection", new FightingStyleSelectionWindow(player, CustomSkill.MartialInitiate));
      else ((FightingStyleSelectionWindow)style).CreateWindow(CustomSkill.MartialInitiate);

      return true;
    }
  }
}
