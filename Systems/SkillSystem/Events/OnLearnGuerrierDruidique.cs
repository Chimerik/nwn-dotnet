using Anvil.API;
using static NWN.Systems.PlayerSystem.Player;

namespace NWN.Systems
{
  public static partial class SkillSystem
  {
    public static bool OnLearnGuerrierDruidique(PlayerSystem.Player player, int customSkillId)
    {
      if (!player.oid.LoginCreature.KnowsFeat((Feat)customSkillId))
        player.oid.LoginCreature.AddFeat((Feat)customSkillId);

      if (!player.windows.TryGetValue("cantripSelection", out var spell2)) player.windows.Add("cantripSelection", new CantripSelectionWindow(player, ClassType.Druid, 2, featId:customSkillId));
      else ((CantripSelectionWindow)spell2).CreateWindow(ClassType.Druid, 2, featId: customSkillId);

      return true;
    }
  }
}
