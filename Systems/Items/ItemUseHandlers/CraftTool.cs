using Anvil.API;

namespace NWN.Systems
{
  class CraftTool
  {
    public CraftTool(NwCreature oPC, NwItem item)
    {
      if (item.Possessor != oPC) // TODO : vérifier qu'il est bien le crafteur de l'objet
      {
        oPC.ControllingPlayer.SendServerMessage($"Vous devez être en possession de l'objet {item.Name.ColorString(ColorConstants.Lime)} pour pouvoir le modifier", ColorConstants.Orange);
        return;
      }

      // TODO : ajouter un métier permettant de modifier n'importe quelle tenue
      if (item.GetObjectVariable<LocalVariableString>("_ORIGINAL_CRAFTER_NAME").HasValue && item.GetObjectVariable<LocalVariableString>("_ORIGINAL_CRAFTER_NAME").Value != oPC.Name 
        && !oPC.ControllingPlayer.IsDM)
      {
        oPC.ControllingPlayer.SendServerMessage($"Il est indiqué : Pour tout modification, s'adresser à {item.GetObjectVariable<LocalVariableString>("_ORIGINAL_CRAFTER_NAME").Value.ColorString(ColorConstants.White)}", ColorConstants.Red);
        return;
      }

      if (!PlayerSystem.Players.TryGetValue(oPC, out PlayerSystem.Player player))
        return;

      switch (item.BaseItem.ItemType)
      {
        case BaseItemType.Armor:
          if (player.windows.ContainsKey("itemAppearanceModifier"))
            ((PlayerSystem.Player.ArmorAppearanceWindow)player.windows["itemAppearanceModifier"]).CreateWindow(item);
          else
            player.windows.Add("itemAppearanceModifier", new PlayerSystem.Player.ArmorAppearanceWindow(player, item));
          break;
        case BaseItemType.Bastardsword:
        case BaseItemType.Battleaxe:
        case BaseItemType.Club:
        case BaseItemType.Dagger:
        case BaseItemType.DireMace:
        case BaseItemType.Doubleaxe:
        case BaseItemType.DwarvenWaraxe:
        case BaseItemType.Greataxe:
        case BaseItemType.Greatsword:
        case BaseItemType.Halberd:
        case BaseItemType.Handaxe:
        case BaseItemType.HeavyCrossbow:
        case BaseItemType.HeavyFlail:
        case BaseItemType.Kama:
        case BaseItemType.Katana:
        case BaseItemType.Kukri:
        case BaseItemType.LightCrossbow:
        case BaseItemType.LightFlail:
        case BaseItemType.LightHammer:
        case BaseItemType.LightMace:
        case BaseItemType.Longbow:
        case BaseItemType.Longsword:
        case BaseItemType.MagicStaff:
        case BaseItemType.Morningstar:
        case BaseItemType.Quarterstaff:
        case BaseItemType.Rapier:
        case BaseItemType.Scimitar:
        case BaseItemType.Scythe:
        case BaseItemType.Shortbow:
        case BaseItemType.ShortSpear:
        case BaseItemType.Shortsword:
        case BaseItemType.Sickle:
        case BaseItemType.Sling:
        case BaseItemType.ThrowingAxe:
        case BaseItemType.Trident:
        case BaseItemType.TwoBladedSword:
        case BaseItemType.Warhammer:
        case BaseItemType.Whip:
          if (player.windows.ContainsKey("weaponAppearanceModifier"))
            ((PlayerSystem.Player.WeaponAppearanceWindow)player.windows["weaponAppearanceModifier"]).CreateWindow(item);
          else
            player.windows.Add("weaponAppearanceModifier", new PlayerSystem.Player.WeaponAppearanceWindow(player, item));
          break;
        case BaseItemType.Amulet:
        case BaseItemType.Arrow:
        case BaseItemType.Belt:
        case BaseItemType.Bolt:
        case BaseItemType.Book:
        case BaseItemType.Boots:
        case BaseItemType.Bracer:
        case BaseItemType.Bullet:
        case BaseItemType.EnchantedPotion:
        case BaseItemType.EnchantedScroll:
        case BaseItemType.EnchantedWand:
        case BaseItemType.Gloves:
        case BaseItemType.Grenade:
        case BaseItemType.LargeShield:
        case BaseItemType.MagicRod:
        case BaseItemType.MagicWand:
        case BaseItemType.Potions:
        case BaseItemType.Ring:
        case BaseItemType.Scroll:
        case BaseItemType.Shuriken:
        case BaseItemType.SmallShield:
        case BaseItemType.SpellScroll:
        case BaseItemType.TowerShield:
        case BaseItemType.TrapKit:
          if (player.windows.ContainsKey("simpleItemAppearanceModifier"))
            ((PlayerSystem.Player.SimpleItemAppearanceWindow)player.windows["simpleItemAppearanceModifier"]).CreateWindow(item);
          else
            player.windows.Add("simpleItemAppearanceModifier", new PlayerSystem.Player.SimpleItemAppearanceWindow(player, item));
          break;
        case BaseItemType.Helmet:
          if (player.windows.ContainsKey("helmetAppearanceModifier"))
            ((PlayerSystem.Player.HelmetAppearanceWindow)player.windows["helmetAppearanceModifier"]).CreateWindow(item);
          else
            player.windows.Add("helmetAppearanceModifier", new PlayerSystem.Player.HelmetAppearanceWindow(player, item));
          break;
        case BaseItemType.Cloak:
          if (player.windows.ContainsKey("cloakAppearanceModifier"))
            ((PlayerSystem.Player.CloakAppearanceWindow)player.windows["cloakAppearanceModifier"]).CreateWindow(item);
          else
            player.windows.Add("cloakAppearanceModifier", new PlayerSystem.Player.CloakAppearanceWindow(player, item));
          break;
      }
    }
  }
}
