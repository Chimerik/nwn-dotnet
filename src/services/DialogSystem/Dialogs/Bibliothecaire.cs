using System;
using System.Linq;
using System.Threading.Tasks;
using Anvil.API;
using NWN.System;
using static NWN.Systems.PlayerSystem;

namespace NWN.Systems
{
  class Bibliothecaire
  {
    public Bibliothecaire(Player player, NwCreature bibliothecaire)
    {
      HandleBiblitohecaire(player, bibliothecaire);
    }
    private async void HandleBiblitohecaire(Player player, NwCreature bibliothecaire)
    { 
      if(bibliothecaire.Area.GetObjectVariable<LocalVariableInt>("_CANT_TRIGGER").HasNothing)
      {
        bibliothecaire.GetObjectVariable<LocalVariableInt>("_CANT_TRIGGER").Value = 1;

        NwStore shop = bibliothecaire.GetNearestObjectsByType<NwStore>().Where(s => s.Tag == "bibliothecaire_shop").FirstOrDefault();

        if (shop == null)
          shop = NwStore.Create("generic_shop_res", bibliothecaire.Location, false, "bibliothecaire_shop");

        shop.GetObjectVariable<LocalVariableObject<NwCreature>>("_STORE_NPC").Value = bibliothecaire;

        if (Utils.random.Next(1, 101) < 21)
        {
          Feat feat = SkillSystem.languageSkillBooks[Utils.random.Next(0, SkillSystem.languageSkillBooks.Length)];
          NwItem skillBook = await NwItem.Create("skillbookgeneriq", shop, 1 , "skillbook");
          skillBook.Appearance.SetSimpleModel((byte)Utils.random.Next(0, 50));
          skillBook.GetObjectVariable<LocalVariableInt>("_SKILL_ID").Value = (int)feat;

          FeatTable.Entry featEntry = Feat2da.featTable.GetFeatDataEntry(feat);
          skillBook.Name = featEntry.name;
          skillBook.Description = featEntry.description;
          skillBook.BaseGoldValue = 3000;
        }
        else
        {
          NwItem skillBook = await NwItem.Create("skillbookgeneriq", shop, 1, "ruined_book");
          skillBook.Appearance.SetSimpleModel((byte)Utils.random.Next(0, 50));
          skillBook.Name = "Ouvrage ruiné";
          skillBook.Description = "Cet ouvrage est abîmé au-delà de toute rédemption. Il est même trop humide pour faire du feu.\n\n Il est fort probable qu'il se désagrège entre vos doigts si vous tentez de l'ouvrir.";
          skillBook.BaseGoldValue = 3000;
        }

        shop.OnOpen += StoreSystem.OnOpenBiblioStore;
        shop.Open(player.oid);

        Task task = NwTask.Run(async () =>
        {
          await NwTask.Delay(TimeSpan.FromHours(4));
          bibliothecaire.Area.GetObjectVariable<LocalVariableInt>("_CANT_TRIGGER").Delete();
          return true;
        });
      }
    }
  }
}
