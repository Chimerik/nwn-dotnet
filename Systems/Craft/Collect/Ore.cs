using System;
using NWN.Core;
using static NWN.Systems.Craft.Collect.Config;
using NWN.API;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace NWN.Systems.Craft.Collect
{
  public static class Ore
  {
    public static void HandleCompleteCycle(PlayerSystem.Player player, NwGameObject oPlaceable, NwItem oExtractor)
    {
      if (oPlaceable == null)
      {
        player.oid.SendServerMessage("Le filon ciblé n'existe plus, impossible de mener à bien l'extraction.", ColorConstants.Magenta);
        return;
      }

      // TODO : Idée pour plus tard, le strip miner le plus avancé pourra équipper un cristal
      // de spécialisation pour extraire deux fois plus de minerai en un cycle sur son minerai de spécialité
      if (oExtractor == null) return;

      int miningYield = 10;
      miningYield += oExtractor.GetObjectVariable<LocalVariableInt>("_ITEM_LEVEL").Value * 5;
      int bonusYield = 0;

      if (player.learntCustomFeats.ContainsKey(CustomFeats.Miner))
        bonusYield += bonusYield * 5 * SkillSystem.GetCustomFeatLevelFromSkillPoints(CustomFeats.Miner, player.learntCustomFeats[CustomFeats.Miner]) / 100;

      if (player.learntCustomFeats.ContainsKey(CustomFeats.Geology))
        bonusYield += bonusYield * 5 * SkillSystem.GetCustomFeatLevelFromSkillPoints(CustomFeats.Geology, player.learntCustomFeats[CustomFeats.Geology]) / 100;

      miningYield += bonusYield;

      Task playerInput = NwTask.Run(async () =>
      {
        int remainingOre = oPlaceable.GetObjectVariable<LocalVariableInt>("_ORE_AMOUNT").Value - miningYield;
        if (remainingOre <= 0)
        {
          miningYield = oPlaceable.GetObjectVariable<LocalVariableInt>("_ORE_AMOUNT").Value;
          oPlaceable.Destroy();

          NwWaypoint.Create("ore_spawn_wp", oPlaceable.Location);
        }
        else
        {
          oPlaceable.GetObjectVariable<LocalVariableInt>("_ORE_AMOUNT").Value = remainingOre;
        }

        NwItem ore = await NwItem .Create("ore", player.oid.LoginCreature, miningYield, oPlaceable.Name);
        ore.Name = oPlaceable.Name;
      });
      
      ItemUtils.DecreaseItemDurability(oExtractor);
      PlayerSystem.Log.Info("end cycle");
    }
    public static void HandleCompleteProspectionCycle(PlayerSystem.Player player, NwGameObject oPlaceable, NwItem oExtractor)
    {
      if (oPlaceable is null || player.oid.LoginCreature.Distance(oPlaceable) > 5.0f)
      {
        player.oid.SendServerMessage("Vous êtes trop éloigné de la veine ciblée, ou alors celui-ci n'existe plus.", ColorConstants.Magenta);
        return;
      }

      if (oExtractor == null) return;

      var result = SqLiteUtils.SelectQuery("areaResourceStock",
        new List<string>() { { "mining" } },
        new List<string[]>() { new string[] { "areaTag", player.oid.LoginCreature.Area.Tag } });

      if (result.Result == null)
      {
        player.oid.SendServerMessage("Cette veine est épuisée. Reste à espérer qu'un prochain glissement de terrain permette d'atteindre de nouveaux filons.", ColorConstants.Maroon);
        return;
      }

      int skillBonus = 0;

      if (player.learntCustomFeats.ContainsKey(CustomFeats.Geology))
        skillBonus += SkillSystem.GetCustomFeatLevelFromSkillPoints(CustomFeats.Geology, player.learntCustomFeats[CustomFeats.Geology]);

      if (player.learntCustomFeats.ContainsKey(CustomFeats.Prospection))
        skillBonus += SkillSystem.GetCustomFeatLevelFromSkillPoints(CustomFeats.Prospection, player.learntCustomFeats[CustomFeats.Prospection]);

      int respawnChance = skillBonus * 5;
      int nbSpawns = 0;

      if(respawnChance <= 0)
      {
        player.oid.SendServerMessage("Vous ne maîtrisez pas suffisament l'art de la prospection de minerai pour débusquer de nouveaux filons.", ColorConstants.Maroon);
        return;
      }

      foreach (NwWaypoint resourcePoint in player.oid.LoginCreature.GetNearestObjectsByType<NwWaypoint>().Where(w => w.Tag == "ore_spawn_wp"))
      {
        int iRandom = Utils.random.Next(1, 101);
        if (iRandom < respawnChance)
        {
          NwPlaceable newRock = NwPlaceable.Create("mineable_rock", resourcePoint.Location);
          newRock.Name = Enum.GetName(typeof(OreType), GetRandomOreSpawnFromAreaLevel(player.oid.LoginCreature.Area.GetObjectVariable<LocalVariableInt>("_AREA_LEVEL").Value));
          newRock.GetObjectVariable<LocalVariableInt>("_ORE_AMOUNT").Value = 10 * iRandom + 10 * iRandom * skillBonus / 100;
          resourcePoint.Destroy();
          nbSpawns++;
        }
      }

      if (nbSpawns > 0)
      {
        player.oid.SendServerMessage($"Votre prospection a permis de mettre à découvert {nbSpawns} veine(s) de minerai !", ColorConstants.Green);

        SqLiteUtils.UpdateQuery("areaResourceStock",
          new List<string[]>() { new string[] { "mining", "1", "-" } },
          new List<string[]>() { new string[] { "areaTag", player.oid.LoginCreature.Area.Tag } } );
      }
      else
        player.oid.SendServerMessage($"Votre prospection ne semble pas avoir abouti à la découverte d'une veine exploitable", ColorConstants.Maroon);

      ItemUtils.DecreaseItemDurability(oExtractor);
    }
  }
}
