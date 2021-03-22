using System.Collections.Generic;
using NWN.Services;

namespace NWN.Systems
{
  public partial class LootSystem
  {
    public readonly static string LOOT_CONTAINER_ON_CLOSE_SCRIPT = "ls_load_onclose";
    public readonly static string ON_LOOT_SCRIPT = "ls_onloot";
    public readonly static string CHEST_AREA_TAG = "la_zone_des_loots";
    private readonly static string SQL_TABLE = "loot_containers";

    public readonly static Dictionary<string, Lootable.Config> lootablesDic = new Dictionary<string, Lootable.Config>
        {
            { "gobelin_chest", new Lootable.Config(
                gold: new Lootable.Gold(min: 15, max: 65, chance: 75),
                items: new List<Lootable.Item> {
                    new Lootable.Item(chestTag: "simple_weapons", count: 1, chance: 2),
                    new Lootable.Item(chestTag: "light_armor", count: 1, chance: 2),
                    new Lootable.Item(chestTag: "ammunitions", count: 1, chance: 25),
                    new Lootable.Item(chestTag: "low_blueprints", count: 1, chance: 1),
                    new Lootable.Item(chestTag: "low_skillbooks", count: 1, chance: 1),
                    new Lootable.Item(chestTag: "low_enchantements", count: 1, chance: 1),
                    new Lootable.Item(chestTag: "low_potions", count: 2, chance: 80),
                    new Lootable.Item(chestTag: "low_scrolls", count: 1, chance: 30),
                }
            )},
            { "Gobelinclaireur", new Lootable.Config(
                items: new List<Lootable.Item> {
                    new Lootable.Item(chestTag: "ammunitions", count: 1, chance: 5),
                }
            )},
            { "Gobelinchairacanon", new Lootable.Config(
                gold: new Lootable.Gold(min: 1, max: 12, chance: 20),
                items: new List<Lootable.Item> {
                    new Lootable.Item(chestTag: "ammunitions", count: 1, chance: 5),
                    new Lootable.Item(chestTag: "light_armor", count: 1, chance: 1),
                }
            )},
            { "Gobelinfrondeur", new Lootable.Config(
                gold: new Lootable.Gold(min: 1, max: 12, chance: 20),
                items: new List<Lootable.Item> {
                    new Lootable.Item(chestTag: "ammunitions", count: 1, chance: 25),
                }
            )},
            { "Gobelinfourbe", new Lootable.Config(
                gold: new Lootable.Gold(min: 1, max: 12, chance: 20),
                items: new List<Lootable.Item> {
                    new Lootable.Item(chestTag: "ammunitions", count: 1, chance: 35),
                }
            )},
            { "boss_gobelin", new Lootable.Config(
                gold: new Lootable.Gold(min: 15, max: 75, chance: 75),
                items: new List<Lootable.Item> {
                    new Lootable.Item(chestTag: "simple_weapons", count: 1, chance: 10),
                    new Lootable.Item(chestTag: "light_armor", count: 1, chance: 5),
                    new Lootable.Item(chestTag: "ammunitions", count: 1, chance: 50),
                    new Lootable.Item(chestTag: "low_blueprints", count: 1, chance: 1),
                    new Lootable.Item(chestTag: "low_skillbooks", count: 1, chance: 1),
                    new Lootable.Item(chestTag: "low_enchantements", count: 1, chance: 1),
                    new Lootable.Item(chestTag: "low_potions", count: 2, chance: 80),
                    new Lootable.Item(chestTag: "low_scrolls", count: 1, chance: 80),
                }
            )},
            { "kobolt_chest", new Lootable.Config(
                gold: new Lootable.Gold(min: 15, max: 65, chance: 75),
                items: new List<Lootable.Item> {
                    new Lootable.Item(chestTag: "simple_weapons", count: 1, chance: 3),
                    new Lootable.Item(chestTag: "light_armor", count: 1, chance: 3),
                    new Lootable.Item(chestTag: "ammunitions", count: 1, chance: 35),
                    new Lootable.Item(chestTag: "low_blueprints", count: 1, chance: 1),
                    new Lootable.Item(chestTag: "low_skillbooks", count: 1, chance: 1),
                    new Lootable.Item(chestTag: "low_enchantements", count: 1, chance: 1),
                    new Lootable.Item(chestTag: "low_potions", count: 3, chance: 50),
                    new Lootable.Item(chestTag: "low_scrolls", count: 2, chance: 25),
                }
            )},
            { "boss_kobolt", new Lootable.Config(
                gold: new Lootable.Gold(min: 25, max: 75, chance: 100),
                items: new List<Lootable.Item> {
                    new Lootable.Item(chestTag: "simple_weapons", count: 1, chance: 10),
                    new Lootable.Item(chestTag: "light_armor", count: 1, chance: 5),
                    new Lootable.Item(chestTag: "ammunitions", count: 1, chance: 50),
                    new Lootable.Item(chestTag: "low_blueprints", count: 1, chance: 2),
                    new Lootable.Item(chestTag: "low_skillbooks", count: 1, chance: 1),
                    new Lootable.Item(chestTag: "low_enchantements", count: 1, chance: 1),
                    new Lootable.Item(chestTag: "low_potions", count: 3, chance: 85),
                    new Lootable.Item(chestTag: "low_scrolls", count: 2, chance: 85),
                }
            )},
            { "Koboltfantassin", new Lootable.Config(
                gold: new Lootable.Gold(min: 2, max: 12, chance: 35),
                items: new List<Lootable.Item> {
                    new Lootable.Item(chestTag: "light_armor", count: 1, chance: 1),
                    new Lootable.Item(chestTag: "ammunitions", count: 1, chance: 5),
                }
            )},
            { "Koboltsournois", new Lootable.Config(
                gold: new Lootable.Gold(min: 2, max: 12, chance: 35),
                items: new List<Lootable.Item> {
                    new Lootable.Item(chestTag: "ammunitions", count: 1, chance: 10),
                }
            )},
            { "StatueCristalline", new Lootable.Config(
                gold: new Lootable.Gold(min: 15, max: 40, chance: 35),
                items: new List<Lootable.Item> {
                    new Lootable.Item(chestTag: "light_armor", count: 1, chance: 1),
                    new Lootable.Item(chestTag: "low_blueprints", count: 1, chance: 1),
                    new Lootable.Item(chestTag: "low_skillbooks", count: 1, chance: 1),
                    new Lootable.Item(chestTag: "low_enchantements", count: 1, chance: 1),
                }
            )},
            { "boss_gothra", new Lootable.Config(
                gold: new Lootable.Gold(min: 75, max: 250, chance: 100),
                items: new List<Lootable.Item> {
                    new Lootable.Item(chestTag: "simple_weapons", count: 1, chance: 10),
                    new Lootable.Item(chestTag: "light_armor", count: 1, chance: 10),
                    new Lootable.Item(chestTag: "low_blueprints", count: 1, chance: 5),
                    new Lootable.Item(chestTag: "low_skillbooks", count: 1, chance: 5),
                    new Lootable.Item(chestTag: "low_enchantements", count: 1, chance: 1),
                    new Lootable.Item(chestTag: "low_potions", count: 3, chance: 85),
                    new Lootable.Item(chestTag: "low_scrolls", count: 2, chance: 85),
                }
            )},
            { "gothra_chest", new Lootable.Config(
                gold: new Lootable.Gold(min: 50, max: 100, chance: 50),
                items: new List<Lootable.Item> {
                    new Lootable.Item(chestTag: "simple_weapons", count: 1, chance: 10),
                    new Lootable.Item(chestTag: "light_armor", count: 1, chance: 10),
                    new Lootable.Item(chestTag: "low_blueprints", count: 1, chance: 5),
                    new Lootable.Item(chestTag: "low_skillbooks", count: 1, chance: 5),
                    new Lootable.Item(chestTag: "low_enchantements", count: 1, chance: 5),
                    new Lootable.Item(chestTag: "low_potions", count: 3, chance: 85),
                    new Lootable.Item(chestTag: "low_scrolls", count: 2, chance: 85),
                }
            )},
            { "KuoToa", new Lootable.Config(
                gold: new Lootable.Gold(min: 30, max: 55, chance: 35),
                items: new List<Lootable.Item> {
                    new Lootable.Item(chestTag: "simple_weapons", count: 1, chance: 1),
                    new Lootable.Item(chestTag: "light_armor", count: 1, chance: 1),
                    new Lootable.Item(chestTag: "ammunitions", count: 1, chance: 5),
                }
            )},
            { "KuoToafouettard", new Lootable.Config(
                gold: new Lootable.Gold(min: 45, max: 65, chance: 35),
                items: new List<Lootable.Item> {
                    new Lootable.Item(chestTag: "simple_weapons", count: 1, chance: 1),
                    new Lootable.Item(chestTag: "ammunitions", count: 1, chance: 10),
                    new Lootable.Item(chestTag: "low_potions", count: 1, chance: 25),
                    new Lootable.Item(chestTag: "low_scrolls", count: 1, chance: 15),
                }
            )},
            { "KuoToasurveillant", new Lootable.Config(
                gold: new Lootable.Gold(min: 35, max: 60, chance: 35),
                items: new List<Lootable.Item> {
                    new Lootable.Item(chestTag: "simple_weapons", count: 1, chance: 1),
                    new Lootable.Item(chestTag: "ammunitions", count: 1, chance: 10),
                }
            )},
            { "kuotoa_chest", new Lootable.Config(
                gold: new Lootable.Gold(min: 250, max: 400, chance: 85),
                items: new List<Lootable.Item> {
                    new Lootable.Item(chestTag: "simple_weapons", count: 1, chance: 4),
                    new Lootable.Item(chestTag: "light_armor", count: 1, chance: 4),
                    new Lootable.Item(chestTag: "ammunitions", count: 1, chance: 45),
                    new Lootable.Item(chestTag: "low_blueprints", count: 1, chance: 3),
                    new Lootable.Item(chestTag: "low_skillbooks", count: 1, chance: 3),
                    new Lootable.Item(chestTag: "low_enchantements", count: 1, chance: 3),
                    new Lootable.Item(chestTag: "low_potions", count: 3, chance: 75),
                    new Lootable.Item(chestTag: "low_scrolls", count: 2, chance: 45),
                }
            )},
            { "boss_harpie", new Lootable.Config(
                gold: new Lootable.Gold(min: 250, max: 400, chance: 100),
                items: new List<Lootable.Item> {
                    new Lootable.Item(chestTag: "simple_weapons", count: 1, chance: 5),
                    new Lootable.Item(chestTag: "light_armor", count: 1, chance: 5),
                    new Lootable.Item(chestTag: "ammunitions", count: 1, chance: 50),
                    new Lootable.Item(chestTag: "low_blueprints", count: 1, chance: 3),
                    new Lootable.Item(chestTag: "low_skillbooks", count: 1, chance: 3),
                    new Lootable.Item(chestTag: "low_enchantements", count: 1, chance: 3),
                    new Lootable.Item(chestTag: "low_potions", count: 3, chance: 85),
                    new Lootable.Item(chestTag: "low_scrolls", count: 2, chance: 85),
                }
            )},
            { "MercenairedelOmniscienceranged", new Lootable.Config(
                gold: new Lootable.Gold(min: 10, max: 45, chance: 20),
                items: new List<Lootable.Item> {
                    new Lootable.Item(chestTag: "ammunitions", count: 1, chance: 25),
                    new Lootable.Item(chestTag: "simple_weapons", count: 1, chance: 1),
                    new Lootable.Item(chestTag: "light_armor", count: 1, chance: 1),
                }
            )},
            { "MercenairedelOmniscience", new Lootable.Config(
                gold: new Lootable.Gold(min: 10, max: 45, chance: 20),
                items: new List<Lootable.Item> {
                    new Lootable.Item(chestTag: "simple_weapons", count: 1, chance: 1),
                    new Lootable.Item(chestTag: "light_armor", count: 1, chance: 1),
                }
            )},
            { "HerautdelOmniscience", new Lootable.Config(
                gold: new Lootable.Gold(min: 35, max: 65, chance: 20),
                items: new List<Lootable.Item> {
                    new Lootable.Item(chestTag: "simple_weapons", count: 1, chance: 1),
                    new Lootable.Item(chestTag: "ammunitions", count: 1, chance: 25),
                    new Lootable.Item(chestTag: "light_armor", count: 1, chance: 1),
                    new Lootable.Item(chestTag: "low_potions", count: 1, chance: 50),
                    new Lootable.Item(chestTag: "low_scrolls", count: 1, chance: 50),
                }
            )},
            { "Moinedelomniscience", new Lootable.Config(
                gold: new Lootable.Gold(min: 35, max: 65, chance: 20),
                items: new List<Lootable.Item> {
                    new Lootable.Item(chestTag: "low_potions", count: 1, chance: 75),
                }
            )},
            { "PretredelOmniscience", new Lootable.Config(
                gold: new Lootable.Gold(min: 100, max: 200, chance: 20),
                items: new List<Lootable.Item> {
                    new Lootable.Item(chestTag: "simple_weapons", count: 1, chance: 25),
                    new Lootable.Item(chestTag: "light_armor", count: 1, chance: 25),
                    new Lootable.Item(chestTag: "low_potions", count: 1, chance: 50),
                    new Lootable.Item(chestTag: "low_scrolls", count: 1, chance: 50),
                }
            )},
            { "RodeurdelOmniscience", new Lootable.Config(
                gold: new Lootable.Gold(min: 100, max: 200, chance: 20),
                items: new List<Lootable.Item> {
                    new Lootable.Item(chestTag: "simple_weapons", count: 1, chance: 25),
                    new Lootable.Item(chestTag: "ammunitions", count: 1, chance: 25),
                    new Lootable.Item(chestTag: "light_armor", count: 1, chance: 25),
                    new Lootable.Item(chestTag: "low_potions", count: 1, chance: 50),
                }
            )},
            { "PaladindelOmniscience", new Lootable.Config(
                gold: new Lootable.Gold(min: 250, max: 450, chance: 20),
                items: new List<Lootable.Item> {
                    new Lootable.Item(chestTag: "simple_weapons", count: 1, chance: 50),
                    new Lootable.Item(chestTag: "martial_weapons", count: 1, chance: 1),
                    new Lootable.Item(chestTag: "light_armor", count: 1, chance: 50),
                    new Lootable.Item(chestTag: "medium_armor", count: 1, chance: 2),
                    new Lootable.Item(chestTag: "low_potions", count: 1, chance: 50),
                    new Lootable.Item(chestTag: "medium_potions", count: 1, chance: 25),
                }
            )},
            { "PrecheurdelOmniscience", new Lootable.Config(
                gold: new Lootable.Gold(min: 250, max: 450, chance: 20),
                items: new List<Lootable.Item> {
                    new Lootable.Item(chestTag: "low_potions", count: 2, chance: 100),
                    new Lootable.Item(chestTag: "medium_potions", count: 1, chance: 50),
                    new Lootable.Item(chestTag: "low_scrolls", count: 2, chance: 100),
                    new Lootable.Item(chestTag: "medium_scrolls", count: 1, chance: 50),
                }
            )},
            { "boss_omni", new Lootable.Config(
                gold: new Lootable.Gold(min: 500, max: 850, chance: 80),
                items: new List<Lootable.Item> {
                    new Lootable.Item(chestTag: "simple_weapons", count: 1, chance: 100),
                    new Lootable.Item(chestTag: "martial_weapons", count: 1, chance: 10),
                    new Lootable.Item(chestTag: "light_armor", count: 1, chance: 100),
                    new Lootable.Item(chestTag: "medium_armor", count: 1, chance: 10),
                    new Lootable.Item(chestTag: "low_potions", count: 3, chance: 100),
                    new Lootable.Item(chestTag: "medium_potions", count: 1, chance: 100),
                    new Lootable.Item(chestTag: "low_scrolls", count: 3, chance: 100),
                    new Lootable.Item(chestTag: "medium_scrolls", count: 1, chance: 100),
                    new Lootable.Item(chestTag: "low_blueprints", count: 1, chance: 10),
                    new Lootable.Item(chestTag: "low_skillbooks", count: 1, chance: 10),
                    new Lootable.Item(chestTag: "low_enchantements", count: 1, chance: 10),
                    new Lootable.Item(chestTag: "medium_blueprints", count: 1, chance: 1),
                    new Lootable.Item(chestTag: "medium_skillbooks", count: 1, chance: 1),
                    new Lootable.Item(chestTag: "medium_enchantements", count: 1, chance: 1),
                }
            )},
            { "omniscience_chest", new Lootable.Config(
                gold: new Lootable.Gold(min: 500, max: 850, chance: 90),
                items: new List<Lootable.Item> {
                    new Lootable.Item(chestTag: "simple_weapons", count: 1, chance: 85),
                    new Lootable.Item(chestTag: "martial_weapons", count: 1, chance: 10),
                    new Lootable.Item(chestTag: "light_armor", count: 1, chance: 85),
                    new Lootable.Item(chestTag: "medium_armor", count: 1, chance: 10),
                    new Lootable.Item(chestTag: "low_potions", count: 3, chance: 85),
                    new Lootable.Item(chestTag: "medium_potions", count: 1, chance: 90),
                    new Lootable.Item(chestTag: "low_scrolls", count: 3, chance: 75),
                    new Lootable.Item(chestTag: "medium_scrolls", count: 1, chance: 60),
                    new Lootable.Item(chestTag: "low_blueprints", count: 1, chance: 10),
                    new Lootable.Item(chestTag: "low_skillbooks", count: 1, chance: 10),
                    new Lootable.Item(chestTag: "low_enchantements", count: 1, chance: 10),
                    new Lootable.Item(chestTag: "medium_blueprints", count: 1, chance: 1),
                    new Lootable.Item(chestTag: "medium_skillbooks", count: 1, chance: 1),
                    new Lootable.Item(chestTag: "medium_enchantements", count: 1, chance: 1),
                }
            )},
            { "boss_bandit", new Lootable.Config(
                gold: new Lootable.Gold(min: 50, max: 150, chance: 20),
                items: new List<Lootable.Item> {
                    new Lootable.Item(chestTag: "simple_weapons", count: 1, chance: 15),
                    new Lootable.Item(chestTag: "light_armor", count: 1, chance: 15),
                    new Lootable.Item(chestTag: "low_potions", count: 1, chance: 50),
                }
            )},
            { "Banditelfe", new Lootable.Config(
                gold: new Lootable.Gold(min: 50, max: 150, chance: 20),
                items: new List<Lootable.Item> {
                    new Lootable.Item(chestTag: "simple_weapons", count: 1, chance: 5),
                    new Lootable.Item(chestTag: "ammunitions", count: 1, chance: 25),
                    new Lootable.Item(chestTag: "low_potions", count: 1, chance: 50),
                    new Lootable.Item(chestTag: "low_scrolls", count: 1, chance: 50),
                }
            )},
            { "Bandithalfelin", new Lootable.Config(
                gold: new Lootable.Gold(min: 50, max: 150, chance: 20),
                items: new List<Lootable.Item> {
                    new Lootable.Item(chestTag: "simple_weapons", count: 1, chance: 5),
                    new Lootable.Item(chestTag: "ammunitions", count: 1, chance: 25),
                    new Lootable.Item(chestTag: "light_armor", count: 1, chance: 5),
                    new Lootable.Item(chestTag: "low_potions", count: 1, chance: 50),
                }
            )},
            { "Banditdemiorc", new Lootable.Config(
                gold: new Lootable.Gold(min: 50, max: 150, chance: 20),
                items: new List<Lootable.Item> {
                    new Lootable.Item(chestTag: "simple_weapons", count: 1, chance: 5),
                    new Lootable.Item(chestTag: "light_armor", count: 1, chance: 5),
                    new Lootable.Item(chestTag: "low_potions", count: 1, chance: 50),
                }
            )},
            { "bandits_chest", new Lootable.Config(
                gold: new Lootable.Gold(min: 50, max: 150, chance: 75),
                items: new List<Lootable.Item> {
                    new Lootable.Item(chestTag: "simple_weapons", count: 1, chance: 3),
                    new Lootable.Item(chestTag: "light_armor", count: 1, chance: 5),
                    new Lootable.Item(chestTag: "low_potions", count: 1, chance: 80),
                    new Lootable.Item(chestTag: "low_scrolls", count: 1, chance: 80),
                }
            )},
            { "gnollwarrior", new Lootable.Config(
                gold: new Lootable.Gold(min: 50, max: 150, chance: 20),
                items: new List<Lootable.Item> {
                    new Lootable.Item(chestTag: "simple_weapons", count: 1, chance: 5),
                    new Lootable.Item(chestTag: "light_armor", count: 1, chance: 5),
                    new Lootable.Item(chestTag: "low_potions", count: 1, chance: 15),
                }
            )},
            { "gnollranger", new Lootable.Config(
                gold: new Lootable.Gold(min: 50, max: 150, chance: 20),
                items: new List<Lootable.Item> {
                    new Lootable.Item(chestTag: "simple_weapons", count: 1, chance: 5),
                    new Lootable.Item(chestTag: "ammunitions", count: 1, chance: 25),
                    new Lootable.Item(chestTag: "light_armor", count: 1, chance: 5),
                    new Lootable.Item(chestTag: "low_potions", count: 1, chance: 15),
                }
            )},
            { "Gnollshaman", new Lootable.Config(
                gold: new Lootable.Gold(min: 50, max: 150, chance: 20),
                items: new List<Lootable.Item> {
                    new Lootable.Item(chestTag: "simple_weapons", count: 1, chance: 5),
                    new Lootable.Item(chestTag: "light_armor", count: 1, chance: 5),
                    new Lootable.Item(chestTag: "low_potions", count: 1, chance: 15),
                    new Lootable.Item(chestTag: "low_scrolls", count: 1, chance: 15),
                    new Lootable.Item(chestTag: "medium_potions", count: 1, chance: 2),
                    new Lootable.Item(chestTag: "medium_scrolls", count: 1, chance: 2),
                }
            )},
            { "boss_gnoll", new Lootable.Config(
                gold: new Lootable.Gold(min: 250, max: 500, chance: 80),
                items: new List<Lootable.Item> {
                    new Lootable.Item(chestTag: "simple_weapons", count: 1, chance: 75),
                    new Lootable.Item(chestTag: "martial_weapons", count: 1, chance: 3),
                    new Lootable.Item(chestTag: "light_armor", count: 1, chance: 75),
                    new Lootable.Item(chestTag: "medium_armor", count: 1, chance: 3),
                    new Lootable.Item(chestTag: "low_potions", count: 2, chance: 100),
                    new Lootable.Item(chestTag: "medium_potions", count: 1, chance: 100),
                    new Lootable.Item(chestTag: "low_scrolls", count: 2, chance: 100),
                    new Lootable.Item(chestTag: "medium_scrolls", count: 1, chance: 100),
                    new Lootable.Item(chestTag: "low_blueprints", count: 1, chance: 5),
                    new Lootable.Item(chestTag: "low_skillbooks", count: 1, chance: 5),
                    new Lootable.Item(chestTag: "low_enchantements", count: 1, chance: 5),
                    new Lootable.Item(chestTag: "medium_skillbooks", count: 1, chance: 1),
                    new Lootable.Item(chestTag: "medium_blueprints", count: 1, chance: 1),
                    new Lootable.Item(chestTag: "medium_enchantements", count: 1, chance: 1),
                }
            )},
            { "gnoll_chest", new Lootable.Config(
                gold: new Lootable.Gold(min: 250, max: 500, chance: 80),
                items: new List<Lootable.Item> {
                    new Lootable.Item(chestTag: "simple_weapons", count: 1, chance: 45),
                    new Lootable.Item(chestTag: "martial_weapons", count: 1, chance: 2),
                    new Lootable.Item(chestTag: "light_armor", count: 1, chance: 55),
                    new Lootable.Item(chestTag: "medium_armor", count: 1, chance: 2),
                    new Lootable.Item(chestTag: "low_potions", count: 2, chance: 85),
                    new Lootable.Item(chestTag: "medium_potions", count: 1, chance: 65),
                    new Lootable.Item(chestTag: "low_scrolls", count: 2, chance: 75),
                    new Lootable.Item(chestTag: "medium_scrolls", count: 1, chance: 45),
                    new Lootable.Item(chestTag: "low_blueprints", count: 1, chance: 4),
                    new Lootable.Item(chestTag: "low_skillbooks", count: 1, chance: 4),
                    new Lootable.Item(chestTag: "low_enchantements", count: 1, chance: 4),
                    new Lootable.Item(chestTag: "medium_skillbooks", count: 1, chance: 1),
                    new Lootable.Item(chestTag: "medium_blueprints", count: 1, chance: 1),
                    new Lootable.Item(chestTag: "medium_enchantements", count: 1, chance: 1),
                }
            )},
            { "ant_chest", new Lootable.Config(
                gold: new Lootable.Gold(min: 250, max: 500, chance: 75),
                items: new List<Lootable.Item> {
                    new Lootable.Item(chestTag: "simple_weapons", count: 1, chance: 55),
                    new Lootable.Item(chestTag: "martial_weapons", count: 1, chance: 2),
                    new Lootable.Item(chestTag: "light_armor", count: 1, chance: 55),
                    new Lootable.Item(chestTag: "medium_armor", count: 1, chance: 2),
                    new Lootable.Item(chestTag: "low_potions", count: 2, chance: 85),
                    new Lootable.Item(chestTag: "medium_potions", count: 1, chance: 75),
                    new Lootable.Item(chestTag: "low_scrolls", count: 2, chance: 85),
                    new Lootable.Item(chestTag: "medium_scrolls", count: 1, chance: 65),
                    new Lootable.Item(chestTag: "low_blueprints", count: 1, chance: 3),
                    new Lootable.Item(chestTag: "low_skillbooks", count: 1, chance: 3),
                    new Lootable.Item(chestTag: "low_enchantements", count: 1, chance: 3),
                    new Lootable.Item(chestTag: "medium_skillbooks", count: 1, chance: 1),
                    new Lootable.Item(chestTag: "medium_blueprints", count: 1, chance: 1),
                    new Lootable.Item(chestTag: "medium_enchantements", count: 1, chance: 1),
                }
            )},
            { "Gobelourscombattant", new Lootable.Config(
                gold: new Lootable.Gold(min: 50, max: 150, chance: 20),
                items: new List<Lootable.Item> {
                    new Lootable.Item(chestTag: "simple_weapons", count: 1, chance: 5),
                    new Lootable.Item(chestTag: "light_armor", count: 1, chance: 5),
                    new Lootable.Item(chestTag: "low_potions", count: 1, chance: 15),
                }
            )},
            { "Gobeloursfurieux", new Lootable.Config(
                gold: new Lootable.Gold(min: 50, max: 150, chance: 20),
                items: new List<Lootable.Item> {
                    new Lootable.Item(chestTag: "simple_weapons", count: 1, chance: 5),
                    new Lootable.Item(chestTag: "light_armor", count: 1, chance: 5),
                    new Lootable.Item(chestTag: "low_potions", count: 1, chance: 15),
                }
            )},
            { "Gobeloursvicieux", new Lootable.Config(
                gold: new Lootable.Gold(min: 50, max: 150, chance: 20),
                items: new List<Lootable.Item> {
                    new Lootable.Item(chestTag: "simple_weapons", count: 1, chance: 5),
                    new Lootable.Item(chestTag: "light_armor", count: 1, chance: 5),
                    new Lootable.Item(chestTag: "low_potions", count: 1, chance: 15),
                }
            )},
            { "Gobeloursshaman", new Lootable.Config(
                gold: new Lootable.Gold(min: 50, max: 150, chance: 20),
                items: new List<Lootable.Item> {
                    new Lootable.Item(chestTag: "simple_weapons", count: 1, chance: 5),
                    new Lootable.Item(chestTag: "light_armor", count: 1, chance: 5),
                    new Lootable.Item(chestTag: "low_potions", count: 1, chance: 15),
                    new Lootable.Item(chestTag: "low_scrolls", count: 1, chance: 15),
                    new Lootable.Item(chestTag: "medium_potions", count: 1, chance: 2),
                    new Lootable.Item(chestTag: "medium_scrolls", count: 1, chance: 2),
                }
            )},
            { "boss_gobelours", new Lootable.Config(
                gold: new Lootable.Gold(min: 250, max: 500, chance: 80),
                items: new List<Lootable.Item> {
                    new Lootable.Item(chestTag: "simple_weapons", count: 1, chance: 75),
                    new Lootable.Item(chestTag: "martial_weapons", count: 1, chance: 3),
                    new Lootable.Item(chestTag: "light_armor", count: 1, chance: 75),
                    new Lootable.Item(chestTag: "medium_armor", count: 1, chance: 3),
                    new Lootable.Item(chestTag: "low_potions", count: 2, chance: 100),
                    new Lootable.Item(chestTag: "medium_potions", count: 1, chance: 100),
                    new Lootable.Item(chestTag: "low_scrolls", count: 2, chance: 100),
                    new Lootable.Item(chestTag: "medium_scrolls", count: 1, chance: 100),
                    new Lootable.Item(chestTag: "low_blueprints", count: 1, chance: 5),
                    new Lootable.Item(chestTag: "low_skillbooks", count: 1, chance: 5),
                    new Lootable.Item(chestTag: "low_enchantements", count: 1, chance: 5),
                    new Lootable.Item(chestTag: "medium_blueprints", count: 1, chance: 1),
                    new Lootable.Item(chestTag: "medium_skillbooks", count: 1, chance: 1),
                    new Lootable.Item(chestTag: "medium_enchantements", count: 1, chance: 1),
                }
            )},
            { "gobelours_chest", new Lootable.Config(
                gold: new Lootable.Gold(min: 250, max: 500, chance: 80),
                items: new List<Lootable.Item> {
                    new Lootable.Item(chestTag: "simple_weapons", count: 1, chance: 45),
                    new Lootable.Item(chestTag: "martial_weapons", count: 1, chance: 2),
                    new Lootable.Item(chestTag: "light_armor", count: 1, chance: 55),
                    new Lootable.Item(chestTag: "medium_armor", count: 1, chance: 2),
                    new Lootable.Item(chestTag: "low_potions", count: 2, chance: 85),
                    new Lootable.Item(chestTag: "medium_potions", count: 1, chance: 65),
                    new Lootable.Item(chestTag: "low_scrolls", count: 2, chance: 75),
                    new Lootable.Item(chestTag: "medium_scrolls", count: 1, chance: 45),
                    new Lootable.Item(chestTag: "low_blueprints", count: 1, chance: 4),
                    new Lootable.Item(chestTag: "low_skillbooks", count: 1, chance: 4),
                    new Lootable.Item(chestTag: "low_enchantements", count: 1, chance: 5),
                    new Lootable.Item(chestTag: "medium_skillbooks", count: 1, chance: 1),
                    new Lootable.Item(chestTag: "medium_blueprints", count: 1, chance: 1),
                    new Lootable.Item(chestTag: "medium_enchantements", count: 1, chance: 1),
                }
            )},
            { "Orcchoc", new Lootable.Config(
                gold: new Lootable.Gold(min: 100, max: 250, chance: 20),
                items: new List<Lootable.Item> {
                    new Lootable.Item(chestTag: "simple_weapons", count: 1, chance: 15),
                    new Lootable.Item(chestTag: "martial_weapons", count: 1, chance: 1),
                    new Lootable.Item(chestTag: "light_armor", count: 1, chance: 15),
                    new Lootable.Item(chestTag: "medium_armor", count: 1, chance: 1),
                    new Lootable.Item(chestTag: "low_potions", count: 1, chance: 25),
                }
            )},
            { "Orctireur", new Lootable.Config(
                gold: new Lootable.Gold(min: 100, max: 250, chance: 20),
                items: new List<Lootable.Item> {
                    new Lootable.Item(chestTag: "ammunitions", count: 1, chance: 25),
                    new Lootable.Item(chestTag: "simple_weapons", count: 1, chance: 15),
                    new Lootable.Item(chestTag: "martial_weapons", count: 1, chance: 1),
                    new Lootable.Item(chestTag: "light_armor", count: 1, chance: 15),
                    new Lootable.Item(chestTag: "medium_armor", count: 1, chance: 1),
                    new Lootable.Item(chestTag: "low_potions", count: 1, chance: 25),
                }
            )},
            { "Orcshaman", new Lootable.Config(
                gold: new Lootable.Gold(min: 100, max: 250, chance: 20),
                items: new List<Lootable.Item> {
                    new Lootable.Item(chestTag: "simple_weapons", count: 1, chance: 15),
                    new Lootable.Item(chestTag: "martial_weapons", count: 1, chance: 1),
                    new Lootable.Item(chestTag: "light_armor", count: 1, chance: 15),
                    new Lootable.Item(chestTag: "medium_armor", count: 1, chance: 1),
                    new Lootable.Item(chestTag: "low_potions", count: 1, chance: 35),
                    new Lootable.Item(chestTag: "low_scrolls", count: 1, chance: 35),
                    new Lootable.Item(chestTag: "medium_potions", count: 1, chance: 20),
                    new Lootable.Item(chestTag: "medium_scrolls", count: 1, chance: 20),
                }
            )},
            { "boss_orc", new Lootable.Config(
                gold: new Lootable.Gold(min: 500, max: 750, chance: 80),
                items: new List<Lootable.Item> {
                    new Lootable.Item(chestTag: "simple_weapons", count: 1, chance: 75),
                    new Lootable.Item(chestTag: "martial_weapons", count: 1, chance: 3),
                    new Lootable.Item(chestTag: "light_armor", count: 1, chance: 75),
                    new Lootable.Item(chestTag: "medium_armor", count: 1, chance: 3),
                    new Lootable.Item(chestTag: "low_potions", count: 2, chance: 100),
                    new Lootable.Item(chestTag: "medium_potions", count: 1, chance: 100),
                    new Lootable.Item(chestTag: "low_scrolls", count: 2, chance: 100),
                    new Lootable.Item(chestTag: "medium_scrolls", count: 1, chance: 100),
                    new Lootable.Item(chestTag: "low_blueprints", count: 1, chance: 5),
                    new Lootable.Item(chestTag: "low_skillbooks", count: 1, chance: 5),
                    new Lootable.Item(chestTag: "low_enchantements", count: 1, chance: 5),
                    new Lootable.Item(chestTag: "medium_skillbooks", count: 1, chance: 1),
                    new Lootable.Item(chestTag: "medium_blueprint", count: 1, chance: 1),
                    new Lootable.Item(chestTag: "medium_enchantements", count: 1, chance: 1),
                }
            )},
            { "orc_chest", new Lootable.Config(
                gold: new Lootable.Gold(min: 450, max: 700, chance: 90),
                items: new List<Lootable.Item> {
                    new Lootable.Item(chestTag: "simple_weapons", count: 1, chance: 45),
                    new Lootable.Item(chestTag: "martial_weapons", count: 1, chance: 3),
                    new Lootable.Item(chestTag: "light_armor", count: 1, chance: 55),
                    new Lootable.Item(chestTag: "medium_armor", count: 1, chance: 3),
                    new Lootable.Item(chestTag: "low_potions", count: 2, chance: 90),
                    new Lootable.Item(chestTag: "medium_potions", count: 1, chance: 65),
                    new Lootable.Item(chestTag: "low_scrolls", count: 2, chance: 85),
                    new Lootable.Item(chestTag: "medium_scrolls", count: 1, chance: 55),
                    new Lootable.Item(chestTag: "low_blueprints", count: 1, chance: 4),
                    new Lootable.Item(chestTag: "low_skillbooks", count: 1, chance: 4),
                    new Lootable.Item(chestTag: "low_enchantements", count: 1, chance: 4),
                    new Lootable.Item(chestTag: "medium_skillbooks", count: 1, chance: 1),
                    new Lootable.Item(chestTag: "medium_blueprints", count: 1, chance: 1),
                    new Lootable.Item(chestTag: "medium_enchantements", count: 1, chance: 1),
                }
            )},
        };
  }
}
