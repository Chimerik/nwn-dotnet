using Anvil.API;
using static NWN.Systems.PlayerSystem.Player;

namespace NWN.Systems
{
  public static partial class SkillSystem
  {
    public static bool OnLearnClercThaumaturge(PlayerSystem.Player player, int customSkillId)
    {
      if (!player.oid.LoginCreature.KnowsFeat((Feat)CustomSkill.ClercThaumaturge))
        player.oid.LoginCreature.AddFeat((Feat)CustomSkill.ClercThaumaturge);

      if (!player.windows.TryGetValue("cantripSelection", out var spell1)) player.windows.Add("cantripSelection", new CantripSelectionWindow(player, ClassType.Cleric, 4));
      else ((CantripSelectionWindow)spell1).CreateWindow(ClassType.Cleric, 4);

      return true;
    }
  }
}
