using Anvil.API;
using static NWN.Systems.PlayerSystem;

namespace NWN.Systems
{
  public partial class FeatSystem
  {
    private static void ReveilSauvage(NwCreature caster)
    {
      if(caster.GetFeatRemainingUses((Feat)CustomSkill.DruideCompagnonSauvage) > 0)
      {
        caster.LoginPlayer?.SendServerMessage("Vous n'avez pas encore épuisé toute vos utilisation de Forme Sauvage", ColorConstants.Red);
        return;
      }

      if (Players.TryGetValue(caster, out var player))
      {
        if (!player.windows.TryGetValue("reveilSauvage", out var reveilSauvage))
          player.windows.Add("reveilSauvage", new Player.ReveilSauvageWindow(player));
        else if (((Player.ReveilSauvageWindow)reveilSauvage).IsOpen)
          ((Player.ReveilSauvageWindow)reveilSauvage).CloseWindow();
        else
          ((Player.ReveilSauvageWindow)reveilSauvage).CreateWindow();
      }
    }
  }
}
