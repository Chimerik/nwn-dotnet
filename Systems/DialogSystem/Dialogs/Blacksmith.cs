using System.Linq;
using NWN.API;
using NWN.Core;
using NWN.Core.NWNX;
using static NWN.Systems.PlayerSystem;

namespace NWN.Systems
{
    class Blacksmith
    {
        public Blacksmith(Player player, NwCreature blacksmith)
        {
            NwStore shop = blacksmith.GetNearestObjectsByType<NwStore>().Where(s => s.Tag == "blacksmith_shop").FirstOrDefault();

            if (!shop.IsValid)
            {
                NwStore.Create("generic_shop_res", blacksmith.Location, false, "blacksmith_shop");
                NWScript.SetLocalObject(shop, "_STORE_NPC", blacksmith);

                foreach (int baseItemType in Craft.Collect.System.forgeBasicBlueprints)
                {
                    Craft.Blueprint blueprint = new Craft.Blueprint(baseItemType);

                    if (!Craft.Collect.System.blueprintDictionnary.ContainsKey(baseItemType))
                        Craft.Collect.System.blueprintDictionnary.Add(baseItemType, blueprint);

                    NwItem oBlueprint = NwItem.Create("blueprintgeneric", shop);
                    oBlueprint.Name = $"Patron original : {blueprint.name}";
                    NWScript.SetLocalInt(oBlueprint, "_BASE_ITEM_TYPE", baseItemType);
                    ItemPlugin.SetBaseGoldPieceValue(oBlueprint, blueprint.goldCost * 10);
                }

                foreach (Feat feat in SkillSystem.forgeBasicSkillBooks)
                {
                    uint skillBook = NWScript.CreateItemOnObject("skillbookgeneriq", shop, 1, "skillbook");
                    ItemPlugin.SetItemAppearance(skillBook, NWScript.ITEM_APPR_TYPE_SIMPLE_MODEL, 2, NWN.Utils.random.Next(0, 50));
                    NWScript.SetLocalInt(skillBook, "_SKILL_ID", (int)feat);

                    int value;
                    if (int.TryParse(NWScript.Get2DAString("feat", "FEAT", (int)feat), out value))
                        NWScript.SetName(skillBook, NWScript.GetStringByStrRef(value));

                    if (int.TryParse(NWScript.Get2DAString("feat", "DESCRIPTION", (int)feat), out value))
                        NWScript.SetDescription(skillBook, NWScript.GetStringByStrRef(value));

                    if (int.TryParse(NWScript.Get2DAString("feat", "CRValue", (int)feat), out value))
                        ItemPlugin.SetBaseGoldPieceValue(skillBook, value * 1000);
                }

                uint craftTool = NWScript.CreateItemOnObject("oreextractor", shop, 1, "oreextractor");
                ItemPlugin.SetBaseGoldPieceValue(craftTool, 50);
                NWScript.SetLocalInt(craftTool, "_DURABILITY", 10);

                craftTool = NWScript.CreateItemOnObject("forgehammer", shop, 1, "forgehammer");
                ItemPlugin.SetBaseGoldPieceValue(craftTool, 50);
                NWScript.SetLocalInt(craftTool, "_DURABILITY", 5);
            }

            shop.Open(player.oid);
        }
    }
}
