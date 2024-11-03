using Anvil.API;
using static NWN.Systems.PlayerSystem.Player;

namespace NWN.Systems
{
  public partial class FeatSystem
  {
    private static void ResilienceFielleuse(NwCreature caster)
    {
      if (caster.IsInCombat)
      {
        caster.LoginPlayer?.SendServerMessage("Non utilisable en combat", ColorConstants.Red);
        return;
      }

      if (PlayerSystem.Players.TryGetValue(caster, out var player))
      {
        if (!player.windows.TryGetValue("resilienceFielleuseSelection", out var resist)) player.windows.Add("resilienceFielleuseSelection", new ResilienceFielleuseSelectionWindow(player));
        else ((ResilienceFielleuseSelectionWindow)resist).CreateWindow();
      }
    }
  }
}
