using Anvil.API;
using Anvil.API.Events;
using Anvil.Services;

namespace NWN.Systems
{
  public partial class ItemSystem
  {
    private static void HandleOnTargetMineableMateria(OnItemUse onUse)
    {
      if (!PlayerSystem.Players.TryGetValue(onUse.UsedBy, out PlayerSystem.Player player))
        return;

      if (!player.learnableSkills.ContainsKey(CustomSkill.InfluxExtraction))
      {
        player.oid.SendServerMessage("La base de la compétence d'extraction de dépot de matéria doit être apprise avant de pouvoir utiliser cet objet", ColorConstants.Red);
        return;
      }

      for (int i = 0; i < onUse.Item.GetObjectVariable<LocalVariableInt>("TOTAL_SLOTS").Value; i++)
      {
        if (onUse.Item.GetObjectVariable<LocalVariableInt>($"SLOT{i}").HasNothing)
          continue;

        int inscriptionId = onUse.Item.GetObjectVariable<LocalVariableInt>($"SLOT{i}");

        if (inscriptionId >= CustomInscription.MateriaExtractionDurabilityMinor && inscriptionId <= CustomInscription.MateriaExtractionSpeedSupreme)
        {
          feedbackService.AddFeedbackMessageFilter(FeedbackMessage.UseItemCantUse, onUse.UsedBy.ControllingPlayer);
          feedbackService.AddFeedbackMessageFilter(FeedbackMessage.UseItemNotEquipped, onUse.UsedBy.ControllingPlayer);
          onUse.PreventUseItem = true;
          if (!player.windows.ContainsKey("materiaExtraction")) player.windows.Add("materiaExtraction", new PlayerSystem.Player.MateriaExtractionWindow(player, onUse.Item, onUse.TargetObject));
          else ((PlayerSystem.Player.MateriaExtractionWindow)player.windows["materiaExtraction"]).CreateWindow(onUse.Item, onUse.TargetObject);
          break;
        }
      }

      player.oid.SendServerMessage("Votre outil ne dispose d'aucune inscription permettant de procéder à une extraction de matéria. Pensez à faire appliquer une nouvelle inscription d'extraction !", ColorConstants.Red);
    }
  }
}
