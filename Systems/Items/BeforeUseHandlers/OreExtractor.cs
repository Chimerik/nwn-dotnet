using System;
using NWN.Core;
using NWN.Core.NWNX;
using static NWN.Systems.PlayerSystem;

namespace NWN.Systems.Items.BeforeUseHandlers
{
  public static class OreExtractor
  {
    public static void HandleActivate(uint oItem, Player player, uint oTarget)
    {
      if (NWScript.GetDistanceBetween(player.oid, oTarget) > 5.0f)
      {
        NWScript.SendMessageToPC(player.oid, $"Vous êtes trop éloigné de votre cible pour démarrer l'extraction.");
        return;
      }

      switch (NWScript.GetTag(oTarget))
      {
        case "mineable_rock":
          player.DoActionOnMiningCycleCancelled();
          CollectSystem.StartMiningCycle(
            player,
            oTarget,
            () => HandleCancelCycle(player, oTarget),
            () => HandleCompleteCycle(player, oTarget, oItem)
          );
          break;
        case "fissurerocheuse":
          player.DoActionOnMiningCycleCancelled();
          CollectSystem.StartMiningCycle(
            player,
            oTarget,
            () => HandleCancelCycle(player, oTarget),
            () => HandleCompleteProspectionCycle(player, oTarget, oItem)
          );
          break;
        default:
          NWScript.SendMessageToPC(player.oid, $"{NWScript.GetName(oTarget)} n'est ni un filon, ni une veine de minerai. Impossible de démarrer l'extraction.");
          break;
      }
    }
    private static void HandleCancelCycle(Player player, uint oPlaceable)
    {
      NWN.Utils.RemoveTaggedEffect(oPlaceable, $"_{NWScript.GetPCPublicCDKey(player.oid)}_MINING_BEAM");
      CollectSystem.RemoveMiningCycleCallbacks(player);   // supprimer la callback de CompleteMiningCycle
    }

    private static void HandleCompleteCycle(Player player, uint oPlaceable, uint oExtractor)
    {
      NWN.Utils.RemoveTaggedEffect(oPlaceable, $"_{NWScript.GetPCPublicCDKey(player.oid)}_MINING_BEAM");
      CollectSystem.RemoveMiningCycleCallbacks(player);   // supprimer la callback de Cancel MiningCycle

      if (NWScript.GetIsObjectValid(oPlaceable) != 1 || NWScript.GetDistanceBetween(player.oid, oPlaceable) > 5.0f)
      {
        NWScript.SendMessageToPC(player.oid, "Vous êtes trop éloigné du bloc ciblé, ou alors celui-ci n'existe plus.");
        return;
      }

      int miningYield = 50;

      // TODO : Idée pour plus tard, le strip miner le plus avancé pourra équipper un cristal
      // de spécialisation pour extraire deux fois plus de minerai en un cycle sur son minerai de spécialité
      if (NWScript.GetIsObjectValid(oExtractor) != 1) return;

      miningYield += NWScript.GetLocalInt(oExtractor, "_ITEM_LEVEL") * 50;
      int bonusYield = 0;

      int value;
      if (int.TryParse(NWScript.Get2DAString("feat", "GAINMULTIPLE", CreaturePlugin.GetHighestLevelOfFeat(player.oid, (int)Feat.Miner)), out value))
      {
        bonusYield += miningYield * value * 5 / 100;
      }

      if (int.TryParse(NWScript.Get2DAString("feat", "GAINMULTIPLE", CreaturePlugin.GetHighestLevelOfFeat(player.oid, (int)Feat.Geology)), out value))
      {
        bonusYield += miningYield * value * 5 / 100;
      }

      miningYield += bonusYield;

      int remainingOre = NWScript.GetLocalInt(oPlaceable, "_ORE_AMOUNT") - miningYield;
      if (remainingOre <= 0)
      {
        miningYield = NWScript.GetLocalInt(oPlaceable, "_ORE_AMOUNT");
        NWScript.DestroyObject(oPlaceable);

        var newRessourcePoint = NWScript.CreateObject(NWScript.OBJECT_TYPE_WAYPOINT, "NW_WAYPOINT001", NWScript.GetLocation(oPlaceable));
        NWScript.SetLocalString(newRessourcePoint, "_RESSOURCE_TYPE", NWScript.GetName(oPlaceable));
        NWScript.SetTag(newRessourcePoint, "ressourcepoint");
      }
      else
      {
        NWScript.SetLocalInt(oPlaceable, "_ORE_AMOUNT", remainingOre);
      }

      var ore = NWScript.CreateItemOnObject("ore", player.oid, miningYield, NWScript.GetName(oPlaceable));
      NWScript.SetName(ore, NWScript.GetName(oPlaceable));

      Utils.DecreaseItemDurability(oExtractor);
    }
    private static void HandleCompleteProspectionCycle(Player player, uint oPlaceable, uint oExtractor)
    {
      NWN.Utils.RemoveTaggedEffect(oPlaceable, $"_{NWScript.GetPCPublicCDKey(player.oid)}_MINING_BEAM");
      CollectSystem.RemoveMiningCycleCallbacks(player);   // supprimer la callback de Cancel MiningCycle

      if (!Convert.ToBoolean(NWScript.GetIsObjectValid(oPlaceable)) || NWScript.GetDistanceBetween(player.oid, oPlaceable) > 5.0f)
      {
        NWScript.SendMessageToPC(player.oid, "Vous êtes trop éloigné de la veine ciblée, ou alors celle-ci n'existe plus.");
        return;
      }

      var ressourcePoint = NWScript.GetNearestObjectByTag("ressourcepoint", oPlaceable, 1);
      int i = 2;
      int skillBonus = 0;
      int value;
      if (int.TryParse(NWScript.Get2DAString("feat", "GAINMULTIPLE", CreaturePlugin.GetHighestLevelOfFeat(player.oid, (int)Feat.Geology)), out value))
      {
        skillBonus += value;
      }

      if (int.TryParse(NWScript.Get2DAString("feat", "GAINMULTIPLE", CreaturePlugin.GetHighestLevelOfFeat(player.oid, (int)Feat.Prospection)), out value))
      {
        skillBonus += value;
      }

      int respawnChance = skillBonus * 5;

      while (NWScript.GetIsObjectValid(ressourcePoint) == 1)
      {
        if (NWScript.GetDistanceBetween(oPlaceable, ressourcePoint) > 20.0f)
          break;

        string ressourceType = NWScript.GetLocalString(ressourcePoint, "_RESSOURCE_TYPE");

        int iRandom = NWN.Utils.random.Next(1, 101);

        if (iRandom < respawnChance)
        {
          var newRock = NWScript.CreateObject(NWScript.OBJECT_TYPE_PLACEABLE, "mineable_rock", NWScript.GetLocation(ressourcePoint));
          NWScript.SetName(newRock, ressourceType);
          NWScript.SetLocalInt(newRock, "_ORE_AMOUNT", 200 * iRandom + 200 * iRandom * skillBonus / 100);
          NWScript.DestroyObject(oPlaceable);
        }

        ressourcePoint = NWScript.GetNearestObjectByTag("ressourcepoint", oPlaceable, i);
        i++;
      }
    }
  }
}
