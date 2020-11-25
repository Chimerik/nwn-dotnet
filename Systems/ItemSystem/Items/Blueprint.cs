using NWN.Core;
using static NWN.Systems.PlayerSystem;

namespace NWN.Systems
{
  public static partial class ItemSystem
  {
    private static void HandleBlueprintActivate(uint oItem, Player player, uint oTarget)
    {
      int baseItemType = NWScript.GetLocalInt(oItem, "_BASE_ITEM_TYPE");

      if (CollectSystem.blueprintDictionnary.ContainsKey(baseItemType))
      {
        Blueprint blueprint = CollectSystem.blueprintDictionnary[baseItemType];

        if (oTarget == NWScript.OBJECT_INVALID)
          NWScript.SendMessageToPC(player.oid, blueprint.DisplayBlueprintInfo(player, oItem));
        else
        {
          if (player.craftJob.CanStartJob(player.oid, oItem))
          {
            if (NWScript.GetTag(oTarget) == blueprint.craftedItemTag)
            {
              if (NWScript.GetNearestObjectByTag(blueprint.workshopTag, player.oid) != NWScript.OBJECT_INVALID)
              {
                string sMaterial = blueprint.GetMaterialFromTargetItem(oTarget);
                CollectSystem.MineralType mineralType = CollectSystem.GetMineralTypeFromName(sMaterial);

                if (mineralType == CollectSystem.MineralType.Invalid)
                  player.craftJob.Start(CraftJob.JobType.Item, blueprint, player, oItem, oTarget, sMaterial, mineralType);
                else
                  NWScript.SendMessageToPC(player.oid, "Cet objet ne peut pas être amélioré.");
              }
              else
                NWScript.SendMessageToPC(player.oid, $"Vous devez être à proximité d'un atelier de type {blueprint.workshopTag} pour commencer ce travail");
            }
            else
              NWScript.SendMessageToPC(player.oid, "Ce patron ne permet pas d'améliorer ce type d'objet");
          }
        }
      }
      else
      {
        NWScript.SendMessageToPC(player.oid, "[ERREUR HRP] - Ce patron n'est pas correctement initialisé. Le bug a été remonté au staff.");
        Utils.LogMessageToDMs($"Invalid blueprint : {NWScript.GetName(oItem)} - Base Item Type : {baseItemType} - Used by {NWScript.GetName(player.oid)}");
      }
    }
  }
}
