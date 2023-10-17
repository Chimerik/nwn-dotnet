using Anvil.API.Events;
using Anvil.API;
using Anvil.Services;

namespace NWN.Systems
{
  public partial class ItemSystem
  {
    public static void MergeStackableItem(ModuleEvents.OnAcquireItem onAcquireItem)
    {
      NwItem oItem = onAcquireItem.Item;

      if (onAcquireItem.AcquiredBy is null || onAcquireItem.AcquiredBy is not NwCreature oPC ||
        oPC.ControllingPlayer == null || oItem == null || !oItem.BaseItem.IsStackable)
        return;

      feedbackService.AddFeedbackMessageFilter(FeedbackMessage.ItemReceived, oPC.ControllingPlayer);

      foreach (var inventoryItem in oPC.Inventory.Items)
      {
        bool sameItem = true;

        if (oItem != inventoryItem && inventoryItem.BaseItem.IsStackable && oItem.BaseItem.ItemType == inventoryItem.BaseItem.ItemType && oItem.Tag == inventoryItem.Tag && oItem.Name == inventoryItem.Name
          && oItem.StackSize + inventoryItem.StackSize <= oItem.BaseItem.MaxStackSize)
        {
          foreach (var localVar in oItem.LocalVariables)
          {
            switch (localVar)
            {
              case LocalVariableString stringVar:
                if (stringVar.Value != inventoryItem.GetObjectVariable<LocalVariableString>(stringVar.Name).Value)
                  sameItem = false;
                break;
              case LocalVariableInt intVar:
                if (intVar.Value != inventoryItem.GetObjectVariable<LocalVariableInt>(intVar.Name).Value)
                  sameItem = false;
                break;
              case LocalVariableFloat floatVar:
                if (floatVar.Value != inventoryItem.GetObjectVariable<LocalVariableFloat>(floatVar.Name).Value)
                  sameItem = false;
                break;
              case DateTimeLocalVariable dateVar:
                if (dateVar.Value != inventoryItem.GetObjectVariable<DateTimeLocalVariable>(dateVar.Name).Value)
                  sameItem = false;
                break;
            }

            if (!sameItem)
              break;
          }

          if (!sameItem)
            continue;

          inventoryItem.StackSize += oItem.StackSize;
          oItem.Destroy();
          break;
        }
      }

      feedbackService.RemoveFeedbackMessageFilter(FeedbackMessage.ItemReceived, oPC.ControllingPlayer);
    }
  }
}
