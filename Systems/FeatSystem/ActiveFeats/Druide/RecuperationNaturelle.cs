using Anvil.API;
using static NWN.Systems.PlayerSystem.Player;

namespace NWN.Systems
{
  public partial class FeatSystem
  {
    private static void RecuperationNaturelle(NwCreature caster)
    {
      if (caster.IsInCombat)
      {
        caster.LoginPlayer?.SendServerMessage("Non utilisable en combat", ColorConstants.Red);
        return;
      }

      if (PlayerSystem.Players.TryGetValue(caster, out var player))
      {
        if (!player.windows.TryGetValue("recuperationNaturelle", out var spellBook)) player.windows.Add("recuperationNaturelle", new RecuperationNaturelleWindow(player));
        else ((RecuperationNaturelleWindow)spellBook).CreateWindow();
      }
    }
  }
}
