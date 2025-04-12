using Anvil.API;

namespace NWN.Systems
{
  public partial class FeatSystem
  {
    private static void LongRest(NwCreature caster)
    {
      // TODO : Utiliser le feat déclenche un job de 8 heures, à l'issue du job le long rest est déclenché. Tout combat annule le job
      // TODO : Utiliser une potion de long rest est instantannée, mais coûte ridiculement cher

      if (caster.IsInCombat)
      {
        caster.LoginPlayer?.SendServerMessage("Ne peut être utilisé en combat", ColorConstants.Red);
        return;
      }

      if (PlayerSystem.Players.TryGetValue(caster, out var player))
        CreatureUtils.HandleLongRest(player);
    }
  }
}
