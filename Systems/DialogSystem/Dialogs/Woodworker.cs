using System.Linq;
using Anvil.API;
using NWN.System;
using static NWN.Systems.PlayerSystem;

namespace NWN.Systems
{
  class Woodworker
  {
    private readonly  BaseItemType[] woodBasicBlueprints = new BaseItemType[] { BaseItemType.SmallShield, BaseItemType.Club, BaseItemType.Dart, BaseItemType.Bullet, BaseItemType.HeavyCrossbow, BaseItemType.LightCrossbow, BaseItemType.Quarterstaff, BaseItemType.Sling, BaseItemType.Arrow, BaseItemType.Bolt };
    public Woodworker(Player player, NwCreature blacksmith)
    {
      HandleWoodworker(player, blacksmith);
    }
    private async void HandleWoodworker(Player player, NwCreature blacksmith)
    { 
      NwStore shop = blacksmith.GetNearestObjectsByType<NwStore>().Where(s => s.Tag == "woodworker_shop").FirstOrDefault();

      if (shop == null)
      {
        shop = NwStore.Create("generic_shop_res", blacksmith.Location, false, "woodworker_shop");
        shop.GetObjectVariable<LocalVariableObject<NwCreature>>("_STORE_NPC").Value = blacksmith;

        foreach (BaseItemType baseItemType in woodBasicBlueprints)
        {
          NwItem oBlueprint = await NwItem.Create("blueprintgeneric", shop, 1, "blueprint");
          ItemUtils.CreateShopWeaponBlueprint(oBlueprint, baseItemType);
        }

        foreach (Feat feat in SkillSystem.woodBasicSkillBooks)
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
