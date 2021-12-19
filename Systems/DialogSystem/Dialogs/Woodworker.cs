using System.Linq;
using Anvil.API;
using NWN.System;
using static NWN.Systems.PlayerSystem;

namespace NWN.Systems
{
  class Woodworker
  {
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

        foreach (int baseItemType in Craft.Collect.System.woodBasicBlueprints)
        {
          Craft.Blueprint blueprint = new Craft.Blueprint(baseItemType);

          if (!Craft.Collect.System.blueprintDictionnary.ContainsKey(baseItemType))
            Craft.Collect.System.blueprintDictionnary.Add(baseItemType, blueprint);

          NwItem oBlueprint = await NwItem.Create("blueprintgeneric", shop, 1, "blueprint");
          oBlueprint.Name = $"Patron original : {blueprint.name}";
          oBlueprint.GetObjectVariable<LocalVariableInt>("_BASE_ITEM_TYPE").Value = baseItemType;
          oBlueprint.BaseGoldValue = (uint)(blueprint.goldCost * 10);
        }

        foreach (Feat feat in SkillSystem.woodBasicSkillBooks)
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
            skillBook.Name = learnable.description;
          }

          skillBook.BaseGoldValue = (uint)(learnable.multiplier * 1000);
        }

        NwItem craftTool = await NwItem.Create("oreextractor", shop, 1, "oreextractor");
        craftTool.BaseGoldValue = 50;
        craftTool.GetObjectVariable<LocalVariableInt>("_DURABILITY").Value = 10;

        craftTool = await NwItem.Create("forgehammer", shop, 1, "forgehammer");
        craftTool.BaseGoldValue = 50;
        craftTool.GetObjectVariable<LocalVariableInt>("_DURABILITY").Value = 5;
      }

      shop.OnOpen += StoreSystem.OnOpenGenericStore;
      shop.Open(player.oid);
    }
  }
}
