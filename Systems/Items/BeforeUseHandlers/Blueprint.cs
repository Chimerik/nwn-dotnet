using System;
using NWN.Core;
using static NWN.Systems.PlayerSystem;

namespace NWN.Systems.Items.BeforeUseHandlers
{
  public static class BluePrint
  {
    public static void HandleActivate(uint oItem, Player player, uint oTarget)
    {
      int baseItemType = NWScript.GetLocalInt(oItem, "_BASE_ITEM_TYPE");

      if (CollectSystem.blueprintDictionnary.ContainsKey(baseItemType))
      {
        Blueprint blueprint = CollectSystem.blueprintDictionnary[baseItemType];

        if (oTarget == NWScript.OBJECT_INVALID)
          NWScript.SendMessageToPC(player.oid, blueprint.DisplayBlueprintInfo(player, oItem));
        else
        {
          if (player.craftJob.CanStartJob(player.oid, oItem, CraftJob.JobType.Item))
          {
            if (NWScript.GetBaseItemType(NWScript.GetItemInSlot(NWScript.INVENTORY_SLOT_RIGHTHAND, player.oid)) == 114) // 114 = marteau de forgeron
            {
              uint forge = NWScript.GetNearestObjectByTag(blueprint.workshopTag, player.oid);

              if (Convert.ToBoolean(NWScript.GetIsObjectValid(forge)) && NWScript.GetDistanceBetween(player.oid, forge) < 5)
              {
                string sMaterial = blueprint.GetMaterialFromTargetItem(oTarget);

                CollectSystem.MineralType mineralType = CollectSystem.GetMineralTypeFromName(sMaterial);

                if (mineralType != CollectSystem.MineralType.Invalid)
                  player.craftJob.Start(CraftJob.JobType.Item, blueprint, player, oItem, oTarget, sMaterial, mineralType);
                else
                  NWScript.SendMessageToPC(player.oid, "Ce patron ne permet pas d'améliorer cet objet.");
              }
              else
                NWScript.SendMessageToPC(player.oid, $"Vous devez être à proximité d'un atelier de type {blueprint.workshopTag} pour commencer ce travail");
            }
            else
              NWScript.SendMessageToPC(player.oid, $"Vous devez avoir un marteau de forgeron en main pour commencer le travail.");
          }
        }
      }
      else
      {
        NWScript.SendMessageToPC(player.oid, "[ERREUR HRP] - Ce patron n'est pas correctement initialisé. Le bug a été remonté au staff.");
        NWN.Utils.LogMessageToDMs($"Invalid blueprint : {NWScript.GetName(oItem)} - Base Item Type : {baseItemType} - Used by {NWScript.GetName(player.oid)}");
      }
    }
  }
}
