using System;
using System.Linq;
using System.Threading.Tasks;
using NWN.API;
using NWN.Core;
using NWN.Core.NWNX;
using static NWN.Systems.Craft.Collect.Config;

namespace NWN.Systems.Craft.Collect
{
  public static class Pelt
  {
    public static void HandleCompleteCycle(PlayerSystem.Player player, NwGameObject oPlaceable, NwItem oExtractor)
    {
      if (oPlaceable == null)
      {
        player.oid.SendServerMessage("L'animal ciblé n'existe plus, impossible de mener à bien l'extraction.", Color.MAROON);
        return;
      }

      int miningYield = 10;

      // TODO : Idée pour plus tard, le strip miner le plus avancé pourra équipper un cristal
      // de spécialisation pour extraire deux fois plus de minerai en un cycle sur son minerai de spécialité
      if (oExtractor == null) return;

      miningYield += oExtractor.GetLocalVariable<int>("_ITEM_LEVEL").Value * 5;
      int bonusYield = 0;

      if (player.learntCustomFeats.ContainsKey(CustomFeats.Skinning))
        bonusYield += miningYield * 5 * SkillSystem.GetCustomFeatLevelFromSkillPoints(CustomFeats.Skinning, player.learntCustomFeats[CustomFeats.Skinning]) / 100;

      if (player.learntCustomFeats.ContainsKey(CustomFeats.AnimalExpertise))
        bonusYield += miningYield * 5 * SkillSystem.GetCustomFeatLevelFromSkillPoints(CustomFeats.AnimalExpertise, player.learntCustomFeats[CustomFeats.AnimalExpertise]) / 100;

      miningYield += bonusYield;

      int remainingOre = oPlaceable.GetLocalVariable<int>("_ORE_AMOUNT").Value; 
      if (remainingOre <= 0)
      {
        miningYield = oPlaceable.GetLocalVariable<int>("_ORE_AMOUNT").Value;
        oPlaceable.Destroy();

        NwWaypoint.Create("animal_spawn_wp", oPlaceable.Location);
      }
      else
      {
        oPlaceable.GetLocalVariable<int>("_ORE_AMOUNT").Value = remainingOre;
      }

      Task playerInput = NwTask.Run(async () =>
      {
        await NwModule.Instance.WaitForObjectContext();
        NwItem ore = NwItem.Create("pelt", player.oid, miningYield, oPlaceable.Name);
        ore.Name = oPlaceable.Name;
      });

      ItemUtils.DecreaseItemDurability(oExtractor);
    }

    public static void HandleCompleteProspectionCycle(PlayerSystem.Player player)
    {
      NwArea area = player.oid.Area;

      if (area.GetLocalVariable<int>("_AREA_LEVEL").Value < 2)
      {
        player.oid.SendServerMessage("Cet endroit ne semble disposer d'aucun animal dont la peau soit réutilisable.", Color.MAROON);
        return;
      }

      var query = NWScript.SqlPrepareQueryCampaign(Systems.Config.database, $"SELECT animals from areaResourceStock where areaTag = @areaTag");
      NWScript.SqlBindString(query, "@areaTag", area.Tag);
      
      if (NWScript.SqlStep(query) == 0 || NWScript.SqlGetInt(query, 0) < 1)
      {
        player.oid.SendServerMessage("Cette zone est épuisée. Les animaux restants disposant de propriétés intéressantes ne semblent pas encore avoir atteint l'âge d'être exploités.", Color.MAROON);
        return;
      }

      int skillBonus = 0;

      if (player.learntCustomFeats.ContainsKey(CustomFeats.Hunting))
        skillBonus += SkillSystem.GetCustomFeatLevelFromSkillPoints(CustomFeats.Hunting, player.learntCustomFeats[CustomFeats.Hunting]);

      if (player.learntCustomFeats.ContainsKey(CustomFeats.AnimalExpertise))
        skillBonus += SkillSystem.GetCustomFeatLevelFromSkillPoints(CustomFeats.AnimalExpertise, player.learntCustomFeats[CustomFeats.AnimalExpertise]);

      int respawnChance = skillBonus * 5 + (NWScript.GetSkillRank(NWScript.SKILL_SPOT, player.oid) + NWScript.GetSkillRank(NWScript.SKILL_LISTEN, player.oid)) / 2;
      string nbSpawns = "";

      foreach (NwWaypoint resourcePoint in player.oid.GetNearestObjectsByType<NwWaypoint>().Where(w => w.Tag == "animal_spawn_wp"))
      {
        int iRandom = Utils.random.Next(1, 101);
        if (iRandom < respawnChance)
        {
          NwCreature newRock = NwCreature.Create(GetRandomPeltSpawnFromAreaLevel(area.GetLocalVariable<int>("_AREA_LEVEL").Value), resourcePoint.Location);
          newRock.GetLocalVariable<int>("_ORE_AMOUNT").Value = 10 * iRandom + 10 * iRandom * skillBonus / 100;
          NwItem.Create("undroppable_item", newRock).Droppable = true;
          resourcePoint.Destroy();
          nbSpawns += newRock.Name + ", ";
        }
      }

      if (nbSpawns.Length > 0)
      {
        player.oid.SendServerMessage($"Votre traque a permis d'identifier les traces des créatures suivantes : {nbSpawns.ColorString(Color.WHITE)} leurs peaux semblent exploitables, à vous de jouer !", Color.GREEN);

        query = NWScript.SqlPrepareQueryCampaign(Systems.Config.database, $"UPDATE areaResourceStock SET animals = animals - 1 where areaTag = @areaTag");
        NWScript.SqlBindString(query, "@areaTag", area.Tag);
        NWScript.SqlStep(query);
      }
      else
        player.oid.SendServerMessage("Votre traque ne semble pas avoir aboutie au repérage d'animaux aux propriétés exploitables.", Color.OLIVE);
    }
  }
}
