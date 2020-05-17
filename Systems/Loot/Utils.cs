using System;
using System.Collections.Generic;
using System.Linq;
using NWN.MySQL;

namespace NWN.Systems
{
    public static partial class Loot
    {
        private static Dictionary<string, List<uint>> chestTagToLootsDic = new Dictionary<string, List<uint>> { };

        struct Data
        {
            public float? respawnDuration;
            public Gold? gold;
            public List<Item> items;
        }

        struct Gold
        {
            public uint min;
            public uint max;
            public uint chance;
        }

        struct Item
        {
            public string chestTag;
            public uint count;
            public uint chance;
        }

        private static void CleanDatabase(List<uint> chestList)
        {
            var command = Client.CreateCommand($"SELECT tag from {SQL_TABLE}");
            var dataReader = command.ExecuteReader();
            var chestTags = chestList.Select(chest => NWScript.GetTag(chest));
            var dbTags = new List<string> { };

            while (dataReader.Read())
            {
                dbTags.Add(dataReader["tag"].ToString());
            }

            dataReader.Close();

            foreach (var tag in dbTags)
            {
                if (!chestTags.Contains(tag))
                {
                    var deleteCmd = Client.CreateCommand($"DELETE FROM {SQL_TABLE} WHERE tag=@tag;");
                    deleteCmd.Parameters.AddWithValue("@tag", tag);
                    deleteCmd.ExecuteNonQuery();
                }
            }
        }

        private static void UpdateDB(uint oChest)
        {
            var tag = NWScript.GetTag(oChest);
            var command = Client.CreateCommand(
                    $"INSERT INTO {SQL_TABLE} (tag, serialized)" +
                    " VALUES (@tag, @serialized)" +
                    " ON DUPLICATE KEY UPDATE serialized=@serialized;");
            command.Parameters.AddWithValue("@tag", tag);
            command.Parameters.AddWithValue("@serialized", NWNX.Object.Serialize(oChest));
            command.ExecuteNonQuery();
        }

        private static void InitChest(uint oChest, uint oArea)
        {
            var chestTag = NWScript.GetTag(oChest);
            var command = Client.CreateCommand($"SELECT serialized FROM {SQL_TABLE} WHERE tag=@tag LIMIT 1;");
            command.Parameters.AddWithValue("@tag", chestTag);

            var dataReader = command.ExecuteReader();

            uint oDeserializedChest = NWScript.OBJECT_INVALID;
            while (dataReader.Read())
            {
                oDeserializedChest = NWNX.Object.Deserialize(dataReader["serialized"].ToString());
            }

            dataReader.Close();

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

        private static void GenerateLoot (uint oContainer, Data loot)
        {
            var containerTag = NWScript.GetTag(oContainer);

            if (NWScript.GetHasInventory(oContainer) == 0)
            {
                ThrowException($"Can't GenerateLoot : Object '{containerTag}' has no inventory.");
            }

            if (loot.gold != null)
            {
                GenerateGold(oContainer, loot.gold.GetValueOrDefault());
            }

            GenerateItems(oContainer, loot.items);
        }

        private static void GenerateGold (uint oContainer, Gold gold, string goldResRef = "nw_it_gold001")
        {
            var random = new Random();
            if (random.Next(1, 100) <= gold.chance)
            {
                var goldCount = random.Next((int)gold.min, (int)gold.max);

                if (NWScript.GetObjectType(oContainer) == Enums.ObjectType.Creature)
                {
                    NWScript.GiveGoldToCreature(oContainer, goldCount);
                } else
                {
                    NWScript.CreateItemOnObject(goldResRef, oContainer, goldCount);
                }
            }
        }

        private static void GenerateItems (uint oContainer, List<Item> items)
        {
            var random = new Random();
            foreach (var item in items)
            {
                List<uint> loots;
                if (chestTagToLootsDic.TryGetValue(item.chestTag, out loots))
                {
                    for (var i = 0; i < item.count; i++)
                    {
                        if (random.Next(1, 100) < item.chance)
                        {
                            NWScript.CopyItem(
                                loots[random.Next(0, loots.Count - 1)],
                                oContainer,
                                true
                            );
                        }
                    }
                } else
                {
                    ThrowException($"Invalid chest tag '{item.chestTag}'");
                }
            }
        }
    }
}
