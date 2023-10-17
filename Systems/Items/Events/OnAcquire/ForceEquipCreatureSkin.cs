using Anvil.API;
using Anvil.API.Events;
using Anvil.Services;

namespace NWN.Systems
{
  public partial class ItemSystem
  {
    public static void OnUnEquipForceEquipCreatureSkin(OnItemUnequip onUnequip)
    {
      if (onUnequip.Creature is null || onUnequip.Item is null || onUnequip.Item.Tag != "Peaudejoueur")
        return;

      feedbackService.AddFeedbackMessageFilter(FeedbackMessage.UseItemCantUse, onUnequip.Creature.ControllingPlayer);
      if (!onUnequip.Creature.RunEquip(onUnequip.Item, InventorySlot.CreatureSkin))
        LogUtils.LogMessage($"WARNING - {onUnequip.Creature.Name} could not equip Creature Skin", LogUtils.LogType.PlayerConnections);
    }
    public static void OnAcquireForceEquipCreatureSkin(ModuleEvents.OnAcquireItem onAcquire)
    {
      if (onAcquire.AcquiredBy is null || onAcquire.Item is null || onAcquire.AcquiredBy is not NwCreature creature || onAcquire.Item.Tag != "Peaudejoueur")
        return;

      feedbackService.AddFeedbackMessageFilter(FeedbackMessage.UseItemCantUse, creature.ControllingPlayer);
      if (!creature.RunEquip(onAcquire.Item, InventorySlot.CreatureSkin))
        LogUtils.LogMessage($"WARNING - {creature.Name} could not equip Creature Skin", LogUtils.LogType.PlayerConnections);
    }
  }
}
