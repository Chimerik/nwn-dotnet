using Anvil.API.Events;
using Anvil.API;
using Anvil.Services;

namespace NWN.Systems
{
  public partial class ItemSystem
  {
    public static void HandleUnacquirableItems(ModuleEvents.OnAcquireItem onAcquireItem)
    {
      NwGameObject oAcquiredFrom = onAcquireItem.AcquiredFrom;
      NwItem oItem = onAcquireItem.Item;

      if (onAcquireItem.AcquiredBy is null || onAcquireItem.AcquiredBy is not NwCreature oPC 
        || oPC.ControllingPlayer is null || oItem is null || oItem.Tag != "undroppable_item")
        return;

      feedbackService.AddFeedbackMessageFilter(FeedbackMessage.ItemLost, oPC.ControllingPlayer);
      oItem.Clone(oAcquiredFrom);
      oItem.Destroy();
      feedbackService.RemoveFeedbackMessageFilter(FeedbackMessage.ItemLost, oPC.ControllingPlayer);
    }
  }
}
