using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NWN.API;
using NWN.API.Constants;
using NWN.API.Events;
using NWN.Core;
using NWN.Core.NWNX;
using NWN.Services;

namespace NWN.Systems
{
  class LoadAppearance
  {
    public LoadAppearance(NwPlayer oPC)
    {
      oPC.SendServerMessage("Veuillez sélectionnner l'objet dont vous souhaitez modifier l'apparence.", ColorConstants.Rose);
      PlayerSystem.cursorTargetService.EnterTargetMode(oPC, OnModifyAppearanceItemSelected, ObjectTypes.Item, MouseCursor.Create);
    }
    private static void OnModifyAppearanceItemSelected(ModuleEvents.OnPlayerTarget selection)
    {
      if (selection.IsCancelled || selection.TargetObject is null || !(selection.TargetObject is NwItem item) || !PlayerSystem.Players.TryGetValue(selection.Player.LoginCreature, out PlayerSystem.Player player))
        return;

      // TODO : ajouter un métier permettant de modifier n'importe quelle tenue
      if (item.GetLocalVariable<string>("_ORIGINAL_CRAFTER_NAME").HasValue && item.GetLocalVariable<string>("_ORIGINAL_CRAFTER_NAME").Value != player.oid.LoginCreature.Name)
      {
        player.oid.SendServerMessage($"Il est indiqué : Pour tout modification, s'adresser à {item.GetLocalVariable<string>("_ORIGINAL_CRAFTER_NAME").Value.ColorString(ColorConstants.White)}", ColorConstants.Orange);
        return;
      }

      int ACValue = -1;
      if (item.BaseItemType == BaseItemType.Armor)
        ACValue = item.BaseACValue;

      player.menu.Clear();
      player.menu.titleLines = new List<string>() {
        $"Voici la liste de vos apparences sauvegardées qui peuvent être appliquées sur votre {item.Name.ColorString(ColorConstants.White)}".ColorString(ColorConstants.Navy)
      };

      var query = SqLiteUtils.SelectQuery("playerItemAppearance",
        new List<string>() { { "appearanceName" }, { "serializedAppearance" } },
        new List<string[]>() { new string[] { "characterId", player.characterId.ToString() }, { new string[] { "AC", ACValue.ToString() } }, { new string[] { "baseItemType", ((int)item.BaseItemType).ToString() } } } );

      if(query != null)
      foreach (var itemAppearance in query)
      {
        string message = $"- {itemAppearance.GetString(0)}".ColorString(ColorConstants.Cyan);
        string appearance = itemAppearance.GetString(1);

        player.menu.choices.Add((
          message,
          () => ApplySelectedAppareance(player, item, appearance)
        ));
      }

      player.menu.choices.Add(("Retour.", () => CommandSystem.DrawCommandList(player)));
      player.menu.choices.Add(("Quitter.", () => player.menu.Close()));

      player.menu.Draw();
    }

    private static void ApplySelectedAppareance(PlayerSystem.Player player, NwItem item, string serializedAppearance)
    {
      player.menu.Close();

      if(item == null || item.Possessor != player.oid.ControlledCreature)
      {
        player.oid.SendServerMessage($"L'objet dont vous essayez de modifier l'apparence n'existe plus ou n'est plus en votre possession !", ColorConstants.Red);
        return;
      }

      ItemSystem.feedbackService.AddFeedbackMessageFilter(FeedbackMessage.ItemReceived, player.oid);
      ItemSystem.feedbackService.AddFeedbackMessageFilter(FeedbackMessage.ItemLost, player.oid);
      ItemSystem.feedbackService.AddFeedbackMessageFilter(FeedbackMessage.EquipWeaponSwappedOut, player.oid);
      ItemSystem.feedbackService.AddFeedbackMessageFilter(FeedbackMessage.EquipSkillSpellModifiers, player.oid);
      ItemSystem.feedbackService.AddFeedbackMessageFilter(FeedbackMessage.InventoryFull, player.oid);
      ItemSystem.feedbackService.AddFeedbackMessageFilter(FeedbackMessage.WeightTooEncumberedToRun, player.oid);
      ItemSystem.feedbackService.AddFeedbackMessageFilter(FeedbackMessage.WeightTooEncumberedWalkSlow, player.oid);
      ItemSystem.feedbackService.AddFeedbackMessageFilter(FeedbackMessage.SendMessageToPc, player.oid);

      item.Appearance.Deserialize(serializedAppearance);
      NwItem newItem = item.Clone(player.oid.ControlledCreature);
      item.Destroy();

      for(int i = 0; i <= (int)InventorySlot.Bolts; i++)
      {
        if (player.oid.ControlledCreature.GetItemInSlot((InventorySlot)i) == item)
        {
          player.oid.ControlledCreature.RunEquip(newItem, (InventorySlot)i);
          break;
        }
      }

      player.menu.Close();

      Task waitDestruction = NwTask.Run(async () =>
      {
        await NwTask.Delay(TimeSpan.FromSeconds(0.4));
        ItemSystem.feedbackService.RemoveFeedbackMessageFilter(FeedbackMessage.ItemReceived, player.oid);
        ItemSystem.feedbackService.RemoveFeedbackMessageFilter(FeedbackMessage.ItemLost, player.oid);
        ItemSystem.feedbackService.RemoveFeedbackMessageFilter(FeedbackMessage.EquipWeaponSwappedOut, player.oid);
        ItemSystem.feedbackService.RemoveFeedbackMessageFilter(FeedbackMessage.EquipSkillSpellModifiers, player.oid);
        ItemSystem.feedbackService.RemoveFeedbackMessageFilter(FeedbackMessage.InventoryFull, player.oid);
        ItemSystem.feedbackService.RemoveFeedbackMessageFilter(FeedbackMessage.WeightTooEncumberedToRun, player.oid);
        ItemSystem.feedbackService.RemoveFeedbackMessageFilter(FeedbackMessage.WeightTooEncumberedWalkSlow, player.oid);
        ItemSystem.feedbackService.RemoveFeedbackMessageFilter(FeedbackMessage.SendMessageToPc, player.oid);

        player.oid.SendServerMessage($"L'apparence de votre {item.Name.ColorString(ColorConstants.White)} a bien été modifiée.", ColorConstants.Green);
      });
    }
  }
}
