using Anvil.API;
using static NWN.Systems.PlayerSystem.Player;

namespace NWN.Systems
{
  public partial class FeatSystem
  {
    private static void RestaurationArcanique(NwCreature caster)
    {
      if (caster.IsInCombat)
      {
        caster.LoginPlayer?.SendServerMessage("Non utilisable en combat", ColorConstants.Red);
        return;
      }

      if (PlayerSystem.Players.TryGetValue(caster, out var player))
      {
        if (!player.windows.TryGetValue("restaurationArcanique", out var spellBook)) player.windows.Add("restaurationArcanique", new RestaurationArcaniqueWindow(player));
        else ((RestaurationArcaniqueWindow)spellBook).CreateWindow();
      }
    }
  }
}
