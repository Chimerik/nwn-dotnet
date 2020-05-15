using NWN.MySQL;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NWN
{
    public class LootSystem
    {
        private readonly static string LOOT_CONTAINER_ON_CLOSE_SCRIPT = "ls_load_onclose";
        private readonly static string CHEST_AREA_TAG = "la_zone_des_loots";
        private readonly static string SQL_TABLE = "loot_containers";

        public static Dictionary<string, Func<uint, int>> Register = new Dictionary<string, Func<uint, int>>
        {
            { LOOT_CONTAINER_ON_CLOSE_SCRIPT, OnContainerClose },
        };

        private static Dictionary<string, List<uint>> chestTagToLootsDic = new Dictionary<string, List<uint>> {};

        public static void InitChestArea ()
        {
            var oArea = NWScript.GetObjectByTag(CHEST_AREA_TAG);

            if (oArea == NWScript.OBJECT_INVALID)
            {
                throw new ApplicationException($"LootSystem: Invalid CHEST_AREA_TAG={CHEST_AREA_TAG}");
            }

            var chestList = GetPlaceables(oArea);
            CleanDatabase(chestList);

            foreach (var oChest in chestList)
            {
                InitChest(oChest, oArea);
            }
        }

        private static int OnContainerClose (uint oidSelf)
        {
            UpdateChestTagToLootsDic(oidSelf);
            UpdateDB(oidSelf);
            return Entrypoints.SCRIPT_HANDLED;
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

        private static void UpdateDB (uint oChest)
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

        private static void InitChest (uint oChest, uint oArea)
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
                var oChestPosition = NWScript.GetPositionFromLocation(NWScript.GetLocation(oChest));
                NWNX.Object.AddToArea(oDeserializedChest, oArea, oChestPosition);
                NWScript.SetEventScript(oDeserializedChest, NWScript.EVENT_SCRIPT_PLACEABLE_ON_CLOSED, LOOT_CONTAINER_ON_CLOSE_SCRIPT);
                NWScript.DestroyObject(oChest);
            }
            else
            {
                UpdateDB(oChest);
            }

            UpdateChestTagToLootsDic(oChest);
        }

        private static List<uint> GetPlaceables (uint oArea)
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

        private static void UpdateChestTagToLootsDic (uint oChest)
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
    }
}
