using NWN.API;
using NWN.Core;
using System;

namespace NWN.Systems.Items.ItemUseHandlers
{
    public static class ResourceExtractor
    {
        public static void HandleActivate(NwItem oItem, NwPlayer oPC, uint oTarget)
        {
            if (!PlayerSystem.Players.TryGetValue(oPC, out PlayerSystem.Player player))
                return;

            NwGameObject target = oTarget.ToNwObject<NwGameObject>();

            player.CancelCollectCycle();

            if (oPC.Distance(target) > 5.0f)
            {
                oPC.SendServerMessage("Vous êtes trop éloigné de votre cible pour démarrer l'extraction.");
                return;
            }

            switch (target.Tag)
            {
                case "mineable_rock":
                    Craft.Collect.System.StartCollectCycle(
                      player,
                      oTarget,
                      () => Craft.Collect.Ore.HandleCompleteCycle(player, oTarget, oItem)
                    );
                    break;

                case "fissurerocheuse":
                    Craft.Collect.System.StartCollectCycle(
                      player,
                      oTarget,
                      () => Craft.Collect.Ore.HandleCompleteProspectionCycle(player, oTarget, oItem)
                    );

                    //SpawnDisturbedMonsters(player.oid, oTarget);

                    break;

                case "mineable_tree":
                    Craft.Collect.System.StartCollectCycle(
                      player,
                      oTarget,
                      () => Craft.Collect.Wood.HandleCompleteCycle(player, oTarget, oItem)
                    );
                    break;

                case "mineable_animal":
                    if (Convert.ToBoolean(NWScript.GetIsDead(oTarget)))
                    {
                        Craft.Collect.System.StartCollectCycle(
                          player,
                          oTarget,
                          () => Craft.Collect.Pelt.HandleCompleteCycle(player, oTarget, oItem)
                        );
                    }
                    else
                        oPC.SendServerMessage("La cible doit être abattue avant de pouvoir commencer le dépeçage.");
                    break;

                default:
                    oPC.SendServerMessage($"{target.Name} n'est pas une cible valide pour l'extraction de matières premières.");
                    break;
            }
        }
    }
}
