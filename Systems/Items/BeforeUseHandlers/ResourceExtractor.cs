using System;
using NWN.Core;
using NWN.Core.NWNX;
using static NWN.Systems.PlayerSystem;
using static NWN.Systems.Craft.Collect.System;
using NWN.Systems.Craft.Collect;

namespace NWN.Systems.Items.BeforeUseHandlers
{
  public static class ResourceExtractor
  {
    public static void HandleActivate(uint oItem, Player player, uint oTarget)
    {
      player.CancelCollectCycle();

      if (NWScript.GetDistanceBetween(player.oid, oTarget) > 5.0f)
      {
        NWScript.SendMessageToPC(player.oid, $"Vous êtes trop éloigné de votre cible pour démarrer l'extraction.");
        return;
      }

      switch (NWScript.GetTag(oTarget))
      {
        case "mineable_rock":
          StartCollectCycle(
            player,
            oTarget,
            () => Ore.HandleCompleteCycle(player, oTarget, oItem)
          );
          break;
        
        case "fissurerocheuse":
          StartCollectCycle(
            player,
            oTarget,
            () => Ore.HandleCompleteProspectionCycle(player, oTarget, oItem)
          );
          break;

        case "prospectable_tree":
          StartCollectCycle(
            player,
            oTarget,
            () => Wood.HandleCompleteProspectionCycle(player, oTarget, oItem)
          );
          break;

        case "harvestable_tree":
          StartCollectCycle(
            player,
            oTarget,
            () => Wood.HandleCompleteCycle(player, oTarget, oItem)
          );
          break;

        default:
          NWScript.SendMessageToPC(player.oid, $"{NWScript.GetName(oTarget)} n'est pas une cible valide pour l'extraction de matieres premieres.");
          break;
      }
    }
  }
}
