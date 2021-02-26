using System.Linq;
using NWN.API;
using NWN.Core;
using NWN.Core.NWNX;
using NWN.API.Constants;
using static NWN.Systems.PlayerSystem;

namespace NWN.Systems
{
  class TribunalHotesse
  {
    public TribunalHotesse(Player player, NwCreature magicshop)
    {
      NwStore shop = magicshop.GetNearestObjectsByType<NwStore>().Where(s => s.Tag == "magic_shop").FirstOrDefault();

      if (shop == null)
      {
        shop = NwStore.Create("generic_shop_res", magicshop.Location, false, "magic_shop");
        NWScript.SetLocalObject(shop, "_STORE_NPC", magicshop);

        foreach (int itemPropertyId in SkillSystem.shopBasicMagicScrolls)
        {
          NwItem oScroll = NwItem.Create("spellscroll", shop, 1, "scroll");
          int spellId = int.Parse(NWScript.Get2DAString("iprp_spells", "SpellIndex", itemPropertyId));
          oScroll.Name = $"{NWScript.GetStringByStrRef(int.Parse(NWScript.Get2DAString("spells", "Name", spellId)))}";
          oScroll.Description = $"{NWScript.GetStringByStrRef(int.Parse(NWScript.Get2DAString("spells", "SpellDesc", spellId)))}";
          oScroll.AddItemProperty(API.ItemProperty.CastSpell((IPCastSpell)itemPropertyId, IPCastSpellNumUses.SingleUse), EffectDuration.Permanent);
        }
      }

      ChatPlugin.SendMessage(ChatPlugin.NWNX_CHAT_CHANNEL_PLAYER_TALK, "Pour obtenir votre amulette de concentration de l'arcane, il vous faut vous enregistrer auprès du juge.", magicshop, player.oid);
      shop.Open(player.oid);
    }
  }
}
