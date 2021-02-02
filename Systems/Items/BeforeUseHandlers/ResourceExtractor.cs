using NWN.Core;
using static NWN.Systems.PlayerSystem;
using static NWN.Systems.Craft.Collect.System;
using NWN.Systems.Craft.Collect;
using System;

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

          //SpawnDisturbedMonsters(player.oid, oTarget);

          break;

        case "mineable_tree":
          StartCollectCycle(
            player,
            oTarget,
            () => Wood.HandleCompleteCycle(player, oTarget, oItem)
          );
          break;

        case "mineable_animal":
          if (Convert.ToBoolean(NWScript.GetIsDead(oTarget)))
          {
            StartCollectCycle(
              player,
              oTarget,
              () => Pelt.HandleCompleteCycle(player, oTarget, oItem)
            );
          }
          else
            NWScript.SendMessageToPC(player.oid, "La cible doit être abattue avant de pouvoir commencer le dépeçage.");
          break;

        default:
          NWScript.SendMessageToPC(player.oid, $"{NWScript.GetName(oTarget)} n'est pas une cible valide pour l'extraction de matières premières.");
          break;
      }
    }
    private static void SpawnDisturbedMonsters(uint oPlayer, uint oVeine)
    {
      NWScript.SendMessageToPC(oPlayer, "Le boucan déclenché par l'extraction risque d'attirer les monstres locaux, jusque là tapis dans l'ombre !");
      uint oMonsterWaypoint = NWScript.GetNearestObjectByTag("disturbed_creature_spawn", oVeine);
      int i = 1;

      while (Convert.ToBoolean(NWScript.GetIsObjectValid(oMonsterWaypoint)) && NWScript.GetDistanceBetween(oMonsterWaypoint, oVeine) < 45.0f)
      {
        NWScript.CreateObject(NWScript.OBJECT_TYPE_CREATURE, NWScript.GetLocalString(oMonsterWaypoint, "_CREATURE_TEMPLATE"), NWScript.GetLocation(oMonsterWaypoint));
        i++;
        oMonsterWaypoint = NWScript.GetNearestObjectByTag("disturbed_creature_spawn", oVeine, i);
      }
    }
  }
}
