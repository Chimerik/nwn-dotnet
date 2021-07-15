using Anvil.API;
using NWN.Core;
using System;

namespace NWN.Systems.Items.ItemUseHandlers
{
  public static class ResourceExtractor
  {
    public static void HandleActivate(NwItem oItem, NwCreature oPC, NwGameObject oTarget)
    {
      if (!PlayerSystem.Players.TryGetValue(oPC, out PlayerSystem.Player player))
        return;

      if (oTarget == null)
        return;

      if (oPC.Distance(oTarget) > 5.0f)
      {
        player.oid.SendServerMessage("Vous êtes trop éloigné de votre cible pour démarrer l'extraction.", ColorConstants.Orange);
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
          if (oTarget is NwCreature oCreature && oCreature.IsDead)
          {
            Craft.Collect.System.StartCollectCycle(
              player,
              () => Craft.Collect.Pelt.HandleCompleteCycle(player, oTarget, oItem),
              oTarget
            );
          }
          else
            player.oid.SendServerMessage("La cible doit être abattue avant de pouvoir commencer le dépeçage.", ColorConstants.Orange);
          break;

        default:
          player.oid.SendServerMessage($"{oTarget.Name} n'est pas une cible valide pour l'extraction de matières premières.", ColorConstants.Red);
          break;
      }
    }
  }
}
