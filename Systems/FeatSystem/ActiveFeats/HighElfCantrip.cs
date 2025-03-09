using Anvil.API;
using static NWN.Systems.PlayerSystem.Player;

namespace NWN.Systems
{
  public partial class FeatSystem
  {
    private static void HighElfCantrip(NwCreature caster)
    {
      if (PlayerSystem.Players.TryGetValue(caster, out var player))
      {
        caster.DecrementRemainingFeatUses((Feat)CustomSkill.HighElfCantrip);

        if (!player.windows.TryGetValue("highElfCantripSelection", out var cantrip)) player.windows.Add("highElfCantripSelection", new HighElfCantripSelectionWindow(player));
        else ((HighElfCantripSelectionWindow)cantrip).CreateWindow();
      }
    }
  }
}
