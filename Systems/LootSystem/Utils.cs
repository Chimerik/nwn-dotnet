﻿using System;
using System.Collections.Generic;
using System.Linq;
using Dapper;

namespace NWN.Systems
{
    public static partial class LootSystem
    {
        private static Dictionary<string, List<uint>> chestTagToLootsDic = new Dictionary<string, List<uint>> { };

        private static void CleanDatabase(List<uint> chestList)
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
        }

        private static void UpdateDB(uint oChest)
        {
            var tag = NWScript.GetTag(oChest);
            var sql = $"INSERT INTO {SQL_TABLE} (tag, serialized)" +
                    " VALUES (@tag, @serialized)" +
                    " ON DUPLICATE KEY UPDATE serialized=@serialized;";

            using (var connection = MySQL.GetConnection())
            {
                connection.Execute(sql, new {
                    tag = tag,
                    serialized = NWNX.Object.Serialize(oChest)
                });
            }
        }

        private static void InitChest(uint oChest, uint oArea)
        {
            var chestTag = NWScript.GetTag(oChest);
            var sql = $"SELECT serialized FROM {SQL_TABLE} WHERE tag=@tag LIMIT 1;";

            using (var connection = MySQL.GetConnection())
            {
                var lootContainer = connection.QueryFirst<Models.LootContainer>(sql, new { tag = chestTag });
                var oDeserializedChest = NWNX.Object.Deserialize(lootContainer.serialized);

                if (NWScript.GetIsObjectValid(oDeserializedChest))
                {
                    var location = NWScript.GetLocation(oChest);
                    var oChestPosition = NWScript.GetPositionFromLocation(location);
                    var direction = NWScript.GetFacingFromLocation(location);
                    NWNX.Object.AddToArea(oDeserializedChest, oArea, oChestPosition);
                    NWScript.AssignCommand(oDeserializedChest, () => NWScript.SetFacing(direction));
                    NWScript.SetEventScript(oDeserializedChest, NWScript.EVENT_SCRIPT_PLACEABLE_ON_CLOSED, LOOT_CONTAINER_ON_CLOSE_SCRIPT);
                    NWScript.DestroyObject(oChest);
                }
                else
                {
                    UpdateDB(oChest);
                }

                UpdateChestTagToLootsDic(oChest);
            }
        }

        private static List<uint> GetPlaceables(uint oArea)
        {
            var oPlaceable = NWScript.GetFirstObjectInArea(oArea);
            var list = new List<uint> { };

            while (NWScript.GetIsObjectValid(oPlaceable))
            {
                if (NWScript.GetObjectType(oPlaceable) == Enums.ObjectType.Placeable &&
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

            while (NWScript.GetIsObjectValid(oLoot))
            {
                loots.Add(oLoot);
                oLoot = NWScript.GetNextItemInInventory(oChest);
            }

            chestTagToLootsDic[tag] = loots;
        }

        private static void ThrowException (string message)
        {
            throw new ApplicationException($"LootSystem: {message}");
        }
    }
}