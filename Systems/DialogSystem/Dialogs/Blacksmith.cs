using System.Linq;
using Anvil.API;
using NWN.System;
using static NWN.Systems.PlayerSystem;

namespace NWN.Systems
{
  class Blacksmith
  {
    private readonly BaseItemType[] forgeBasicWeaponBlueprints = new BaseItemType[] { BaseItemType.LightMace, BaseItemType.Helmet, BaseItemType.Dagger, BaseItemType.Morningstar, BaseItemType.ShortSpear, BaseItemType.Sickle, BaseItemType.LightHammer, BaseItemType.LightFlail, BaseItemType.Bracer };
    private readonly int[] forgeBasicArmorBlueprints = new int[] { 4 };
    public Blacksmith(Player player, NwCreature blacksmith)
    {
      HandleBlacksmith(player, blacksmith);
    }
    private async void HandleBlacksmith(Player player, NwCreature blacksmith)
    { 
      NwStore shop = blacksmith.GetNearestObjectsByType<NwStore>().FirstOrDefault(s => s.Tag == "blacksmith_shop");
      
      if (shop == null)
      {
        shop = NwStore.Create("generic_shop_res", blacksmith.Location, false, "blacksmith_shop");
        shop.GetObjectVariable<LocalVariableObject<NwCreature>>("_STORE_NPC").Value = blacksmith;

        foreach (BaseItemType baseItemType in forgeBasicWeaponBlueprints)
        {
          NwItem oBlueprint = await NwItem.Create("blueprintgeneric", shop);
          ItemUtils.CreateShopWeaponBlueprint(oBlueprint, NwBaseItem.FromItemType(baseItemType));
        }

        foreach (int baseArmor in forgeBasicArmorBlueprints)
        {
          NwItem oBlueprint = await NwItem.Create("blueprintgeneric", shop);
          ItemUtils.CreateShopArmorBlueprint(oBlueprint, baseArmor);
        }

        foreach (int customSkill in SkillSystem.forgeBasicSkillBooks)
        {
          NwItem skillBook = await NwItem.Create("skillbookgeneriq", shop, 1, "skillbook");
          ItemUtils.CreateShopSkillBook(skillBook, customSkill);
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
