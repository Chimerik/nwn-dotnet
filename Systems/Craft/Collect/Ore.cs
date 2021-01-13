using System;
using NWN.Core;
using NWN.Core.NWNX;
using static NWN.Systems.Craft.Collect.Config;
using static NWN.Systems.PlayerSystem;

namespace NWN.Systems.Craft.Collect
{
  public static class Ore
  {
    public static void HandleCompleteCycle(Player player, uint oPlaceable, uint oExtractor)
    {
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
        bonusYield += miningYield * value * 5 / 100;

      if (int.TryParse(NWScript.Get2DAString("feat", "GAINMULTIPLE", CreaturePlugin.GetHighestLevelOfFeat(player.oid, (int)Feat.Geology)), out value))
        bonusYield += miningYield * value * 5 / 100;

      miningYield += bonusYield;

      int remainingOre = NWScript.GetLocalInt(oPlaceable, "_ORE_AMOUNT") - miningYield;
      if (remainingOre <= 0)
      {
        miningYield = NWScript.GetLocalInt(oPlaceable, "_ORE_AMOUNT");
        NWScript.DestroyObject(oPlaceable);

        NWScript.CreateObject(NWScript.OBJECT_TYPE_WAYPOINT, "ore_spawn_wp", NWScript.GetLocation(oPlaceable));
      }
      else
      {
        NWScript.SetLocalInt(oPlaceable, "_ORE_AMOUNT", remainingOre);
      }
      var ore = NWScript.CreateItemOnObject("ore", player.oid, miningYield, NWScript.GetName(oPlaceable));
      NWScript.SetName(ore, NWScript.GetName(oPlaceable));

      Items.Utils.DecreaseItemDurability(oExtractor);
    }
    public static void HandleCompleteProspectionCycle(Player player, uint oPlaceable, uint oExtractor)
    {
      if (!Convert.ToBoolean(NWScript.GetIsObjectValid(oPlaceable)) || NWScript.GetDistanceBetween(player.oid, oPlaceable) > 5.0f)
      {
        NWScript.SendMessageToPC(player.oid, "Vous êtes trop éloigné de la veine ciblée, ou alors celle-ci n'existe plus.");
        return;
      }

      if (NWScript.GetIsObjectValid(oExtractor) != 1) return;

      uint resourcePoint = NWScript.GetNearestObjectByTag("ore_spawn_wp", oPlaceable);
      int i = 1;

      Area area;
      if (!Module.areaDictionnary.TryGetValue(NWScript.GetObjectUUID(NWScript.GetArea(resourcePoint)), out area)) return;

      int skillBonus = 0;
      int value;
      if (int.TryParse(NWScript.Get2DAString("feat", "GAINMULTIPLE", CreaturePlugin.GetHighestLevelOfFeat(player.oid, (int)Feat.Geology)), out value))
        skillBonus += value;

      if (int.TryParse(NWScript.Get2DAString("feat", "GAINMULTIPLE", CreaturePlugin.GetHighestLevelOfFeat(player.oid, (int)Feat.Prospection)), out value))
        skillBonus += value;

      int respawnChance = skillBonus * 5;
      int nbSpawns = 0;

      while (NWScript.GetIsObjectValid(resourcePoint) == 1)
      {
        int iRandom = Utils.random.Next(1, 101);
        if (iRandom < respawnChance)
        {
          var newRock = NWScript.CreateObject(NWScript.OBJECT_TYPE_PLACEABLE, "mineable_rock", NWScript.GetLocation(resourcePoint));
          NWScript.SetName(newRock, Enum.GetName(typeof(OreType), GetRandomOreSpawnFromAreaLevel(area.level)));
          NWScript.SetLocalInt(newRock, "_ORE_AMOUNT", 50 * iRandom + 50 * iRandom * skillBonus / 100);
          NWScript.DestroyObject(oPlaceable);
          nbSpawns++; 
        }

        i++;
        resourcePoint = NWScript.GetNearestObjectByTag("ore_spawn_wp", oPlaceable, i);
      }

      if(nbSpawns > 0)
        NWScript.SendMessageToPC(player.oid, $"Votre prospection a permis de mettre à découvert {nbSpawns} veine(s) de minerai !");
      else
        NWScript.SendMessageToPC(player.oid, $"Votre prospection ne semble pas avoir abouti à la découverte d'une veine exploitable");

      Items.Utils.DecreaseItemDurability(oExtractor);
    }
  }
}
