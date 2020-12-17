using System;
using System.Collections.Generic;
using System.Linq;
using NWN.Core;
using NWN.Core.NWNX;
using static NWN.Systems.PlayerSystem;

namespace NWN.Systems
{
  public static partial class LootSystem
  {
    private static Dictionary<string, List<uint>> chestTagToLootsDic = new Dictionary<string, List<uint>> { };

    /*private static void CleanDatabase(List<uint> chestList)
    {
      var sql = $"SELECT tag from {SQL_TABLE}";
      var chestTags = chestList.Select(chest => NWScript.GetTag(chest));

      using (var connection = MySQL.GetConnection())
      {
        var lootContainers = connection.Query<Models.LootContainer>(sql).ToList();

        foreach (var lootContainer in lootContainers)
        {
          if (!chestTags.Contains(lootContainer.tag))
          {
            sql = $"DELETE FROM {SQL_TABLE} WHERE tag=@tag;";

            connection.Execute(sql, new { tag = lootContainer.tag });
          }
        }
      }
    }*/

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
    /*
    private static void InitChest(uint oChest, uint oArea)
    {
      var chestTag = NWScript.GetTag(oChest);
      var sql = $"SELECT serialized FROM {SQL_TABLE} WHERE tag=@tag LIMIT 1;";

      using (var connection = MySQL.GetConnection())
      {
        try
        {
          var lootContainer = connection.QuerySingle<Models.LootContainer>(sql, new { tag = chestTag });
          var oDeserializedChest = ObjectPlugin.Deserialize(lootContainer.serialized);
          var location = NWScript.GetLocation(oChest);
          var oChestPosition = NWScript.GetPositionFromLocation(location);
          var direction = NWScript.GetFacingFromLocation(location);
          ObjectPlugin.AddToArea(oDeserializedChest, oArea, oChestPosition);
          NWScript.AssignCommand(oDeserializedChest, () => NWScript.SetFacing(direction));
          NWScript.SetEventScript(oDeserializedChest, NWScript.EVENT_SCRIPT_PLACEABLE_ON_CLOSED, LOOT_CONTAINER_ON_CLOSE_SCRIPT);
          NWScript.DestroyObject(oChest);
        }
        catch (Exception _)
        {
          UpdateDB(oChest);
        }

      }

      UpdateChestTagToLootsDic(oChest);
    }
    */
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
