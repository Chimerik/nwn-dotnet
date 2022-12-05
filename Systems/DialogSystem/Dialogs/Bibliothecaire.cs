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
      if (bibliothecaire.Area.GetObjectVariable<DateTimeLocalVariable>("NEXT_TRIGGER_DATE").HasValue
        && bibliothecaire.Area.GetObjectVariable<DateTimeLocalVariable>("NEXT_TRIGGER_DATE").Value > DateTime.Now)
        return;

      NwStore shop = bibliothecaire.GetNearestObjectsByType<NwStore>().Where(s => s.Tag == "bibliothecaire_shop").FirstOrDefault();

      if (shop == null)
        shop = NwStore.Create("generic_shop_res", bibliothecaire.Location, false, "bibliothecaire_shop");

      shop.GetObjectVariable<LocalVariableObject<NwCreature>>("_STORE_NPC").Value = bibliothecaire;

      if (Utils.random.Next(1, 101) < 21)
      {
        var languages = SkillSystem.learnableDictionary.Where(l => l.Value is LearnableSkill skill && skill.category == SkillSystem.Category.Language);
        int randomLanguage = languages.ElementAt(Utils.random.Next(0, languages.Count())).Key;

        NwItem skillBook = await NwItem.Create("skillbookgeneriq", shop, 1, "skillbook");
        ItemUtils.CreateShopSkillBook(skillBook, randomLanguage);
        skillBook.BaseGoldValue = 3000;
      }
      else
      {
        NwItem skillBook = await NwItem.Create("skillbookgeneriq", shop, 1, "ruined_book");
        skillBook.Appearance.SetSimpleModel((byte)Utils.random.Next(0, 50));
        skillBook.Name = "Ouvrage ruiné";
        skillBook.Description = "Cet ouvrage est abîmé au-delà de toute rédemption. Il est même trop humide pour faire du feu.\n\n Il est fort probable qu'il se désagrège entre vos doigts si vous tentez de l'ouvrir.";
        skillBook.BaseGoldValue = 3000;
        //skillBook.GetObjectVariable<LocalVariableString>("ITEM_KEY").Value = Config.itemKey;
      }

      shop.OnOpen += StoreSystem.OnOpenBiblioStore;
      shop.Open(player.oid);

      bibliothecaire.Area.GetObjectVariable<DateTimeLocalVariable>("NEXT_TRIGGER_DATE").Value = DateTime.Now.AddHours(4);
    }
  }
}
