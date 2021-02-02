using System;
using NWN.Core;
using NWN.Core.NWNX;
using static NWN.Systems.Craft.Collect.Config;
using static NWN.Systems.PlayerSystem;

namespace NWN.Systems.Craft.Collect
{
  public static class Wood
  {
    public static void HandleCompleteCycle(Player player, uint oPlaceable, uint oExtractor)
    {
      if (NWScript.GetIsObjectValid(oPlaceable) != 1 || NWScript.GetDistanceBetween(player.oid, oPlaceable) > 5.0f)
      {
        NWScript.SendMessageToPC(player.oid, "Vous êtes trop éloigné de l'arbre ciblé, ou alors celui-ci n'existe plus.");
        return;
      }

      int miningYield = 50;

      // TODO : Idée pour plus tard, le strip miner le plus avancé pourra équipper un cristal
      // de spécialisation pour extraire deux fois plus de minerai en un cycle sur son minerai de spécialité
      if (NWScript.GetIsObjectValid(oExtractor) != 1) return;

      miningYield += NWScript.GetLocalInt(oExtractor, "_ITEM_LEVEL") * 50;
      int bonusYield = 0;

      int value;
      if (int.TryParse(NWScript.Get2DAString("feat", "GAINMULTIPLE", CreaturePlugin.GetHighestLevelOfFeat(player.oid, (int)Feat.WoodCutter)), out value))
        bonusYield += miningYield * value * 5 / 100;

      if (int.TryParse(NWScript.Get2DAString("feat", "GAINMULTIPLE", CreaturePlugin.GetHighestLevelOfFeat(player.oid, (int)Feat.WoodExpertise)), out value))
        bonusYield += miningYield * value * 5 / 100;
      
      miningYield += bonusYield;

      int remainingOre = NWScript.GetLocalInt(oPlaceable, "_ORE_AMOUNT") - miningYield;
      if (remainingOre <= 0)
      {
        miningYield = NWScript.GetLocalInt(oPlaceable, "_ORE_AMOUNT");
        NWScript.DestroyObject(oPlaceable);

        NWScript.CreateObject(NWScript.OBJECT_TYPE_WAYPOINT, "wood_spawn_wp", NWScript.GetLocation(oPlaceable));
      }
      else
      {
        NWScript.SetLocalInt(oPlaceable, "_ORE_AMOUNT", remainingOre);
      }
      var ore = NWScript.CreateItemOnObject("wood", player.oid, miningYield, NWScript.GetName(oPlaceable));
      NWScript.SetName(ore, NWScript.GetName(oPlaceable));

      Items.Utils.DecreaseItemDurability(oExtractor);
    }

    public static void HandleCompleteProspectionCycle(Player player)
    {
      if (!AreaSystem.areaDictionnary.TryGetValue(NWScript.GetObjectUUID(NWScript.GetArea(player.oid)), out Area area)) return;

      if (area.level < 2)
      {
        NWScript.SendMessageToPC(player.oid, "Cet endroit ne semble disposer d'aucune ressource récoltable.");
        return;
      }

      var query = NWScript.SqlPrepareQueryCampaign(ModuleSystem.database, $"SELECT wood from areaResourceStock where areaTag = @areaTag");
      NWScript.SqlBindString(query, "@areaTag", area.tag);
      NWScript.SqlStep(query);

      if (NWScript.SqlGetInt(query, 0) < 1)
      {
        NWScript.SendMessageToPC(player.oid, "Cette zone est épuisée. Les arbres restant disposant de propriétés intéressantes ne semblent pas encore avoir atteint l'âge d'être exploités.");
        return;
      }

      uint resourcePoint = NWScript.GetNearestObjectByTag("wood_spawn_wp", player.oid);
      int i = 1;

      int skillBonus = 0;
      int value;
      if (int.TryParse(NWScript.Get2DAString("feat", "GAINMULTIPLE", CreaturePlugin.GetHighestLevelOfFeat(player.oid, (int)Feat.WoodExpertise)), out value))
        skillBonus += value;

      if (int.TryParse(NWScript.Get2DAString("feat", "GAINMULTIPLE", CreaturePlugin.GetHighestLevelOfFeat(player.oid, (int)Feat.WoodProspection)), out value))
        skillBonus += value;

      int respawnChance = skillBonus * 5;
      int nbSpawns = 0;

      while (NWScript.GetIsObjectValid(resourcePoint) == 1)
      {
        int iRandom = Utils.random.Next(1, 101);
        if (iRandom < respawnChance)
        {
          var newRock = NWScript.CreateObject(NWScript.OBJECT_TYPE_PLACEABLE, "mineable_tree", NWScript.GetLocation(resourcePoint));
          NWScript.SetName(newRock, Enum.GetName(typeof(WoodType), GetRandomWoodSpawnFromAreaLevel(area.level)));
          NWScript.SetLocalInt(newRock, "_ORE_AMOUNT", 50 * iRandom + 50 * iRandom * skillBonus / 100);
          NWScript.DestroyObject(resourcePoint);
          nbSpawns++;
        }

        i++;
        resourcePoint = NWScript.GetNearestObjectByTag("ore_spawn_wp", player.oid, i);
      }

      if (nbSpawns > 0)
      {
        NWScript.SendMessageToPC(player.oid, $"Votre repérage a permis d'identifier {nbSpawns} arbre(s) aux propriétés exploitables !");
        
        query = NWScript.SqlPrepareQueryCampaign(ModuleSystem.database, $"UPDATE areaResourceStock SET wood = wood - 1 where areaTag = @areaTag");
        NWScript.SqlBindString(query, "@areaTag", area.tag);
        NWScript.SqlStep(query);
      }
      else
        NWScript.SendMessageToPC(player.oid, $"Votre repérage semble pas avoir abouti à la découverte d'un arbre aux propriétés exploitables.");
    }
  }
}
