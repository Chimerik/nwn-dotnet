using System.Linq;
using NWN.API;
using NWN.API.Constants;
using NWN.Core;
using NWN.Core.NWNX;
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
        shop.GetLocalVariable<NwObject>("_STORE_NPC").Value = tanneur;

        foreach (int baseItemType in Craft.Collect.System.leatherBasicBlueprints)
        {
          Craft.Blueprint blueprint = new Craft.Blueprint(baseItemType);

          if (!Craft.Collect.System.blueprintDictionnary.ContainsKey(baseItemType))
            Craft.Collect.System.blueprintDictionnary.Add(baseItemType, blueprint);

          NwItem oBlueprint = await NwItem.Create("blueprintgeneric", shop, 1, "blueprint");
          oBlueprint.Name = $"Patron original : {blueprint.name}";
          oBlueprint.GetLocalVariable<int>("_BASE_ITEM_TYPE").Value = baseItemType;
          oBlueprint.BaseGoldValue = (uint)(blueprint.goldCost * 10);
        }

        foreach (Feat feat in SkillSystem.leatherBasicSkillBooks)
        {
          NwItem skillBook = await NwItem.Create("skillbookgeneriq", shop, 1, "skillbook");
          skillBook.Appearance.SetSimpleModel((byte)Utils.random.Next(0, 50));
          skillBook.GetLocalVariable<int>("_SKILL_ID").Value = (int)feat;

          FeatTable.Entry entry = Feat2da.featTable.GetFeatDataEntry(feat);

          if (SkillSystem.customFeatsDictionnary.ContainsKey(feat))
          {
            skillBook.Name = SkillSystem.customFeatsDictionnary[feat].name;
            skillBook.Description = SkillSystem.customFeatsDictionnary[feat].description;
          }
          else
          {
            skillBook.Name = entry.name;
            skillBook.Description = entry.description;
          }

          skillBook.BaseGoldValue = (uint)(entry.CRValue * 1000);
        }

        NwItem craftTool = await NwItem.Create("oreextractor", shop, 1, "oreextractor");
        craftTool.BaseGoldValue = 50;
        craftTool.GetLocalVariable<int>("_DURABILITY").Value = 10;

        craftTool = await NwItem.Create("forgehammer", shop, 1, "forgehammer");
        craftTool.BaseGoldValue = 50;
        craftTool.GetLocalVariable<int>("_DURABILITY").Value = 5;
      }

      shop.OnOpen += StoreSystem.OnOpenGenericStore;
      shop.Open(player.oid);
    }
  }
}
