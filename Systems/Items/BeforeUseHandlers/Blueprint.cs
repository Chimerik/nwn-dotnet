using System;
using NWN.Core;
using static NWN.Systems.PlayerSystem;
using static NWN.Systems.Craft.Collect.System;
using NWN.Systems.Craft;

namespace NWN.Systems.Items.BeforeUseHandlers
{
  public static class BluePrint
  {
    public static void HandleActivate(uint oItem, Player player, uint oTarget)
    {
      int baseItemType = NWScript.GetLocalInt(oItem, "_BASE_ITEM_TYPE");

      if (blueprintDictionnary.TryGetValue(baseItemType, out Blueprint blueprint))
      {
        if (oTarget == NWScript.OBJECT_INVALID)
          NWScript.SendMessageToPC(player.oid, blueprint.DisplayBlueprintInfo(player, oItem));
        else
        {
          if (player.craftJob.CanStartJob(player.oid, oItem, Job.JobType.Item))
          {
            if (NWScript.GetBaseItemType(NWScript.GetItemInSlot(NWScript.INVENTORY_SLOT_RIGHTHAND, player.oid)) == 114) // 114 = marteau de forgeron
            {
              uint forge = NWScript.GetNearestObjectByTag(blueprint.workshopTag, player.oid);

              if (Convert.ToBoolean(NWScript.GetIsObjectValid(forge)) && NWScript.GetDistanceBetween(player.oid, forge) < 5)
              {
                string sMaterial = blueprint.GetMaterialFromTargetItem(oTarget);

                if (sMaterial != "Invalid")
                  player.craftJob.Start(Job.JobType.Item, blueprint, player, oItem, oTarget, sMaterial);
                else
                  NWScript.SendMessageToPC(player.oid, "Ce patron ne permet pas d'améliorer cet objet.");
              }
              else
                NWScript.SendMessageToPC(player.oid, $"Vous devez être à proximité d'un atelier de type {blueprint.workshopTag} pour commencer ce travail");
            }
            else
              NWScript.SendMessageToPC(player.oid, $"Vous devez avoir un marteau d'artisan en main pour commencer le travail.");
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
