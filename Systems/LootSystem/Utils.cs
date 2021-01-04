using System;
using System.Collections.Generic;
using NWN.Core;
using static NWN.Systems.PlayerSystem;

namespace NWN.Systems
{
  public static partial class LootSystem
  {
    private static Dictionary<string, List<uint>> chestTagToLootsDic = new Dictionary<string, List<uint>> { };
    private static void UpdateDB(uint oChest)
    {
      Player oPC;
      if (Players.TryGetValue(NWScript.GetLastClosedBy(), out oPC))
      {
        var query = NWScript.SqlPrepareQueryCampaign(ModuleSystem.database, $"INSERT INTO {SQL_TABLE}(chestTag, accountId, serializedChest, position, facing)" +
      " VALUES(@chestTag, @accountId, @serializedChest, @position, @facing)" +
      " ON CONFLICT(chestTag) DO UPDATE SET serializedChest = @serializedChest, position = @position, facing = @facing;");
        NWScript.SqlBindString(query, "@chestTag", NWScript.GetTag(oChest));
        NWScript.SqlBindInt(query, "@accountId", oPC.accountId);
        NWScript.SqlBindObject(query, "@serializedChest", oChest);
        NWScript.SqlBindVector(query, "@position", NWScript.GetPosition(oChest));
        NWScript.SqlBindFloat(query, "@facing", NWScript.GetFacing(oChest));
        NWScript.SqlStep(query);
      }
    }
    private static List<uint> GetPlaceables(uint oArea)
    {
      var oPlaceable = NWScript.GetFirstObjectInArea(oArea);
      var list = new List<uint> { };

      while (NWScript.GetIsObjectValid(oPlaceable) == 1)
      {
        if (NWScript.GetObjectType(oPlaceable) == NWScript.OBJECT_TYPE_PLACEABLE &&
            NWScript.GetHasInventory(oPlaceable) == 1)
        {
          list.Add(oPlaceable);
        }

        oPlaceable = NWScript.GetNextObjectInArea(oArea);
      }

      return list;
    }
    private static void UpdateChestTagToLootsDic(uint oChest)
    {
      var tag = NWScript.GetTag(oChest);
      var loots = new List<uint> { };

      var oLoot = NWScript.GetFirstItemInInventory(oChest);

      while (NWScript.GetIsObjectValid(oLoot) == 1)
      {
        loots.Add(oLoot);
        oLoot = NWScript.GetNextItemInInventory(oChest);
      }

      chestTagToLootsDic[tag] = loots;
    }
    private static void ThrowException(string message)
    {
      throw new ApplicationException($"LootSystem: {message}");
    }
  }
}
