using Anvil.API;

namespace NWN.Systems
{
  public partial class FeatSystem
  {
    private static void ShortRest(NwCreature caster)
    {
      // TODO : Utiliser le feat déclenche un job d'une heure, à l'issue du job le short rest est déclenché. Tout combat annule le job
      // TODO : Utiliser une potion de short rest est instantannée, mais coûte cher

      if (caster.IsInCombat)
      {
        caster.LoginPlayer?.SendServerMessage("Ne peut être utilisé en combat", ColorConstants.Red);
        return;
      }

      if (PlayerSystem.Players.TryGetValue(caster, out var player))
      {
        if(player.shortRest < 2)
          CreatureUtils.HandleShortRest(player);
        else
          player.oid.SendServerMessage("Vous ne pouvez plus prendre de repos court avant votre prochain long repos", ColorConstants.Orange);
      }
    }
  }
}
