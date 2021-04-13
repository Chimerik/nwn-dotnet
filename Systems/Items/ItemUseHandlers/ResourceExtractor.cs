using NWN.API;
using NWN.Core;
using System;

namespace NWN.Systems.Items.ItemUseHandlers
{
  public static class ResourceExtractor
  {
    public static void HandleActivate(NwItem oItem, NwPlayer oPC, NwGameObject oTarget)
    {
      if (!PlayerSystem.Players.TryGetValue(oPC, out PlayerSystem.Player player))
        return;

      if (oTarget == null)
        return;

      //player.CancelCollectCycle();
      if(player.oid.GetLocalVariable<int>("_COLLECT_IN_PROGRESS").HasValue)
        player.oid.GetLocalVariable<int>("_COLLECT_CANCELLED").Value = 1;

      if (oPC.Distance(oTarget) > 5.0f)
      {
        oPC.SendServerMessage("Vous êtes trop éloigné de votre cible pour démarrer l'extraction.");
        return;
      }

      switch (oTarget.Tag)
      {
        case "mineable_rock":
          Craft.Collect.System.StartCollectCycle(
            player,
            () => Craft.Collect.Ore.HandleCompleteCycle(player, oTarget, oItem),
            oTarget
          );
          break;

        case "fissurerocheuse":
          Craft.Collect.System.StartCollectCycle(
            player,
            () => Craft.Collect.Ore.HandleCompleteProspectionCycle(player, oTarget, oItem),
            oTarget
          );

          //SpawnDisturbedMonsters(player.oid, oTarget);
          break;

        case "mineable_tree":
          Craft.Collect.System.StartCollectCycle(
            player,
            () => Craft.Collect.Wood.HandleCompleteCycle(player, oTarget, oItem),
            oTarget
          );
          break;

        case "mineable_animal":
          if (Convert.ToBoolean(NWScript.GetIsDead(oTarget)))
          {
            Craft.Collect.System.StartCollectCycle(
              player,
              () => Craft.Collect.Pelt.HandleCompleteCycle(player, oTarget, oItem),
              oTarget
            );
          }
          else
            oPC.SendServerMessage("La cible doit être abattue avant de pouvoir commencer le dépeçage.", Color.ORANGE);
          break;

        default:
          oPC.SendServerMessage($"{oTarget.Name} n'est pas une cible valide pour l'extraction de matières premières.", Color.RED);
          break;
      }
    }
  }
}
