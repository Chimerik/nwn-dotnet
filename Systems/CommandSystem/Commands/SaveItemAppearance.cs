﻿using System.Collections.Generic;
using Anvil.API;
using Anvil.API.Events;

namespace NWN.Systems
{
  class SaveItemAppearance
  {
    public SaveItemAppearance(NwPlayer oPC)
    {
      oPC.SendServerMessage("Veuillez sélectionnner l'objet dont vous souhaitez sauvegarder l'apparence.", ColorConstants.Rose);
      PlayerSystem.cursorTargetService.EnterTargetMode(oPC, OnAppearanceSelected, ObjectTypes.Item, MouseCursor.Create);
    }
    private static async void OnAppearanceSelected(ModuleEvents.OnPlayerTarget selection)
    {
      if (selection.IsCancelled || selection.TargetObject is null || !(selection.TargetObject is NwItem item) || !PlayerSystem.Players.TryGetValue(selection.Player.LoginCreature, out PlayerSystem.Player player))
        return;

      // TODO : ajouter un métier permettant de modifier n'importe quelle tenue
      if (item.GetObjectVariable<LocalVariableString>("_ORIGINAL_CRAFTER_NAME").HasValue && item.GetObjectVariable<LocalVariableString>("_ORIGINAL_CRAFTER_NAME").Value != player.oid.LoginCreature.Name)
      {
        player.oid.SendServerMessage($"Impossible de sauvegarder cette apparence. Il est indiqué : Pour tout modification, s'adresser à {item.GetObjectVariable<LocalVariableString>("_ORIGINAL_CRAFTER_NAME").Value.ColorString(ColorConstants.White)}", ColorConstants.Orange);
        return;
      }

      int ACValue = -1;
      if (item.BaseItemType == BaseItemType.Armor)
        ACValue = item.BaseACValue;

      player.menu.Clear();

      player.menu.titleLines = new List<string>() {
        "Veuillez indiquer le nom sous lequel vous souhaitez sauvegarder l'apparence de cet objet."
      };

      player.menu.Draw();

      bool awaitedValue = await player.WaitForPlayerInputString();

      if (awaitedValue)
      {
        string input = player.oid.LoginCreature.GetObjectVariable<LocalVariableString>("_PLAYER_INPUT").Value;
        player.menu.Close();
        player.oid.LoginCreature.GetObjectVariable<LocalVariableString>("_PLAYER_INPUT").Delete();

        string serializedAppearance = item.Appearance.Serialize();
        string baseItemType = ((int)item.BaseItemType).ToString();

        bool queryResult = await SqLiteUtils.InsertQueryAsync("playerItemAppearance",
          new List<string[]>() {
            new string[] { "characterId", player.characterId.ToString() },
            new string[] { "appearanceName", input },
            new string[] { "serializedAppearance", serializedAppearance },
            new string[] { "baseItemType", baseItemType },
            new string[] { "AC", ACValue.ToString() }},
          new List<string>() { "characterId", "appearanceName" },
          new List<string[]>() { new string[] { "serializedAppearance" }, new string[] { "baseItemType" }, new string[] { "AC" } },
          new List<string>() { "characterId", "appearanceName" });

        player.HandleAsyncQueryFeedback(queryResult, $"L'apparence de votre {selection.TargetObject.Name.ColorString(ColorConstants.White)} a été sauvegardée sous le nom {input.ColorString(ColorConstants.White)}.", "Erreur technique - L'apparence de votre objet n'a pas été sauvegardée !");
      }
    }
  }
}
