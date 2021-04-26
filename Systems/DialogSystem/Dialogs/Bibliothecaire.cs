using System;
using System.Linq;
using System.Threading.Tasks;
using NWN.API;
using NWN.Core;
using NWN.Core.NWNX;
using NWN.System;
using static NWN.Systems.PlayerSystem;

namespace NWN.Systems
{
  class Bibliothecaire
  {
    public Bibliothecaire(Player player, NwCreature bibliothecaire)
    {
      if(bibliothecaire.Area.GetLocalVariable<int>("_CANT_TRIGGER").HasNothing)
      {
        bibliothecaire.GetLocalVariable<int>("_CANT_TRIGGER").Value = 1;

        NwStore shop = bibliothecaire.GetNearestObjectsByType<NwStore>().Where(s => s.Tag == "bibliothecaire_shop").FirstOrDefault();

        if (shop == null)
          shop = NwStore.Create("generic_shop_res", bibliothecaire.Location, false, "bibliothecaire_shop");

        NWScript.SetLocalObject(shop, "_STORE_NPC", bibliothecaire);

        if (NWN.Utils.random.Next(1, 101) < 21)
        {
          int feat = (int)SkillSystem.languageSkillBooks[NWN.Utils.random.Next(0, SkillSystem.languageSkillBooks.Length)];
          NwItem skillBook = NwItem.Create("skillbookgeneriq", shop, 1 , "skillbook");
          ItemPlugin.SetItemAppearance(skillBook, NWScript.ITEM_APPR_TYPE_SIMPLE_MODEL, 2, NWN.Utils.random.Next(0, 50));
          skillBook.GetLocalVariable<int>("_SKILL_ID").Value = feat;

          int value;
          if (int.TryParse(NWScript.Get2DAString("feat", "FEAT", feat), out value))
            skillBook.Name = NWScript.GetStringByStrRef(value);

          if (int.TryParse(NWScript.Get2DAString("feat", "DESCRIPTION", feat), out value))
            skillBook.Description = NWScript.GetStringByStrRef(value);

          ItemPlugin.SetBaseGoldPieceValue(skillBook, 3000);
        }
        else
        {
          NwItem skillBook = NwItem.Create("skillbookgeneriq", shop, 1, "ruined_book");
          ItemPlugin.SetItemAppearance(skillBook, NWScript.ITEM_APPR_TYPE_SIMPLE_MODEL, 2, NWN.Utils.random.Next(0, 50));
          skillBook.Name = "Ouvrage ruiné";
          skillBook.Description = "Cet ouvrage est abîmé au-delà de toute rédemption. Il est même trop humide pour faire du feu.\n\n Il est fort probable qu'il se désagrège entre vos doigts si vous tentez de l'ouvrir.";
          ItemPlugin.SetBaseGoldPieceValue(skillBook, 3000);
        }

        shop.OnOpen += StoreSystem.OnOpenBiblioStore;
        shop.Open(player.oid);

        Task task = NwTask.Run(async () =>
        {
          await NwTask.Delay(TimeSpan.FromHours(4));
          bibliothecaire.Area.GetLocalVariable<int>("_CANT_TRIGGER").Delete();
          return true;
        });
      }
    }
  }
}
