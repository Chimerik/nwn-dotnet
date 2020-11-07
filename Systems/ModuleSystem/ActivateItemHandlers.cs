using System;
using System.Collections.Generic;
using NWN.Core;
using NWN.Core.NWNX;
using NWN.Systems;
using static NWN.Systems.Blueprint;
using static NWN.Systems.PlayerSystem;

namespace NWN.Systems
{
  static public class ActivateItemHandlers
  {
    public static Dictionary<string, Func<uint, uint, uint, int>> Register = new Dictionary<string, Func<uint, uint, uint, int>>
    {
            { "MenuTester", HandleMenuTesterActivate },
            { "test_block", HandleBlockTesterActivate },
            { "skillbook", HandleSkillBookActivate },
            { "blueprint", HandleBlueprintActivate },
            { "loot_saver", HandleLootSaverActivate },
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
    private static int HandleLootSaverActivate(uint oItem, uint oActivator, uint oTarget)
    {
      if (Convert.ToBoolean(NWScript.GetIsDM(oActivator))
        && NWScript.GetTag(NWScript.GetArea(oActivator)) == LootSystem.CHEST_AREA_TAG
        && NWScript.GetObjectType(oTarget) == NWScript.OBJECT_TYPE_PLACEABLE 
        && Convert.ToBoolean(NWScript.GetHasInventory(oTarget)))
      {
        Player oPC;
        if (Players.TryGetValue(oActivator, out oPC))
        {
          NWScript.SetEventScript(oTarget, NWScript.EVENT_SCRIPT_PLACEABLE_ON_CLOSED, LootSystem.LOOT_CONTAINER_ON_CLOSE_SCRIPT);

          var query = NWScript.SqlPrepareQueryCampaign(ModuleSystem.database, "INSERT INTO loot_containers(chestTag, accountID, serializedPlaceable, position, facing)" +
          " VALUES(@chestTag, @accountId, @serializedPlaceable, @position, @facing)");
          NWScript.SqlBindString(query, "@chestTag", NWScript.GetTag(oTarget));
          NWScript.SqlBindInt(query, "@accountId", oPC.accountId);
          NWScript.SqlBindObject(query, "@serializedPlaceable", oTarget);
          NWScript.SqlBindVector(query, "@position", NWScript.GetPosition(oTarget));
          NWScript.SqlBindFloat(query, "@facing", NWScript.GetFacing(oTarget));
          NWScript.SqlStep(query);
        }
      }
      else
        NWScript.SendMessageToPC(oActivator, "Cet objet ne peut être utilisé que par un dm, dans la zone de configuration des loots, sur un coffre disposant d'un inventaire.");

      Utils.LogMessageToDMs($"Loot Saver - Utilisation par {NWScript.GetName(oActivator)} ({NWScript.GetPCPlayerName(oActivator)}) dans la zone {NWScript.GetName(NWScript.GetArea(oActivator))} sur {NWScript.GetName(oTarget)}");

      return 0;
    }
    private static int HandleBlueprintActivate(uint oItem, uint oActivator, uint oTarget)
    {
      PlayerSystem.Player player;
      if (PlayerSystem.Players.TryGetValue(oActivator, out player))
      {
        Blueprint blueprint = InitializeBlueprint(oItem);

        if(blueprint.type != BlueprintType.Invalid)
        {
          if (oTarget == NWScript.OBJECT_INVALID)
            blueprint.DisplayBlueprintInfo(player, oItem);
          else
          {
            if (player.craftJob.CanStartJob(oActivator, oItem))
            {
              if (NWScript.GetTag(oTarget) == blueprint.craftedItemTag)
              {
                if (NWScript.GetNearestObjectByTag(blueprint.workshopTag, oActivator) != NWScript.OBJECT_INVALID)
                {
                  string sMaterial = blueprint.GetMaterialFromTargetItem(oTarget);
                  CollectSystem.MineralType mineralType = CollectSystem.GetMineralTypeFromName(sMaterial);

                  if (mineralType == CollectSystem.MineralType.Invalid)
                    player.craftJob.Start(CraftJob.JobType.Item, blueprint, player, oItem, oTarget, sMaterial, mineralType);
                  else
                    NWScript.SendMessageToPC(oActivator, "Cet objet ne peut pas être amélioré.");
                }
                else
                  NWScript.SendMessageToPC(oActivator, $"Vous devez être à proximité d'un atelier de type {blueprint.workshopTag} pour commencer ce travail");
              }
              else
                NWScript.SendMessageToPC(oActivator, "Ce patron ne permet pas d'améliorer ce type d'objet");
            }
          }
        }
        else
        {
          NWScript.SendMessageToPC(oActivator, "[ERREUR HRP] - Ce patron n'est pas correctement initialisé. Le staff a été informé.");
          Utils.LogMessageToDMs($"Invalid blueprint : {NWScript.GetName(oItem)} used by {NWScript.GetName(oActivator)}");
        }
      }

      return 0;
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
