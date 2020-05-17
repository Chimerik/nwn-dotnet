using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace NWN.Systems
{
    public static partial class Loot
    {
        private readonly static string LOOT_CONTAINER_ON_CLOSE_SCRIPT = "ls_load_onclose";
        private readonly static string ON_LOOT_SCRIPT = "ls_onloot";
        private readonly static string CHEST_AREA_TAG = "la_zone_des_loots";
        private readonly static string SQL_TABLE = "loot_containers";
        private readonly static string LOOT_DATA_VARNAME = "LS__LOOT_DATA";
        private readonly static string IS_LOOTED_VARNAME = "LS__IS_LOOTED";

        public static Dictionary<string, Func<uint, int>> Register = new Dictionary<string, Func<uint, int>>
        {
            { LOOT_CONTAINER_ON_CLOSE_SCRIPT, OnContainerClose },
            { ON_LOOT_SCRIPT, OnLoot },
        };

        public static void InitChestArea ()
        {
            var oArea = NWScript.GetObjectByTag(CHEST_AREA_TAG);

            if (oArea == NWScript.OBJECT_INVALID)
            {
                ThrowException($"Invalid CHEST_AREA_TAG={CHEST_AREA_TAG}");
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

        private static int OnLoot (uint oidSelf)
        {
            var oLooter = NWScript.GetLastKiller();
            var oContainer = oidSelf;
            var oArea = NWScript.GetArea(oContainer);

            var lootData = JsonConvert.DeserializeObject<Data>(
                NWScript.GetLocalString(oContainer, LOOT_DATA_VARNAME)
            );
            var respawnDuration = lootData.respawnDuration.GetValueOrDefault();

            if (NWScript.GetIsObjectValid(oLooter))
            {
                // Creature was killed or chest was destroyed
                if (lootData.respawnDuration != null)
                {
                    var type = NWScript.GetObjectType(oContainer);
                    var resref = NWScript.GetResRef(oContainer);
                    var location = NWScript.GetLocation(oContainer);

                    NWScript.AssignCommand(
                        oArea,
                        () => NWScript.DelayCommand(
                            respawnDuration,
                            () => NWScript.ActionDoCommand(
                                () => NWScript.CreateObject(type, resref, location)
                            )
                        )
                    );
                }
            } else
            {
                // Chest was opened
                oLooter = NWScript.GetLastOpenedBy();
            }

            if (!NWScript.GetIsObjectValid(oLooter))
            {
                ThrowException($"Invalid Event for the script {ON_LOOT_SCRIPT}");
            }

            if (NWScript.GetLocalInt(oContainer, IS_LOOTED_VARNAME) == 1)
            {
                // Prevents looting from opening chest multiple times, and then looting again by destroying it.
                return Entrypoints.SCRIPT_HANDLED;
            }

            Utils.DestroyInventory(oContainer);
            NWScript.AssignCommand(oArea, () => NWScript.DelayCommand(
                0.1f,
                () => GenerateLoot(oContainer, lootData)
            ));

            NWScript.SetLocalInt(oContainer, IS_LOOTED_VARNAME, 1);
            // Remove flag for next loot
            NWScript.AssignCommand(oArea, () => NWScript.DelayCommand(
                respawnDuration,
                () => NWScript.SetLocalInt(oContainer, IS_LOOTED_VARNAME, 0)
            ));

            return Entrypoints.SCRIPT_HANDLED;
        }
    }
}
