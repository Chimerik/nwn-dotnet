using System.Linq;
using NWN.API;
using NWN.Core;
using NWN.Core.NWNX;
using static NWN.Systems.PlayerSystem;

namespace NWN.Systems
{
    class TribunalHotesse
    {
        public TribunalHotesse(Player player, NwCreature blacksmith)
        {
            NwStore shop = blacksmith.GetNearestObjectsByType<NwStore>().Where(s => s.Tag == "magic_shop").FirstOrDefault();

            if (!shop.IsValid)
            {
                NwStore.Create("generic_shop_res", blacksmith.Location, false, "magic_shop");
                NWScript.SetLocalObject(shop, "_STORE_NPC", blacksmith);

                foreach (int itemPropertyId in SkillSystem.shopBasicMagicScrolls)
                {
                    uint oScroll = NWScript.CreateItemOnObject("spellscroll", shop, 1, "scroll");
                    int spellId = int.Parse(NWScript.Get2DAString("iprp_spells", "SpellIndex", itemPropertyId));
                    NWScript.SetName(oScroll, $"{NWScript.GetStringByStrRef(int.Parse(NWScript.Get2DAString("spells", "Name", spellId)))}");
                    NWScript.SetDescription(oScroll, $"{NWScript.GetStringByStrRef(int.Parse(NWScript.Get2DAString("spells", "SpellDesc", spellId)))}");

                    NWScript.AddItemProperty(NWScript.DURATION_TYPE_PERMANENT, NWScript.ItemPropertyCastSpell(itemPropertyId, 1), oScroll);
                }
            }

            ChatPlugin.SendMessage(ChatPlugin.NWNX_CHAT_CHANNEL_PLAYER_TALK, "Pour obtenir votre amulette de concentration de l'arcane, il vous faut vous enregistrer auprès du juge.", blacksmith, player.oid);
            shop.Open(player.oid);
        }
    }
}
