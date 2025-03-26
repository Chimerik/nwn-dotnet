using Anvil.API;
using static NWN.Systems.PlayerSystem.Player;

namespace NWN.Systems
{
  public static partial class SkillSystem
  {
    public static bool OnLearnInitieArcanique(PlayerSystem.Player player, int customSkillId)
    {
      if (!player.oid.LoginCreature.KnowsFeat((Feat)customSkillId))
        player.oid.LoginCreature.AddFeat((Feat)customSkillId);

      if (!player.windows.TryGetValue("cantripSelection", out var cantrip)) player.windows.Add("cantripSelection", new CantripSelectionWindow(player, ClassType.Wizard, 3, featId: customSkillId));
      else ((CantripSelectionWindow)cantrip).CreateWindow(ClassType.Wizard, 3, featId: customSkillId);

      return true;
    }
  }
}
