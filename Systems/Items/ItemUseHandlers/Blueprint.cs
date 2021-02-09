using NWN.API;
using NWN.Core;
using NWN.API.Constants;
using System.Linq;

namespace NWN.Systems.Items.ItemUseHandlers
{
    public static class Blueprint
    {
        public static void HandleActivate(NwItem oBlueprint, NwPlayer oPC, uint oTarget)
        {
            int baseItemType = oBlueprint.GetLocalVariable<int>("_BASE_ITEM_TYPE").Value;

            if (Craft.Collect.System.blueprintDictionnary.TryGetValue(baseItemType, out Craft.Blueprint blueprint))
            {
                if (oTarget == NWScript.OBJECT_INVALID)
                    oPC.SendServerMessage(blueprint.DisplayBlueprintInfo(oPC, oBlueprint));
                else
                {
                    if (PlayerSystem.Players.TryGetValue(oPC, out PlayerSystem.Player player))
                    {
                        if (player.craftJob.CanStartJob(oPC, oBlueprint, Craft.Job.JobType.Item))
                        {
                            if ((int)oPC.GetItemInSlot(InventorySlot.RightHand).BaseItemType == 114) // 114 = marteau de forgeron
                            {
                                NwPlaceable forge = oPC.Area.FindObjectsOfTypeInArea<NwPlaceable>().Where(f => f.Tag == blueprint.workshopTag && f.Distance(oPC) < 5).FirstOrDefault();

                                if (forge.IsValid)
                                {
                                    string sMaterial = blueprint.GetMaterialFromTargetItem(oTarget);

                                    if (sMaterial != "Invalid")
                                        player.craftJob.Start(Craft.Job.JobType.Item, blueprint, player, oBlueprint, oTarget, sMaterial);
                                    else
                                        oPC.SendServerMessage("Ce patron ne permet pas d'améliorer cet objet.");
                                }
                                else
                                    oPC.SendServerMessage($"Vous devez être à proximité d'un atelier de type {blueprint.workshopTag} pour commencer ce travail");
                            }
                            else
                                oPC.SendServerMessage($"Vous devez avoir un marteau d'artisan en main pour commencer le travail.");
                        }
                    }
                }
            }
            else
            {
                oPC.SendServerMessage("[ERREUR HRP] - Ce patron n'est pas correctement initialisé. Le bug a été remonté au staff.");
                NWN.Utils.LogMessageToDMs($"Invalid blueprint : {oBlueprint.Name} - Base Item Type : {baseItemType} - Used by {oPC.Name}");
            }
        }
    }
}
