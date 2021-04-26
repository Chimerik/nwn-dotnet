using System.Linq;
using NWN.API;
using NWN.API.Constants;
using NWN.Core;
using NWN.Core.NWNX;
using NWN.System;
using static NWN.Systems.PlayerSystem;

namespace NWN.Systems
{
  class Blacksmith
  {
    public Blacksmith(Player player, NwCreature blacksmith)
    {
      NwStore shop = blacksmith.GetNearestObjectsByType<NwStore>().FirstOrDefault(s => s.Tag == "blacksmith_shop");
      
      if (shop == null)
      {
        shop = NwStore.Create("generic_shop_res", blacksmith.Location, false, "blacksmith_shop");
        NWScript.SetLocalObject(shop, "_STORE_NPC", blacksmith);

        foreach (int baseItemType in Craft.Collect.System.forgeBasicBlueprints)
        {
          Craft.Blueprint blueprint = new Craft.Blueprint(baseItemType);

          if (!Craft.Collect.System.blueprintDictionnary.ContainsKey(baseItemType))
            Craft.Collect.System.blueprintDictionnary.Add(baseItemType, blueprint);

          NwItem oBlueprint = NwItem.Create("blueprintgeneric", shop);
          oBlueprint.Name = $"Patron original : {blueprint.name}";

          oBlueprint.GetLocalVariable<int>("_BASE_ITEM_TYPE").Value = baseItemType;
          ItemPlugin.SetBaseGoldPieceValue(oBlueprint, blueprint.goldCost * 10);
        }

        foreach (Feat feat in SkillSystem.forgeBasicSkillBooks)
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

        NwItem craftTool = NwItem.Create("oreextractor", shop, 1, "oreextractor");
        ItemPlugin.SetBaseGoldPieceValue(craftTool, 50);
        craftTool.GetLocalVariable<int>("_DURABILITY").Value = 10;

        craftTool = NwItem.Create("forgehammer", shop, 1, "forgehammer");
        ItemPlugin.SetBaseGoldPieceValue(craftTool, 50);
        craftTool.GetLocalVariable<int>("_DURABILITY").Value = 5;
      }

      shop.OnOpen += StoreSystem.OnOpenGenericStore;
      shop.Open(player.oid);
    }
  }
}
