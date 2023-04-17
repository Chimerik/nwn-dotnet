using System;
using System.Collections.Generic;
using Anvil.API;

namespace NWN.Systems
{
  public partial class LootSystem
  {
    private static async void UpdateDB(NwPlaceable oChest, NwCreature oClosedBy)
    {
      if (PlayerSystem.Players.TryGetValue(oClosedBy, out PlayerSystem.Player oPC))
      {
        string tag = oChest.Tag;
        string accountId = oPC.accountId.ToString();
        string serializedChest = oChest.Serialize().ToBase64EncodedString();
        string position = oChest.Position.ToString();
        string facing = oChest.Rotation.ToString();

        bool queryResult = await SqLiteUtils.InsertQueryAsync(SQL_TABLE,
          new List<string[]>() {
            new string[] { "chestTag", tag },
            new string[] { "accountId", accountId },
            new string[] { "serializedChest", serializedChest },
          new string[] { "position", position },
          new string[] { "facing", facing } },
          new List<string>() { "chestTag" },
          new List<string[]>() { new string[] { "serializedChest" }, new string[] { "position" }, new string[] { "facing" } },
          new List<string>() { "characterId", "grimoireName" });

        oPC.HandleAsyncQueryFeedback(queryResult, $"Coffre {tag} correctement sauvegardé.", "Erreur technique - Le coffre n'a pas pu être sauvegardé.");
      }
    }
    private void UpdateChestTagToLootsDic(NwPlaceable oChest)
    {
      var loots = new List<NwItem> { };

      foreach (NwItem item in oChest.Inventory.Items)
      {
        loots.Add(item);
      }
      chestTagToLootsDic[oChest.Tag] = loots;
    }
    private static void ThrowException(string message)
    {
      throw new ApplicationException($"LootSystem: {message}");
    }
  }
}
