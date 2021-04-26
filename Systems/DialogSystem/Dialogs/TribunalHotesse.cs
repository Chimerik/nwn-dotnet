using System.Linq;
using NWN.API;
using NWN.Core;
using NWN.Core.NWNX;
using NWN.API.Constants;
using static NWN.Systems.PlayerSystem;
using NWN.System;

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

        foreach(Feat feat in SkillSystem.shopBasicMagicSkillBooks)
        {
          NwItem skillBook = NwItem.Create("skillbookgeneriq", shop, 1, "skillbook");
          ItemPlugin.SetItemAppearance(skillBook, NWScript.ITEM_APPR_TYPE_SIMPLE_MODEL, 2, Utils.random.Next(0, 50));
          skillBook.GetLocalVariable<int>("_SKILL_ID").Value = (int)feat;

          if (SkillSystem.customFeatsDictionnary.ContainsKey(feat))
          {
            skillBook.Name = SkillSystem.customFeatsDictionnary[feat].name;
            skillBook.Description = SkillSystem.customFeatsDictionnary[feat].description;
          }
          else
          {
            if (int.TryParse(NWScript.Get2DAString("feat", "FEAT", (int)feat), out int nameValue))
              skillBook.Name = NWScript.GetStringByStrRef(nameValue);

            if (int.TryParse(NWScript.Get2DAString("feat", "DESCRIPTION", (int)feat), out int descriptionValue))
              skillBook.Description = NWScript.GetStringByStrRef(descriptionValue);
          }

          if (int.TryParse(NWScript.Get2DAString("feat", "CRValue", (int)feat), out int crValue))
            ItemPlugin.SetBaseGoldPieceValue(skillBook, crValue * 1000);
        }
      }

      ChatPlugin.SendMessage(ChatPlugin.NWNX_CHAT_CHANNEL_PLAYER_TALK, "Pour obtenir votre amulette de concentration de l'arcane, il vous faut vous enregistrer auprès du juge.", magicshop, player.oid);
      shop.OnOpen += StoreSystem.OnOpenGenericStore;
      shop.Open(player.oid);
    }
  }
}
