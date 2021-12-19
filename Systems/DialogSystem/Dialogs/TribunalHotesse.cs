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
          Spell spell = ItemPropertySpells2da.spellsTable.GetSpellDataEntry(itemPropertyId).spell;
          NwSpell nwSpell = NwSpell.FromSpellType(spell);
          oScroll.Name = nwSpell.Name;
          oScroll.Description = nwSpell.Description;
          oScroll.AddItemProperty(ItemProperty.CastSpell((IPCastSpell)itemPropertyId, IPCastSpellNumUses.SingleUse), EffectDuration.Permanent);
        }

        foreach(Feat feat in SkillSystem.shopBasicMagicSkillBooks)
        {
          NwItem skillBook = await NwItem.Create("skillbookgeneriq", shop, 1, "skillbook");
          skillBook.Appearance.SetSimpleModel((byte)Utils.random.Next(0, 50));
          skillBook.GetObjectVariable<LocalVariableInt>("_SKILL_ID").Value = (int)feat;

          Learnable learnable = SkillSystem.learnableDictionary[(int)feat];

          if (SkillSystem.customFeatsDictionnary.ContainsKey(feat))
          {
            skillBook.Name = SkillSystem.customFeatsDictionnary[feat].name;
            skillBook.Description = SkillSystem.customFeatsDictionnary[feat].description;
          }
          else
          {
            skillBook.Name = learnable.name;
            skillBook.Description = learnable.description;
          }

          skillBook.BaseGoldValue = (uint)(learnable.multiplier * 1000);
        }
      }

      ChatSystem.chatService.SendMessage(ChatChannel.PlayerTalk, "Pour obtenir votre amulette de concentration de l'arcane, il vous faut vous enregistrer auprès du juge.", magicshop, player.oid);
      shop.OnOpen += StoreSystem.OnOpenGenericStore;
      shop.Open(player.oid);
    }
  }
}
