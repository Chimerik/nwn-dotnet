using System.Linq;
using Anvil.API;
using NWN.System;
using static NWN.Systems.PlayerSystem;

namespace NWN.Systems
{
  class Tanneur
  {
    public Tanneur(Player player, NwCreature tanneur)
    {
      HandleTanneur(player, tanneur);
    }
    private async void HandleTanneur(Player player, NwCreature tanneur)
    {
      NwStore shop = tanneur.GetNearestObjectsByType<NwStore>().Where(s => s.Tag == "tannery_shop").FirstOrDefault();

      if (shop == null)
      {
        shop = NwStore.Create("generic_shop_res", tanneur.Location, false, "tannery_shop");
        shop.GetObjectVariable<LocalVariableObject<NwCreature>>("_STORE_NPC").Value = tanneur;

        foreach (int baseItemType in Craft.Collect.System.leatherBasicBlueprints)
        {
          NwItem oBlueprint = await NwItem.Create("blueprintgeneric", shop, 1, "blueprint");
          ItemUtils.CreateShopBlueprint(oBlueprint, baseItemType);
        }

        foreach (Feat feat in SkillSystem.leatherBasicSkillBooks)
        {
          NwItem skillBook = await NwItem.Create("skillbookgeneriq", shop, 1, "skillbook");
          ItemUtils.CreateShopSkillBook(skillBook, (int)feat);
        }

        NwItem craftTool = await NwItem.Create("oreextractor", shop, 1, "oreextractor");
        craftTool.BaseGoldValue = 50;
        craftTool.GetObjectVariable<LocalVariableInt>("_DURABILITY").Value = 10;
        craftTool.GetObjectVariable<LocalVariableString>("ITEM_KEY").Value = Config.itemKey;

        craftTool = await NwItem.Create("forgehammer", shop, 1, "forgehammer");
        craftTool.BaseGoldValue = 50;
        craftTool.GetObjectVariable<LocalVariableInt>("_DURABILITY").Value = 5;
        craftTool.GetObjectVariable<LocalVariableString>("ITEM_KEY").Value = Config.itemKey;
      }

      shop.OnOpen += StoreSystem.OnOpenGenericStore;
      shop.Open(player.oid);
    }
  }
}
