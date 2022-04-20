using System.Linq;
using Anvil.API;
using static NWN.Systems.PlayerSystem;
using NWN.System;
using Anvil.Services;

namespace NWN.Systems
{
  class TribunalHotesse
  {
    public TribunalHotesse(Player player, NwCreature magicshop)
    {
      HandleHotesse(player, magicshop);
    }
    private async void HandleHotesse(Player player, NwCreature magicshop)
    { 
      NwStore shop = magicshop.GetNearestObjectsByType<NwStore>().Where(s => s.Tag == "magic_shop").FirstOrDefault();

      if (shop == null)
      {
        shop = NwStore.Create("generic_shop_res", magicshop.Location, false, "magic_shop");
        shop.GetObjectVariable<LocalVariableObject<NwCreature>>("_STORE_NPC").Value = magicshop;

        foreach (int itemPropertyId in SkillSystem.shopBasicMagicScrolls)
        {
          NwItem oScroll = await NwItem.Create("spellscroll", shop, 1, "scroll");
          Spell spell = ItemPropertySpells2da.ipSpellTable[itemPropertyId].spell;
          NwSpell nwSpell = NwSpell.FromSpellType(spell);
          oScroll.Name = nwSpell.Name.ToString();
          oScroll.Description = nwSpell.Description.ToString();
          oScroll.AddItemProperty(ItemProperty.CastSpell((IPCastSpell)itemPropertyId, IPCastSpellNumUses.SingleUse), EffectDuration.Permanent);
          oScroll.GetObjectVariable<LocalVariableString>("ITEM_KEY").Value = Config.itemKey;
        }

        foreach(Feat feat in SkillSystem.shopBasicMagicSkillBooks)
        {
          NwItem skillBook = await NwItem.Create("skillbookgeneriq", shop, 1, "skillbook");
          ItemUtils.CreateShopSkillBook(skillBook, (int)feat);
        }
      }

      ChatSystem.chatService.SendMessage(ChatChannel.PlayerTalk, "Pour obtenir votre amulette de concentration de l'arcane, il vous faut vous enregistrer auprès du juge.", magicshop, player.oid);
      shop.OnOpen += StoreSystem.OnOpenGenericStore;
      shop.Open(player.oid);
    }
  }
}
