using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NWN.API;
using NWN.API.Constants;
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
        player.oid.SendServerMessage("L'animal ciblé n'existe plus, impossible de mener à bien l'extraction.", ColorConstants.Maroon);
        return;
      }

      int miningYield = 10;

      // TODO : Idée pour plus tard, le strip miner le plus avancé pourra équipper un cristal
      // de spécialisation pour extraire deux fois plus de minerai en un cycle sur son minerai de spécialité
      if (oExtractor == null) return;

      miningYield += oExtractor.GetObjectVariable<LocalVariableInt>("_ITEM_LEVEL").Value * 5;
      int bonusYield = 0;

      if (player.learntCustomFeats.ContainsKey(CustomFeats.Skinning))
        bonusYield += miningYield * 5 * SkillSystem.GetCustomFeatLevelFromSkillPoints(CustomFeats.Skinning, player.learntCustomFeats[CustomFeats.Skinning]) / 100;

      if (player.learntCustomFeats.ContainsKey(CustomFeats.AnimalExpertise))
        bonusYield += miningYield * 5 * SkillSystem.GetCustomFeatLevelFromSkillPoints(CustomFeats.AnimalExpertise, player.learntCustomFeats[CustomFeats.AnimalExpertise]) / 100;

      miningYield += bonusYield;

      Task playerInput = NwTask.Run(async () =>
      {
        int remainingOre = oPlaceable.GetObjectVariable<LocalVariableInt>("_ORE_AMOUNT").Value;
        if (remainingOre <= 0)
        {
          miningYield = oPlaceable.GetObjectVariable<LocalVariableInt>("_ORE_AMOUNT").Value;
          oPlaceable.Destroy();

          NwWaypoint.Create("animal_spawn_wp", oPlaceable.Location);
        }
        else
        {
          oPlaceable.GetObjectVariable<LocalVariableInt>("_ORE_AMOUNT").Value = remainingOre;
        }

        NwItem ore = await NwItem.Create("pelt", player.oid.LoginCreature, miningYield, oPlaceable.Name);
        ore.Name = oPlaceable.Name;
      });

      ItemUtils.DecreaseItemDurability(oExtractor);
    }

    public static async void HandleCompleteProspectionCycle(PlayerSystem.Player player)
    {
      NwArea area = player.oid.LoginCreature.Area;

      if (area.GetObjectVariable<LocalVariableInt>("_AREA_LEVEL").Value < 2)
      {
        player.oid.SendServerMessage("Cet endroit ne semble disposer d'aucun animal dont la peau soit réutilisable.", ColorConstants.Maroon);
        return;
      }

      var result = SqLiteUtils.SelectQuery("areaResourceStock",
        new List<string>() { { "animals" } },
        new List<string[]>() { new string[] { "areaTag", player.oid.LoginCreature.Area.Tag } });
      
      if (result.Result == null)
      {
        player.oid.SendServerMessage("Cette zone est épuisée. Les animaux restants disposant de propriétés intéressantes ne semblent pas encore avoir atteint l'âge d'être exploités.", ColorConstants.Maroon);
        return;
      }

      int skillBonus = 0;

      if (player.learntCustomFeats.ContainsKey(CustomFeats.Hunting))
        skillBonus += SkillSystem.GetCustomFeatLevelFromSkillPoints(CustomFeats.Hunting, player.learntCustomFeats[CustomFeats.Hunting]);

      if (player.learntCustomFeats.ContainsKey(CustomFeats.AnimalExpertise))
        skillBonus += SkillSystem.GetCustomFeatLevelFromSkillPoints(CustomFeats.AnimalExpertise, player.learntCustomFeats[CustomFeats.AnimalExpertise]);

      int respawnChance = skillBonus * 5 + (player.oid.LoginCreature.GetSkillRank(Skill.Spot) + player.oid.LoginCreature.GetSkillRank(Skill.Listen)) / 2;
      string nbSpawns = "";

      foreach (NwWaypoint resourcePoint in player.oid.LoginCreature.GetNearestObjectsByType<NwWaypoint>().Where(w => w.Tag == "animal_spawn_wp"))
      {
        int iRandom = Utils.random.Next(1, 101);
        if (iRandom < respawnChance)
        {
          NwCreature newRock = NwCreature.Create(GetRandomPeltSpawnFromAreaLevel(area.GetObjectVariable<LocalVariableInt>("_AREA_LEVEL").Value), resourcePoint.Location);
          newRock.GetObjectVariable<LocalVariableInt>("_ORE_AMOUNT").Value = 10 * iRandom + 10 * iRandom * skillBonus / 100;
          NwItem extractedResource = await NwItem.Create("undroppable_item", newRock);
          extractedResource.Droppable = true;
          resourcePoint.Destroy();
          nbSpawns += newRock.Name + ", ";
        }
      }

      if (nbSpawns.Length > 0)
      {
        player.oid.SendServerMessage($"Votre traque a permis d'identifier les traces des créatures suivantes : {nbSpawns.ColorString(ColorConstants.White)} leurs peaux semblent exploitables, à vous de jouer !", ColorConstants.Green);

        SqLiteUtils.UpdateQuery("areaResourceStock",
          new List<string[]>() { new string[] { "animals", "1", "-" } },
          new List<string[]>() { new string[] { "areaTag", player.oid.LoginCreature.Area.Tag } });
      }
      else
        player.oid.SendServerMessage("Votre traque ne semble pas avoir aboutie au repérage d'animaux aux propriétés exploitables.", ColorConstants.Red);
    }
  }
}
