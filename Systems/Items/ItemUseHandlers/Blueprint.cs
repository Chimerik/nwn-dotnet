using NWN.API;
using NWN.Core;
using NWN.API.Constants;
using System.Linq;

namespace NWN.Systems.Items.ItemUseHandlers
{
    public static class Blueprint
    {
        public static void HandleActivate(NwItem oBlueprint, NwPlayer oPC, NwGameObject oTarget)
        {
            int baseItemType = oBlueprint.GetLocalVariable<int>("_BASE_ITEM_TYPE").Value;

            if (Craft.Collect.System.blueprintDictionnary.TryGetValue(baseItemType, out Craft.Blueprint blueprint))
            {
                if (oTarget == null)
                    oPC.SendServerMessage(blueprint.DisplayBlueprintInfo(oPC, oBlueprint));
                else
                {
                    if (PlayerSystem.Players.TryGetValue(oPC, out PlayerSystem.Player player))
                    {
                        if (player.craftJob.CanStartJob(oPC, oBlueprint, Craft.Job.JobType.Item))
                        {
                            if (oPC.GetItemInSlot(InventorySlot.RightHand) != null && (int)oPC.GetItemInSlot(InventorySlot.RightHand).BaseItemType == 114) // 114 = marteau de forgeron
                            {
                                if (oPC.GetNearestObjectsByType<NwPlaceable>().Any(f => f.Tag == blueprint.workshopTag && f.Distance(oPC) < 5))
                                {
                                    string sMaterial = blueprint.GetMaterialFromTargetItem(oTarget);

                                    if (sMaterial != "Invalid")
                                        player.craftJob.Start(Craft.Job.JobType.Item, blueprint, player, oBlueprint, oTarget, sMaterial);
                                    else
                                        oPC.SendServerMessage("Ce patron ne permet pas d'améliorer cet objet.", Color.RED);
                                }
                                else
                                    oPC.SendServerMessage($"Vous devez être à proximité d'un atelier de type {blueprint.workshopTag} pour commencer ce travail", Color.ORANGE);
                            }
                            else
                                oPC.SendServerMessage($"Vous devez avoir un marteau d'artisan en main pour commencer le travail.", Color.ORANGE);
                        }
                    }
                }
            }
            else
            {
                oPC.SendServerMessage("[ERREUR HRP] - Ce patron n'est pas correctement initialisé. Le bug a été remonté au staff.", Color.RED);
                Utils.LogMessageToDMs($"Invalid blueprint : {oBlueprint.Name} - Base Item Type : {baseItemType} - Used by {oPC.Name}");
            }
        }
    }
}
