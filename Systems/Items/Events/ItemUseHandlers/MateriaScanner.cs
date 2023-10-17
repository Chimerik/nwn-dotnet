using Anvil.API;
using Anvil.API.Events;
using Anvil.Services;

namespace NWN.Systems
{
  public partial class ItemSystem
  {
    private static void HandleMateriaScanning(OnItemUse onUse)
    {
      if (!PlayerSystem.Players.TryGetValue(onUse.UsedBy, out PlayerSystem.Player player))
        return;

      for (int i = 0; i < onUse.Item.GetObjectVariable<LocalVariableInt>("TOTAL_SLOTS").Value; i++)
      {
        if (onUse.Item.GetObjectVariable<LocalVariableInt>($"SLOT{i}").HasNothing)
          continue;

        int inscriptionId = onUse.Item.GetObjectVariable<LocalVariableInt>($"SLOT{i}");

        if (inscriptionId >= CustomInscription.MateriaDetectionDurabilityMinor && inscriptionId <= CustomInscription.MateriaExtractionDurabilitySupreme)
        {
          if (!player.learnableSkills.ContainsKey(CustomSkill.MateriaScanning))
          {
            player.oid.SendServerMessage("La base de la compétence de recherche de dépot de matéria doit être apprise avant de pouvoir utiliser cet objet", ColorConstants.Red);
            return;
          }

          feedbackService.AddFeedbackMessageFilter(FeedbackMessage.UseItemCantUse, onUse.UsedBy.ControllingPlayer);
          feedbackService.AddFeedbackMessageFilter(FeedbackMessage.UseItemNotEquipped, onUse.UsedBy.ControllingPlayer);
          onUse.PreventUseItem = true;
          if (!player.windows.ContainsKey("materiaDetector")) player.windows.Add("materiaDetector", new PlayerSystem.Player.MateriaDetectorWindow(player, onUse.Item));
          else ((PlayerSystem.Player.MateriaDetectorWindow)player.windows["materiaDetector"]).CreateWindow(onUse.Item);
          break;
        }
      }
    }
  }
}
