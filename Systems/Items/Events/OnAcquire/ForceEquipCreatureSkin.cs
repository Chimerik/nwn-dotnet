using Anvil.API;
using Anvil.API.Events;
using Anvil.Services;

namespace NWN.Systems
{
  public partial class ItemSystem
  {
    public static void OnUnEquipForceEquipCreatureSkin(OnItemUnequip onUnequip)
    {
      ModuleSystem.Log.Info($"unequip : {onUnequip.Item.Name} - {onUnequip.Item.Tag} - {onUnequip.Item.ResRef}");

      if (onUnequip.Creature is null || onUnequip.Item is null || onUnequip.Item.Tag != "Peaudejoueur")
        return;

      onUnequip.Item.Destroy();

      /*feedbackService.AddFeedbackMessageFilter(FeedbackMessage.UseItemCantUse, onUnequip.Creature.ControllingPlayer);
      if (!onUnequip.Creature.RunEquip(onUnequip.Item, InventorySlot.CreatureSkin))
        LogUtils.LogMessage($"WARNING - {onUnequip.Creature.Name} could not equip Creature Skin", LogUtils.LogType.PlayerConnections);*/
    }
    public static async void OnAcquireForceEquipCreatureSkin(ModuleEvents.OnAcquireItem onAcquire)
    {
      ModuleSystem.Log.Info($"acquired : {onAcquire.Item.Name} - {onAcquire.Item.Tag} - {onAcquire.Item.ResRef}");

      if (onAcquire.Item is not null && onAcquire.Item.Tag == "x3_it_pchide" && onAcquire.AcquiredBy is NwCreature creature)
      {
        feedbackService.AddFeedbackMessageFilter(FeedbackMessage.ItemLost, creature.ControllingPlayer);
        onAcquire.Item.Destroy();

        await NwTask.NextFrame();
        feedbackService.RemoveFeedbackMessageFilter(FeedbackMessage.ItemLost, creature.ControllingPlayer);
      }
    }
  }
}
