using System.Linq;
using Anvil.API;
using NWN.System;
using static NWN.Systems.PlayerSystem;

namespace NWN.Systems
{
  class Tanneur
  {
    private readonly BaseItemType[] leatherBasicWeaponBlueprints = new BaseItemType[] { BaseItemType.Belt, BaseItemType.Gloves, BaseItemType.Boots, BaseItemType.Cloak, BaseItemType.Whip };
    private readonly int[] leatherBasicArmorBlueprints = new int[] { 0, 1, 2, 3 };
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

        foreach (BaseItemType baseItemType in leatherBasicWeaponBlueprints)
        {
          NwItem oBlueprint = await NwItem.Create("blueprintgeneric", shop, 1, "blueprint");
          ItemUtils.CreateShopWeaponBlueprint(oBlueprint, baseItemType);
        }

        foreach (int baseArmor in leatherBasicArmorBlueprints)
        {
          NwItem oBlueprint = await NwItem.Create("blueprintgeneric", shop);
          ItemUtils.CreateShopArmorBlueprint(oBlueprint, baseArmor);
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
