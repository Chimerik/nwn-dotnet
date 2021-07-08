using System;
using System.Collections.Generic;
using NWN.API;
using NWN.Core;

namespace NWN.Systems
{
  public partial class LootSystem
  {
    private void UpdateDB(NwPlaceable oChest, NwCreature oClosedBy)
    {
      if (PlayerSystem.Players.TryGetValue(oClosedBy, out PlayerSystem.Player oPC))
      {
        SqLiteUtils.InsertQuery(SQL_TABLE,
          new List<string[]>() {
            new string[] { "chestTag", oChest.Tag },
            new string[] { "accountId", oPC.accountId.ToString() },
            new string[] { "serializedChest", oChest.Serialize().ToBase64EncodedString() },
          new string[] { "position", oChest.Position.ToString() },
          new string[] { "facing", oChest.Rotation.ToString() } },
          new List<string>() { "chestTag" },
          new List<string[]>() { new string[] { "serializedChest" }, new string[] { "position" }, new string[] { "facing" } },
          new List<string>() { "characterId", "grimoireName" });
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
