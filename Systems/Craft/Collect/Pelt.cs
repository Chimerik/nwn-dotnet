using System;
using NWN.API;
using NWN.Core;
using NWN.Core.NWNX;
using static NWN.Systems.Craft.Collect.Config;

namespace NWN.Systems.Craft.Collect
{
  public static class Pelt
  {
    public static void HandleCompleteCycle(PlayerSystem.Player player, uint oPlaceable, uint oExtractor)
    {
      if (NWScript.GetIsObjectValid(oPlaceable) != 1 || NWScript.GetDistanceBetween(player.oid, oPlaceable) > 5.0f)
      {
        NWScript.SendMessageToPC(player.oid, "Vous êtes trop éloigné de l'animal ciblé, ou alors celui-ci n'existe plus.");
        return;
      }

      int miningYield = 10;

      // TODO : Idée pour plus tard, le strip miner le plus avancé pourra équipper un cristal
      // de spécialisation pour extraire deux fois plus de minerai en un cycle sur son minerai de spécialité
      if (NWScript.GetIsObjectValid(oExtractor) != 1) return;

      miningYield += NWScript.GetLocalInt(oExtractor, "_ITEM_LEVEL") * 5;
      int bonusYield = 0;

      int value;
      if (int.TryParse(NWScript.Get2DAString("feat", "GAINMULTIPLE", CreaturePlugin.GetHighestLevelOfFeat(player.oid, (int)Feat.Skinning)), out value))
        bonusYield += miningYield * value * 5 / 100;

      if (int.TryParse(NWScript.Get2DAString("feat", "GAINMULTIPLE", CreaturePlugin.GetHighestLevelOfFeat(player.oid, (int)Feat.AnimalExpertise)), out value))
        bonusYield += miningYield * value * 5 / 100;

      miningYield += bonusYield;

      int remainingOre = NWScript.GetLocalInt(oPlaceable, "_ORE_AMOUNT") - miningYield;
      if (remainingOre <= 0)
      {
        miningYield = NWScript.GetLocalInt(oPlaceable, "_ORE_AMOUNT");
        NWScript.DestroyObject(oPlaceable);

        NWScript.CreateObject(NWScript.OBJECT_TYPE_WAYPOINT, "animal_spawn_wp", NWScript.GetLocation(oPlaceable));
      }
      else
      {
        NWScript.SetLocalInt(oPlaceable, "_ORE_AMOUNT", remainingOre);
      }

      var ore = NWScript.CreateItemOnObject("pelt", player.oid, miningYield, NWScript.GetResRef(oPlaceable));
      NWScript.SetName(ore, $"Peau de {NWScript.GetName(oPlaceable)}");
      //NWScript.SetLocalString(ore, "_PELT_RESREF", NWScript.GetResRef(oPlaceable));

      ItemUtils.DecreaseItemDurability(oExtractor);
    }

    public static void HandleCompleteProspectionCycle(PlayerSystem.Player player)
    {
      NwArea area = NWScript.GetArea(player.oid).ToNwObject<NwArea>();

      if (area.GetLocalVariable<int>("_AREA_LEVEL").Value < 2)
      {
        NWScript.SendMessageToPC(player.oid, "Cet endroit ne semble disposer d'aucun animal dont la peau soit réutilisable.");
        return;
      }

      var query = NWScript.SqlPrepareQueryCampaign(Systems.Config.database, $"SELECT animals from areaResourceStock where areaTag = @areaTag");
      NWScript.SqlBindString(query, "@areaTag", area.Tag);
      NWScript.SqlStep(query);

      if (NWScript.SqlGetInt(query, 0) < 1)
      {
        NWScript.SendMessageToPC(player.oid, "Cette zone est épuisée. Les animaux restants disposant de propriétés intéressantes ne semblent pas encore avoir atteint l'âge d'être exploités.");
        return;
      }

      uint resourcePoint = NWScript.GetNearestObjectByTag("animal_spawn_wp", player.oid);
      int i = 1;

      int skillBonus = 0;
      int value;
      if (int.TryParse(NWScript.Get2DAString("feat", "GAINMULTIPLE", CreaturePlugin.GetHighestLevelOfFeat(player.oid, (int)Feat.AnimalExpertise)), out value))
        skillBonus += value;

      if (int.TryParse(NWScript.Get2DAString("feat", "GAINMULTIPLE", CreaturePlugin.GetHighestLevelOfFeat(player.oid, (int)Feat.Hunting)), out value))
        skillBonus += value;

      int respawnChance = skillBonus * 5 + (NWScript.GetSkillRank(NWScript.SKILL_SPOT, player.oid) + NWScript.GetSkillRank(NWScript.SKILL_LISTEN, player.oid)) / 2;
      string nbSpawns = "";

      while (NWScript.GetIsObjectValid(resourcePoint) == 1)
      {
        int iRandom = NwRandom.Roll(Utils.random, 100, 1);

        if (iRandom < respawnChance || Systems.Config.env == Systems.Config.Env.Chim)
        {
          var newRock = NWScript.CreateObject(NWScript.OBJECT_TYPE_CREATURE, GetRandomPeltSpawnFromAreaLevel(area.GetLocalVariable<int>("_AREA_LEVEL").Value), NWScript.GetLocation(resourcePoint));
          NWScript.SetLocalInt(newRock, "_ORE_AMOUNT", 50 * iRandom + 50 * iRandom * skillBonus / 100);
          NWScript.SetDroppableFlag(NWScript.CreateItemOnObject("undroppable_item", newRock), 1);
          NWScript.DestroyObject(resourcePoint);
          nbSpawns += NWScript.GetName(newRock) + ", ";
        }

        i++;
        resourcePoint = NWScript.GetNearestObjectByTag("animal_spawn_wp", player.oid, i);
      }

      if (nbSpawns.Length > 0)
      {
        NWScript.SendMessageToPC(player.oid, $"Votre traque a permis d'identifier les traces des créatures suivantes : {nbSpawns} leurs peaux semblent exploitables, à vous de jouer !");

        query = NWScript.SqlPrepareQueryCampaign(Systems.Config.database, $"UPDATE areaResourceStock SET animals = animals - 1 where areaTag = @areaTag");
        NWScript.SqlBindString(query, "@areaTag", area.Tag);
        NWScript.SqlStep(query);
      }
      else
        player.oid.SendServerMessage("Votre traque ne semble pas avoir aboutie au repérage d'animaux aux propriétés exploitables.", Color.OLIVE);
    }
  }
}
