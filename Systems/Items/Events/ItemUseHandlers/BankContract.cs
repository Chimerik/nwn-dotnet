using Anvil.API.Events;
using Anvil.Services;

namespace NWN.Systems
{
  public partial class ItemSystem
  {
    private static void HandleBankContract(OnItemUse onUse)
    {
      if (!PlayerSystem.Players.TryGetValue(onUse.UsedBy, out PlayerSystem.Player player))
        return;

      feedbackService.AddFeedbackMessageFilter(FeedbackMessage.UseItemCantUse, onUse.UsedBy.ControllingPlayer);
      feedbackService.AddFeedbackMessageFilter(FeedbackMessage.UseItemCantUse, onUse.UsedBy.ControllingPlayer);
      onUse.PreventUseItem = true;

      if (!player.windows.ContainsKey("bankContract")) player.windows.Add("bankContract", new PlayerSystem.Player.BankContractWindow(player, onUse.Item));
      else ((PlayerSystem.Player.BankContractWindow)player.windows["bankContract"]).CreateWindow();
    }
  }
}
