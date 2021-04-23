using System;
using NWN.Core;
using static NWN.Systems.Craft.Collect.Config;
using NWN.API;
using System.Linq;
using System.Threading.Tasks;

namespace NWN.Systems.Craft.Collect
{
  public static class Ore
  {
    public static void HandleCompleteCycle(PlayerSystem.Player player, NwGameObject oPlaceable, NwItem oExtractor)
    {
      if (oPlaceable == null)
      {
        player.oid.SendServerMessage("Le filon ciblé n'existe plus, impossible de mener à bien l'extraction.", Color.MAGENTA);
        return;
      }

      // TODO : Idée pour plus tard, le strip miner le plus avancé pourra équipper un cristal
      // de spécialisation pour extraire deux fois plus de minerai en un cycle sur son minerai de spécialité
      if (oExtractor == null) return;

      int miningYield = 10;
      miningYield += oExtractor.GetLocalVariable<int>("_ITEM_LEVEL").Value * 5;
      int bonusYield = 0;

      if (player.learntCustomFeats.ContainsKey(CustomFeats.Miner))
        bonusYield += bonusYield * 5 * SkillSystem.GetCustomFeatLevelFromSkillPoints(CustomFeats.Miner, player.learntCustomFeats[CustomFeats.Miner]) / 100;

      if (player.learntCustomFeats.ContainsKey(CustomFeats.Geology))
        bonusYield += bonusYield * 5 * SkillSystem.GetCustomFeatLevelFromSkillPoints(CustomFeats.Geology, player.learntCustomFeats[CustomFeats.Geology]) / 100;

      miningYield += bonusYield;

      Task playerInput = NwTask.Run(async () =>
      {
        await NwModule.Instance.WaitForObjectContext();

        int remainingOre = oPlaceable.GetLocalVariable<int>("_ORE_AMOUNT").Value - miningYield;
        if (remainingOre <= 0)
        {
          miningYield = oPlaceable.GetLocalVariable<int>("_ORE_AMOUNT").Value;
          oPlaceable.Destroy();

          NwWaypoint.Create("ore_spawn_wp", oPlaceable.Location);
        }
        else
        {
          oPlaceable.GetLocalVariable<int>("_ORE_AMOUNT").Value = remainingOre;
        }

        NwItem ore = NwItem.Create("ore", player.oid, miningYield, oPlaceable.Name);
        ore.Name = oPlaceable.Name;
      });
      
      ItemUtils.DecreaseItemDurability(oExtractor);
      PlayerSystem.Log.Info("end cycle");
    }
    public static void HandleCompleteProspectionCycle(PlayerSystem.Player player, NwGameObject oPlaceable, NwItem oExtractor)
    {
      if (oPlaceable is null || player.oid.Distance(oPlaceable) > 5.0f)
      {
        player.oid.SendServerMessage("Vous êtes trop éloigné de la veine ciblée, ou alors celui-ci n'existe plus.", Color.MAGENTA);
        return;
      }

      if (oExtractor == null) return;

      var query = NWScript.SqlPrepareQueryCampaign(Systems.Config.database, $"SELECT mining from areaResourceStock where areaTag = @areaTag");
      NWScript.SqlBindString(query, "@areaTag", player.oid.Area.Tag);

      if (NWScript.SqlStep(query) == 0 || NWScript.SqlGetInt(query, 0) < 1)
      {
        player.oid.SendServerMessage("Cette veine est épuisée. Reste à espérer qu'un prochain glissement de terrain permette d'atteindre de nouveaux filons.", Color.MAROON);
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
        player.oid.SendServerMessage("Vous ne maîtrisez pas suffisament l'art de la prospection de minerai pour débusquer de nouveaux filons.", Color.MAROON);
        return;
      }

      foreach (NwWaypoint resourcePoint in player.oid.GetNearestObjectsByType<NwWaypoint>().Where(w => w.Tag == "ore_spawn_wp"))
      {
        int iRandom = Utils.random.Next(1, 101);
        if (iRandom < respawnChance)
        {
          NwPlaceable newRock = NwPlaceable.Create("mineable_rock", NWScript.GetLocation(resourcePoint));
          newRock.Name = Enum.GetName(typeof(OreType), GetRandomOreSpawnFromAreaLevel(player.oid.Area.GetLocalVariable<int>("_AREA_LEVEL").Value));
          newRock.GetLocalVariable<int>("_ORE_AMOUNT").Value = 10 * iRandom + 10 * iRandom * skillBonus / 100;
          resourcePoint.Destroy();
          nbSpawns++;
        }
      }

      if (nbSpawns > 0)
      {
        player.oid.SendServerMessage($"Votre prospection a permis de mettre à découvert {nbSpawns} veine(s) de minerai !", Color.GREEN);

        query = NWScript.SqlPrepareQueryCampaign(Systems.Config.database, $"UPDATE areaResourceStock SET mining = mining - 1 where areaTag = @areaTag");
        NWScript.SqlBindString(query, "@areaTag", player.oid.Area.Tag);
        NWScript.SqlStep(query);
      }
      else
        player.oid.SendServerMessage($"Votre prospection ne semble pas avoir abouti à la découverte d'une veine exploitable", Color.MAROON);

      ItemUtils.DecreaseItemDurability(oExtractor);
    }
  }
}
