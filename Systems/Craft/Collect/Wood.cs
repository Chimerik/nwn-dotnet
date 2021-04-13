using System;
using System.Linq;
using NWN.API;
using NWN.Core;
using NWN.Core.NWNX;
using static NWN.Systems.Craft.Collect.Config;

namespace NWN.Systems.Craft.Collect
{
  public static class Wood
  {
    public static void HandleCompleteCycle(PlayerSystem.Player player, NwGameObject oPlaceable, NwItem oExtractor)
    {
      if (oPlaceable == null || player.oid.Distance(oPlaceable) > 5.0f)
      {
        player.oid.SendServerMessage("Vous êtes trop éloigné de l'arbre ciblé, ou alors celui-ci n'existe plus.", Color.MAGENTA);
        return;
      }

      int miningYield = 10;

      // TODO : Idée pour plus tard, le strip miner le plus avancé pourra équipper un cristal
      // de spécialisation pour extraire deux fois plus de minerai en un cycle sur son minerai de spécialité
      if (oExtractor == null) return;

      miningYield += oExtractor.GetLocalVariable<int>("_ITEM_LEVEL").Value * 5;
      int bonusYield = 0;

      if (player.learntCustomFeats.ContainsKey(CustomFeats.WoodCutter))
        bonusYield += bonusYield * 5 * SkillSystem.GetCustomFeatLevelFromSkillPoints(CustomFeats.WoodCutter, player.learntCustomFeats[CustomFeats.WoodCutter]) / 100;
      
      if (player.learntCustomFeats.ContainsKey(CustomFeats.WoodExpertise))
        bonusYield += bonusYield * 5 * SkillSystem.GetCustomFeatLevelFromSkillPoints(CustomFeats.WoodExpertise, player.learntCustomFeats[CustomFeats.WoodExpertise]) / 100;

      miningYield += bonusYield;

      int remainingOre = oPlaceable.GetLocalVariable<int>("_ORE_AMOUNT").Value - miningYield;
      if (remainingOre <= 0)
      {
        miningYield = oPlaceable.GetLocalVariable<int>("_ORE_AMOUNT").Value;
        oPlaceable.Destroy();

        NwWaypoint.Create("wood_spawn_wp", oPlaceable.Location);
      }
      else
      {
        oPlaceable.GetLocalVariable<int>("_ORE_AMOUNT").Value = remainingOre;
      }

      NwItem ore = NWScript.CreateItemOnObject("wood", player.oid, miningYield, oPlaceable.Name).ToNwObject<NwItem>();
      ore.Name = oPlaceable.Name;

      ItemUtils.DecreaseItemDurability(oExtractor);
    }

    public static void HandleCompleteProspectionCycle(PlayerSystem.Player player)
    {
      NwArea area = player.oid.Area;

      if (area.GetLocalVariable<int>("_AREA_LEVEL").Value < 2)
      {
        player.oid.SendServerMessage("Cet endroit ne semble disposer d'aucune ressource récoltable.", Color.MAROON);
        return;
      }

      var query = NWScript.SqlPrepareQueryCampaign(Systems.Config.database, $"SELECT wood from areaResourceStock where areaTag = @areaTag");
      NWScript.SqlBindString(query, "@areaTag", area.Tag);
      
      if (NWScript.SqlStep(query) == 0 || NWScript.SqlGetInt(query, 0) < 1)
      {
        player.oid.SendServerMessage("Cette zone est épuisée. Les arbres restant disposant de propriétés intéressantes ne semblent pas encore avoir atteint l'âge d'être exploités.", Color.MAROON);
        return;
      }

      int skillBonus = 0;

      if (player.learntCustomFeats.ContainsKey(CustomFeats.WoodExpertise))
        skillBonus += SkillSystem.GetCustomFeatLevelFromSkillPoints(CustomFeats.WoodExpertise, player.learntCustomFeats[CustomFeats.WoodExpertise]);

      if (player.learntCustomFeats.ContainsKey(CustomFeats.WoodProspection))
        skillBonus += SkillSystem.GetCustomFeatLevelFromSkillPoints(CustomFeats.WoodProspection, player.learntCustomFeats[CustomFeats.WoodProspection]);

      int respawnChance = skillBonus * 5;
      int nbSpawns = 0;

      foreach(NwWaypoint resourcePoint in player.oid.GetNearestObjectsByType<NwWaypoint>().Where(w => w.Tag == "wood_spawn_wp"))
      {
        int iRandom = Utils.random.Next(1, 101);
        if (iRandom < respawnChance)
        {
          NwPlaceable newRock = NwPlaceable.Create("mineable_tree", NWScript.GetLocation(resourcePoint));
          newRock.Name = Enum.GetName(typeof(WoodType), GetRandomWoodSpawnFromAreaLevel(area.GetLocalVariable<int>("_AREA_LEVEL").Value));
          newRock.GetLocalVariable<int>("_ORE_AMOUNT").Value = 10 * iRandom + 10 * iRandom * skillBonus / 100;
          resourcePoint.Destroy();
          nbSpawns++;
        }
      }

      if (nbSpawns > 0)
      {
        player.oid.SendServerMessage($"Votre repérage a permis d'identifier {nbSpawns} arbre(s) aux propriétés exploitables !", Color.GREEN);

        query = NWScript.SqlPrepareQueryCampaign(Systems.Config.database, $"UPDATE areaResourceStock SET wood = wood - 1 where areaTag = @areaTag");
        NWScript.SqlBindString(query, "@areaTag", area.Tag);
        NWScript.SqlStep(query);
      }
      else
        player.oid.SendServerMessage($"Votre repérage semble pas avoir abouti à la découverte d'un arbre aux propriétés exploitables.", Color.MAROON);
    }
  }
}
