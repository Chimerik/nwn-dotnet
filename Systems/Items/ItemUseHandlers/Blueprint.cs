using Anvil.API;
using System.Linq;

namespace NWN.Systems.Items.ItemUseHandlers
{
  public static class Blueprint
  {
    public static void HandleActivate(NwItem oBlueprint, NwCreature oPC, NwGameObject oTarget)
    {
      int baseItemType = oBlueprint.GetObjectVariable<LocalVariableInt>("_BASE_ITEM_TYPE").Value;

      if (!Craft.Collect.System.blueprintDictionnary.TryGetValue(baseItemType, out Craft.Blueprint blueprint))
      {
        oPC.ControllingPlayer.SendServerMessage("[ERREUR HRP] - Ce patron n'est pas correctement initialisé. Le bug a été remonté au staff.", ColorConstants.Red);
        Utils.LogMessageToDMs($"Invalid blueprint : {oBlueprint.Name} - Base Item Type : {baseItemType} - Used by {oPC.Name}");
        return;
      }

      if (oTarget == null)
      {
        oPC.ControllingPlayer.SendServerMessage(blueprint.DisplayBlueprintInfo(oPC.ControllingPlayer, oBlueprint));
        return;
      }

      if (!PlayerSystem.Players.TryGetValue(oPC, out PlayerSystem.Player player))
        return;

      if (oPC.GetItemInSlot(InventorySlot.RightHand) == null || (int)oPC.GetItemInSlot(InventorySlot.RightHand).BaseItem.ItemType != 114) // 114 = marteau de forgeron
      {
        oPC.ControllingPlayer.SendServerMessage($"Vous devez avoir un marteau d'artisan en main pour commencer le travail.", ColorConstants.Orange);
        return;
      }

      if (oPC.GetNearestObjectsByType<NwPlaceable>().Any(f => f.Tag == blueprint.workshopTag && f.Distance(oPC) >= 5))
      {
        oPC.ControllingPlayer.SendServerMessage($"Vous devez être à proximité d'un atelier de type {blueprint.workshopTag} pour commencer ce travail", ColorConstants.Orange);
        return;
      }

      if (!player.craftJob.CanStartJob(oPC.ControllingPlayer, oBlueprint, Craft.Job.JobType.Item))
        return;

      string sMaterial = blueprint.GetMaterialFromTargetItem(oTarget);
      if (sMaterial == "Invalid")
      {
        oPC.ControllingPlayer.SendServerMessage("Ce patron ne permet pas d'améliorer cet objet.", ColorConstants.Red);
        return;
      }

      if (oTarget.GetObjectVariable<LocalVariableInt>("_DURABILITY").Value > 0 || oTarget is NwPlaceable)
        player.craftJob.Start(Craft.Job.JobType.Item, blueprint, player, oBlueprint, oTarget, sMaterial);
      else
        player.craftJob.Start(Craft.Job.JobType.Repair, blueprint, player, oBlueprint, oTarget, sMaterial);
    }
  }
}
