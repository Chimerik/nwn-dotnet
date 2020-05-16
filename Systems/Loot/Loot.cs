using System;
using System.Collections.Generic;

namespace NWN.Systems
{
    public static partial class Loot
    {
        private readonly static string LOOT_CONTAINER_ON_CLOSE_SCRIPT = "ls_load_onclose";
        private readonly static string CHEST_AREA_TAG = "la_zone_des_loots";
        private readonly static string SQL_TABLE = "loot_containers";

        public static Dictionary<string, Func<uint, int>> Register = new Dictionary<string, Func<uint, int>>
        {
            { LOOT_CONTAINER_ON_CLOSE_SCRIPT, OnContainerClose },
        };

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
    }
}
