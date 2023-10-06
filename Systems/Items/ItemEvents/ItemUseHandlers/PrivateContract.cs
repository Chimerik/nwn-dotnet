using Anvil.API;
using Anvil.API.Events;
using Anvil.Services;

namespace NWN.Systems
{
  public partial class ItemSystem
  {
    private static void HandlePrivateContract(OnItemUse onUse)
    {
      if (onUse.TargetObject is null || onUse.TargetObject == onUse.UsedBy || !PlayerSystem.Players.TryGetValue(onUse.UsedBy, out PlayerSystem.Player player))
        return;

      feedbackService.AddFeedbackMessageFilter(FeedbackMessage.UseItemCantUse, onUse.UsedBy.ControllingPlayer);
      onUse.PreventUseItem = true;

      if (!player.windows.ContainsKey("resourceExchange")) player.windows.Add("resourceExchange", new PlayerSystem.Player.ResourceExchangeWindow(player, onUse.TargetObject));
      else ((PlayerSystem.Player.ResourceExchangeWindow)player.windows["resourceExchange"]).CreateOwnerWindow(onUse.TargetObject);
    }  
  }
}
