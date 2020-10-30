using System;
using System.Collections.Generic;
using NWN.Core;
using NWN.Core.NWNX;
using NWN.Systems;

namespace NWN.ScriptHandlers
{
  static public class ActivateItemHandlers
  {
    public static Dictionary<string, Func<uint, uint, uint, int>> Register = new Dictionary<string, Func<uint, uint, uint, int>>
    {
            { "MenuTester", HandleMenuTesterActivate },
            { "test_block", HandleBlockTesterActivate },
            { "skillbook", HandleSkillBookActivate },
            { "blueprint", HandleBlueprintActivate },
    };

    private static int HandleMenuTesterActivate(uint oItem, uint oActivator, uint oTarget)
    {
      Console.WriteLine($"You activated the item {NWScript.GetName(oItem)}! {NWScript.GetName(oActivator)}");

      return 0;
    }

    private static int HandleBlockTesterActivate(uint oItem, uint oActivator, uint oTarget)
    {
      PlayerSystem.Player player;
      if (PlayerSystem.Players.TryGetValue(oActivator, out player))
      {
        player.BoulderBlock();
      }

      return 0;
    }
    private static int HandleBlueprintActivate(uint oItem, uint oActivator, uint oTarget)
    {
      PlayerSystem.Player player;
      if (PlayerSystem.Players.TryGetValue(oActivator, out player))
      {
        var item = oItem;
        CollectSystem.Blueprint blueprint;
        CollectSystem.BlueprintType blueprintType = CollectSystem.GetBlueprintTypeFromName(NWScript.GetName(item));

        if(blueprintType == CollectSystem.BlueprintType.Invalid)
        {
          Utils.LogMessageToDMs($"Invalid blueprint : {blueprintType}");
          return 0;
        }

        if (CollectSystem.blueprintDictionnary.ContainsKey(blueprintType))
          blueprint = CollectSystem.blueprintDictionnary[blueprintType];
        else
          blueprint = new CollectSystem.Blueprint(blueprintType);
        
        if(oTarget == NWScript.OBJECT_INVALID)
        {
          int iMineralCost = GetBlueprintMineralCostForPlayer(player, blueprint, item);
          float iJobDuration = GetBlueprintTimeCostForPlayer(player, blueprint, item);

          NWScript.SendMessageToPC(player.oid, $"Patron de création de l'objet artisanal : {blueprint.type}");
          NWScript.SendMessageToPC(player.oid, $"Recherche d'efficacité matérielle niveau {NWScript.GetLocalInt(item, "_BLUEPRINT_MATERIAL_EFFICIENCY")}");
          NWScript.SendMessageToPC(player.oid, $"Coût initial en Tritanium : {iMineralCost}. Puis 10 % de moins par amélioration vers un matériau supérieur.");
          NWScript.SendMessageToPC(player.oid, $"Recherche d'efficacité de production niveau {NWScript.GetLocalInt(item, "_BLUEPRINT_TIME_EFFICIENCY")}");
          NWScript.SendMessageToPC(player.oid, $"Temps de fabrication et d'amélioration : {Utils.GetRemainingTimeAsDisplayableString(iJobDuration)}.");
        }
        else
        {
          if(player.currentCraftJobRemainingTime < 600.0f)
          {
            NWScript.SendMessageToPC(player.oid, $"Impossible d'annuler un travail en cours si près de la fin !");
            return 0;
          }

          if(player.currentCraftJob != "" && !player.craftCancellationConfirmation)
          {
            NWScript.SendMessageToPC(player.oid, $"Attention, votre travail sur l'objet {player.currentCraftJob} n'est pas terminé. Lancer un nouveau travail signifie perdre la totalité du travail en cours !");
            NWScript.SendMessageToPC(player.oid, $"Utilisez une seconde fois le plan pour confirmer l'annulation du travail en cours.");
            player.craftCancellationConfirmation = true;
            NWScript.DelayCommand(60.0f, () => player.ResetCancellationConfirmation());
            return 0;
          }

          var target = oTarget;
          string sMaterial = "";
          if (NWScript.GetTag(target) == blueprint.workshopTag)
            sMaterial = "Tritanium";
          else if(NWScript.GetObjectType(oTarget) == NWScript.OBJECT_TYPE_ITEM && NWScript.GetTag(oTarget) == blueprint.craftedItemTag  && NWScript.GetNearestObjectByTag(blueprint.workshopTag, oActivator) != NWScript.OBJECT_INVALID)
            sMaterial = NWScript.GetLocalString(oTarget, "_ITEM_MATERIAL");

          CollectSystem.MineralType mineralType = CollectSystem.GetMineralTypeFromName(sMaterial);

          if (mineralType == CollectSystem.MineralType.Invalid)
          {
            NWScript.SendMessageToPC(player.oid, "Cet objet ne peut pas être amélioré.");
            return 0;
          }

          int iMineralCost = GetBlueprintMineralCostForPlayer(player, blueprint, item);
          float iJobDuration = GetBlueprintTimeCostForPlayer(player, blueprint, item);
          iMineralCost -= iMineralCost * (int)mineralType / 10;

          var query = NWScript.SqlPrepareQueryCampaign(Scripts.database, $"SELECT @resourceName FROM playerResources where characterId = @characterId");
          NWScript.SqlBindInt(query, "@characterId", player.characterId);
          NWScript.SqlBindString(query, "@resourceName", sMaterial);

          if (Convert.ToBoolean(NWScript.SqlStep(query)))
          {
            int iResourceStock = NWScript.SqlGetInt(query, 0);
            if (iResourceStock >= iMineralCost)
            {
              player.currentCraftJob = NWScript.GetName(item);
              player.currentCraftJobRemainingTime = iJobDuration;
              player.currentCraftJobMaterial = sMaterial;

              query = NWScript.SqlPrepareQueryCampaign(Scripts.database, $"UPDATE playerResources SET @resourceName = @iResourceStock where characterId = @characterId");
              NWScript.SqlBindInt(query, "@characterId", player.characterId);
              NWScript.SqlBindInt(query, "@iResourceStock", iResourceStock - iMineralCost);
              NWScript.SqlBindString(query, "@resourceName", sMaterial);
              NWScript.SqlStep(query);

              NWScript.SendMessageToPC(player.oid, $"Vous venez de démarrer la fabrication de l'objet artisanal : {blueprint.type} en {sMaterial}");
              // TODO : afficher des effets visuels sur la forge
              
              if (NWScript.GetTag(oTarget) == blueprint.craftedItemTag) // En cas d'amélioration d'un objet, on détruit l'original
                NWScript.DestroyObject(oTarget);
              
              // s'il s'agit d'une copie de blueprint, alors le nombre d'utilisation diminue de 1
              int iBlueprintRemainingRuns = NWScript.GetLocalInt(item, "_BLUEPRINT_RUNS");
              if (iBlueprintRemainingRuns == 1)
                NWScript.DestroyObject(item);
              else if(iBlueprintRemainingRuns > 0)
                NWScript.SetLocalInt(item, "_BLUEPRINT_RUNS", iBlueprintRemainingRuns - 1);
            }
            else
              NWScript.SendMessageToPC(player.oid, $"Vous n'avez pas les ressources nécessaires pour démarrer la fabrication de cet objet artisanal.");
          }
          else
            NWScript.SendMessageToPC(player.oid, $"Vous n'avez pas les ressources nécessaires pour démarrer la fabrication de cet objet artisanal.");

          player.craftCancellationConfirmation = false;
        }
      }

      return 0;
    }
    private static int GetBlueprintMineralCostForPlayer(PlayerSystem.Player player, CollectSystem.Blueprint blueprint, uint item)
    {
      int iSkillLevel = 1;

      int value;
      if (int.TryParse(NWScript.Get2DAString("feat", "GAINMULTIPLE", CreaturePlugin.GetHighestLevelOfFeat(player.oid, (int)Feat.Forge)), out value))
        iSkillLevel += value;

      if (int.TryParse(NWScript.Get2DAString("feat", "GAINMULTIPLE", CreaturePlugin.GetHighestLevelOfFeat(player.oid, (int)blueprint.feat)), out value))
        iSkillLevel += value;

      return blueprint.mineralsCost - (blueprint.mineralsCost * (iSkillLevel + NWScript.GetLocalInt(item, "_BLUEPRINT_MATERIAL_EFFICIENCY")) / 100);
    }
    private static float GetBlueprintTimeCostForPlayer(PlayerSystem.Player player, CollectSystem.Blueprint blueprint, uint item)
    {
      int iSkillLevel = 1;
      float fJobDuration = blueprint.mineralsCost / 50;

      int value;
      if (int.TryParse(NWScript.Get2DAString("feat", "GAINMULTIPLE", CreaturePlugin.GetHighestLevelOfFeat(player.oid, (int)Feat.Forge)), out value))
        iSkillLevel += value;

      if (int.TryParse(NWScript.Get2DAString("feat", "GAINMULTIPLE", CreaturePlugin.GetHighestLevelOfFeat(player.oid, (int)blueprint.feat)), out value))
        iSkillLevel += value;

      return fJobDuration - (fJobDuration * (iSkillLevel + NWScript.GetLocalInt(item, "_BLUEPRINT_TIME_EFFICIENCY")) / 100);
    }
    private static int HandleSkillBookActivate(uint oItem, uint oActivator, uint oTarget)
    {
      PlayerSystem.Player player;
      if (PlayerSystem.Players.TryGetValue(oActivator, out player))
      {
        var FeatBook = oItem;
        int FeatId = NWScript.GetLocalInt(FeatBook, "_SKILL_ID");
        if (CreaturePlugin.GetHighestLevelOfFeat(player.oid, FeatId) == (int)Feat.Invalid) 
        {
          SkillBook.pipeline.Execute(new SkillBook.Context(
          oItem: FeatBook,
          oActivator: player,
          SkillId: FeatId
        ));
        }
        else
          NWScript.SendMessageToPC(player.oid, "Vous connaissez déjà les bases d'entrainement de cette capacité");
      }

      return 0;
    }
  }
}
