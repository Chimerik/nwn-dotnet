using Anvil.API;
using static NWN.Systems.PlayerSystem.Player;

namespace NWN.Systems
{
  public static partial class SkillSystem
  {
    public static bool OnLearnDruideSage(PlayerSystem.Player player, int customSkillId)
    {
      if (!player.oid.LoginCreature.KnowsFeat((Feat)customSkillId))
        player.oid.LoginCreature.AddFeat((Feat)customSkillId);

      if (!player.windows.TryGetValue("cantripSelection", out var spell1)) player.windows.Add("cantripSelection", new CantripSelectionWindow(player, ClassType.Druid, 3));
      else ((CantripSelectionWindow)spell1).CreateWindow(ClassType.Druid, 3);

      return true;
    }
  }
}
