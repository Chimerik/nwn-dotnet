﻿using System.Linq;

using Anvil.API;

using NWN.System;

using static NWN.Systems.PlayerSystem;

namespace NWN.Systems
{
  class Alchemist
  {
    Player player;
    NwCreature alchemist;
    public Alchemist(Player player, NwCreature alchemist)
    {
      this.player = player;
      this.alchemist = alchemist;

      HandleAlchemist();
    }
    private async void HandleAlchemist()
    {
      NwStore shop = alchemist.GetNearestObjectsByType<NwStore>().FirstOrDefault(s => s.Tag == "alchemist_shop");

      if (shop == null)
      {
        shop = NwStore.Create("generic_shop_res", alchemist.Location, false, "alchemist_shop");
        shop.GetObjectVariable<LocalVariableObject<NwCreature>>("_STORE_NPC").Value = alchemist;

        foreach (Feat feat in SkillSystem.alchemyBasicSkillBooks)
        {
          NwItem skillBook = await NwItem.Create("skillbookgeneriq", shop, 1, "skillbook");
          skillBook.Appearance.SetSimpleModel((byte)Utils.random.Next(0, 50));
          skillBook.GetObjectVariable<LocalVariableInt>("_SKILL_ID").Value = (int)feat;

          FeatTable.Entry featEntry = Feat2da.featTable.GetFeatDataEntry(feat);

          if (SkillSystem.customFeatsDictionnary.ContainsKey(feat))
          {
            skillBook.Name = SkillSystem.customFeatsDictionnary[feat].name;
            skillBook.Description = SkillSystem.customFeatsDictionnary[feat].description;
          }
          else
          {

            skillBook.Name = featEntry.name;
            skillBook.Description = featEntry.description;
          }

          skillBook.BaseGoldValue = (uint)(featEntry.CRValue * 1000);
        }

        NwItem craftTool = await NwItem.Create("NW_IT_MPOTION006", shop);
        craftTool.BaseGoldValue = 450;
      }

      shop.OnOpen += StoreSystem.OnOpenGenericStore;
      shop.Open(player.oid);
    }
  }
}
