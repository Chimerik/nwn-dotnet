using System;
using System.Linq;
using NWN.API;
using NWN.Core;
using NWN.Core.NWNX;
using static NWN.Systems.PlayerSystem;

namespace NWN.Systems
{
    class Bibliothecaire
    {
        public Bibliothecaire(Player player, NwCreature blacksmith)
        {
            if (!DateTime.TryParse(NWScript.GetLocalString(blacksmith.Area, "_DATE_LAST_TRIGGERED"), out DateTime previousSpawnDate) || (DateTime.Now - previousSpawnDate).TotalHours > 4)
            {
                NWScript.SetLocalString(NWScript.GetArea(blacksmith), "_DATE_LAST_TRIGGERED", DateTime.Now.ToString());

                NwStore shop = blacksmith.GetNearestObjectsByType<NwStore>().Where(s => s.Tag == "bibliothecaire_shop").FirstOrDefault();

                if (!shop.IsValid)
                {
                    NwStore.Create("generic_shop_res", blacksmith.Location, false, "bibliothecaire_shop");
                    NWScript.SetLocalObject(shop, "_STORE_NPC", blacksmith);
                }
                if (NWN.Utils.random.Next(1, 101) < 21)
                {
                    int feat = (int)SkillSystem.languageSkillBooks[NWN.Utils.random.Next(0, SkillSystem.languageSkillBooks.Length)];
                    uint skillBook = NWScript.CreateItemOnObject("skillbookgeneriq", shop, 1, "skillbook");
                    ItemPlugin.SetItemAppearance(skillBook, NWScript.ITEM_APPR_TYPE_SIMPLE_MODEL, 2, NWN.Utils.random.Next(0, 50));
                    NWScript.SetLocalInt(skillBook, "_SKILL_ID", feat);

                    int value;
                    if (int.TryParse(NWScript.Get2DAString("feat", "FEAT", feat), out value))
                        NWScript.SetName(skillBook, NWScript.GetStringByStrRef(value));

                    if (int.TryParse(NWScript.Get2DAString("feat", "DESCRIPTION", feat), out value))
                        NWScript.SetDescription(skillBook, NWScript.GetStringByStrRef(value));

                    ItemPlugin.SetBaseGoldPieceValue(skillBook, 3000);
                }
                else
                {
                    uint skillBook = NWScript.CreateItemOnObject("skillbookgeneriq", shop, 1, "ruined_book");
                    ItemPlugin.SetItemAppearance(skillBook, NWScript.ITEM_APPR_TYPE_SIMPLE_MODEL, 2, NWN.Utils.random.Next(0, 50));
                    NWScript.SetName(skillBook, "Ouvrage ruiné");
                    NWScript.SetDescription(skillBook, "Cet ouvrage est abîmé au-delà de toute rédemption. Il est même trop humide pour faire du feu.");
                    ItemPlugin.SetBaseGoldPieceValue(skillBook, 3000);
                }

                NWScript.OpenStore(shop, player.oid);
            }
        }
    }
}
