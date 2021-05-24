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
      oPC.SendServerMessage("Veuillez sélectionnner l'objet dont vous souhaitez modifier l'apparence.", Color.ROSE);
      PlayerSystem.cursorTargetService.EnterTargetMode(oPC, OnModifyAppearanceItemSelected, ObjectTypes.Item, MouseCursor.Create);
    }
    private static void OnModifyAppearanceItemSelected(ModuleEvents.OnPlayerTarget selection)
    {
      if (selection.TargetObject is null || !(selection.TargetObject is NwItem) || !PlayerSystem.Players.TryGetValue(selection.Player.LoginCreature, out PlayerSystem.Player player))
        return;

      NwItem item = (NwItem)selection.TargetObject;

      // TODO : ajouter un métier permettant de modifier n'importe quelle tenue
      if (item.GetLocalVariable<string>("_ORIGINAL_CRAFTER_NAME").HasValue && item.GetLocalVariable<string>("_ORIGINAL_CRAFTER_NAME").Value != player.oid.LoginCreature.Name)
      {
        player.oid.SendServerMessage($"Il est indiqué : Pour tout modification, s'adresser à {item.GetLocalVariable<string>("_ORIGINAL_CRAFTER_NAME").Value.ColorString(Color.WHITE)}", Color.ORANGE);
        return;
      }

      int ACValue = -1;
      if (item.BaseItemType == BaseItemType.Armor)
        ACValue = ItemPlugin.GetBaseArmorClass(selection.TargetObject);

      player.menu.Clear();
      player.menu.titleLines = new List<string>() {
        $"Voici la liste de vos apparences sauvegardées qui peuvent être appliquées sur votre {item.Name.ColorString(Color.WHITE)}".ColorString(Color.NAVY)
      };

      var query = NWScript.SqlPrepareQueryCampaign(Config.database, $"SELECT appearanceName, serializedAppearance from playerItemAppearance where characterId = @characterId and AC = @AC and baseItemType = @baseItemType");
      NWScript.SqlBindInt(query, "@characterId", player.characterId);
      NWScript.SqlBindInt(query, "@AC", ACValue);
      NWScript.SqlBindInt(query, "@baseItemType", (int)item.BaseItemType);

      while (NWScript.SqlStep(query) > 0)
      {
        string message = $"- {NWScript.SqlGetString(query, 0)}".ColorString(Color.CYAN);
        string appearance = NWScript.SqlGetString(query, 1);

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
        player.oid.SendServerMessage($"L'objet dont vous essayez de modifier l'apparence n'existe plus ou n'est plus en votre possession !", Color.RED);
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

      ItemPlugin.RestoreItemAppearance(item, serializedAppearance);
      NwItem newItem = item.Clone(player.oid.ControlledCreature);
      item.Destroy();

      for(int i = 0; i <= (int)InventorySlot.Bolts; i++)
      {
        if (player.oid.ControlledCreature.GetItemInSlot((InventorySlot)i) == item)
        {
          CreaturePlugin.RunEquip(player.oid.ControlledCreature, newItem, i);
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

        player.oid.SendServerMessage($"L'apparence de votre {item.Name.ColorString(Color.WHITE)} a bien été modifiée.", Color.GREEN);
      });
    }
  }
}
