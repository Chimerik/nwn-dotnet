using System.Collections.Generic;

namespace NWN.Systems
{
    public partial class LootSystem
    {
        private readonly static string LOOT_CONTAINER_ON_CLOSE_SCRIPT = "ls_load_onclose";
        private readonly static string ON_LOOT_SCRIPT = "ls_onloot";
        private readonly static string CHEST_AREA_TAG = "la_zone_des_loots";
        private readonly static string SQL_TABLE = "loot_containers";
        private readonly static string IS_LOOTED_VARNAME = "LS__IS_LOOTED";

        private readonly static Dictionary<string, Lootable.Config> lootablesDic = new Dictionary<string, Lootable.Config>
        {
            { "gobelin_dungeon_chest", new Lootable.Config(
                respawnDuration: 10f,
                gold: new Lootable.Gold(min: 10, max: 100, chance: 100),
                items: new List<Lootable.Item> {
                    new Lootable.Item(chestTag: "super_loots", count: 10, chance: 55),
                    new Lootable.Item(chestTag: "animal_loots", count: 1, chance: 55),
                }
            )},
            { "gobelin", new Lootable.Config(
                items: new List<Lootable.Item> {
                    new Lootable.Item(chestTag: "junk_loots", count: 3, chance: 70),
                }
            )},
            { "gobelin_chief", new Lootable.Config(
                gold: new Lootable.Gold(min: 100, max: 200, chance: 50),
                items: new List<Lootable.Item> {
                    new Lootable.Item(chestTag: "gobelin_chief_loots", count: 1, chance: 100),
                    new Lootable.Item(chestTag: "good_loots", count: 3, chance: 35),
                }
            )},
        };
    }
}
