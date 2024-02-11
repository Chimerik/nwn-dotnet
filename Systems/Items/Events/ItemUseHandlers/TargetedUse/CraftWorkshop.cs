using Anvil.API;
using Anvil.API.Events;
using Anvil.Services;

namespace NWN.Systems
{
  public partial class ItemSystem
  {
    private static void HandleOnTargetCraftWorkshop(OnItemUse onUse)
    {
      if (!PlayerSystem.Players.TryGetValue(onUse.UsedBy, out PlayerSystem.Player player))
        return;

      for (int i = 0; i < onUse.Item.GetObjectVariable<LocalVariableInt>("TOTAL_SLOTS").Value; i++)
      {
        if (onUse.Item.GetObjectVariable<LocalVariableInt>($"SLOT{i}").HasNothing)
          continue;

        int inscriptionId = onUse.Item.GetObjectVariable<LocalVariableInt>($"SLOT{i}");

        if (inscriptionId >= CustomInscription.MateriaProductionDurabilityMinor && inscriptionId <= CustomInscription.MateriaProductionQualitySupreme)
        {
          feedbackService.AddFeedbackMessageFilter(FeedbackMessage.UseItemCantUse, onUse.UsedBy.ControllingPlayer);
          feedbackService.AddFeedbackMessageFilter(FeedbackMessage.UseItemNotEquipped, onUse.UsedBy.ControllingPlayer);
          onUse.PreventUseItem = true;

          if (!player.windows.TryGetValue("craftWorkshop", out var value)) player.windows.Add("craftWorkshop", new PlayerSystem.Player.WorkshopWindow(player, onUse.TargetObject.Tag, onUse.Item));
          else ((PlayerSystem.Player.WorkshopWindow)value).CreateWindow(onUse.TargetObject.Tag, onUse.Item);

          break;
        }
      }

      player.oid.SendServerMessage("Votre outil ne dispose d'aucune inscription permettant de manipuler de la matéria pour une production artisanale. Pensez à faire appliquer une nouvelle inscription de production !", ColorConstants.Red);
    }
  }
}
