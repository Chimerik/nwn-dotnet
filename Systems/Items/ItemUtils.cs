using System;
using System.Collections.Generic;
using System.Linq;
using NWN.API;
using NWN.API.Constants;
using NWN.Core;
using NWN.Core.NWNX;

namespace NWN.Systems
{
  public static class ItemUtils
  {
    public enum ItemCategory
    {
      Invalid = -1,
      OneHandedMeleeWeapon = 0,
      TwoHandedMeleeWeapon = 1,
      RangedWeapon = 2,
      Shield = 3,
      Armor = 4,
      Ammunition = 5,
      CraftTool = 6,
      Potions = 7,
      Scroll = 8,
      Clothes = 9,
    }

    public static ItemCategory GetItemCategory(BaseItemType baseItemType)
    {
      switch (baseItemType)
      {
        case BaseItemType.Armor:
        case BaseItemType.Helmet:
          return ItemCategory.Armor;
        case BaseItemType.SmallShield:
        case BaseItemType.TowerShield:
        case BaseItemType.LargeShield:
          return ItemCategory.Shield;
        case BaseItemType.Doubleaxe:
        case BaseItemType.Greataxe:
        case BaseItemType.Greatsword:
        case BaseItemType.Halberd:
        case BaseItemType.HeavyFlail:
        case BaseItemType.Quarterstaff:
        case BaseItemType.Scythe:
        case BaseItemType.TwoBladedSword:
        case BaseItemType.DireMace:
        case BaseItemType.Trident:
        case BaseItemType.ShortSpear:
          return ItemCategory.TwoHandedMeleeWeapon;
        case BaseItemType.Bastardsword:
        case BaseItemType.Longsword:
        case BaseItemType.Battleaxe:
        case BaseItemType.Club:
        case BaseItemType.Dagger:
        case BaseItemType.DwarvenWaraxe:
        case BaseItemType.Handaxe:
        case BaseItemType.Kama:
        case BaseItemType.Katana:
        case BaseItemType.Kukri:
        case BaseItemType.LightFlail:
        case BaseItemType.LightHammer:
        case BaseItemType.LightMace:
        case BaseItemType.Morningstar:
        case BaseItemType.Rapier:
        case BaseItemType.Shortsword:
        case BaseItemType.Scimitar:
        case BaseItemType.Sickle:
        case BaseItemType.Warhammer:
        case BaseItemType.Whip:
          return ItemCategory.OneHandedMeleeWeapon;
        case BaseItemType.HeavyCrossbow:
        case BaseItemType.LightCrossbow:
        case BaseItemType.Shortbow:
        case BaseItemType.Longbow:
        case BaseItemType.Dart:
        case BaseItemType.Sling:
        case BaseItemType.ThrowingAxe:
          return ItemCategory.RangedWeapon;
        case BaseItemType.Arrow:
        case BaseItemType.Bolt:
        case BaseItemType.Bullet:
          return ItemCategory.Ammunition;
        case BaseItemType.Potions:
        case BaseItemType.BlankPotion:
        case BaseItemType.EnchantedPotion:
          return ItemCategory.Potions;
        case BaseItemType.Scroll:
        case BaseItemType.BlankScroll:
        case BaseItemType.EnchantedScroll:
        case BaseItemType.SpellScroll:
          return ItemCategory.Scroll;
        case BaseItemType.Belt:
        case BaseItemType.Boots:
        case BaseItemType.Bracer:
        case BaseItemType.Cloak:
        case BaseItemType.Gloves:
          return ItemCategory.Clothes;
        case (BaseItemType)114: //marteau de forgeron
        case (BaseItemType)115: //extracteur de minerai
          return ItemCategory.CraftTool;
        default:
          return ItemCategory.Invalid;
      }
    }
    // ----------------------------------------------------------------------------
    // Removes all itemproperties with matching nItemPropertyType and
    // nItemPropertyDuration (a DURATION_TYPE_* constant)
    // ----------------------------------------------------------------------------
    public static void RemoveMatchingItemProperties(NwItem oItem, ItemPropertyType nItemPropertyType, EffectDuration nItemPropertyDuration = EffectDuration.Temporary, int nItemPropertySubType = -1)
    {
      foreach(ItemProperty ip in oItem.ItemProperties.Where(i => i.PropertyType == nItemPropertyType && (i.DurationType == nItemPropertyDuration || nItemPropertyDuration == (EffectDuration)(-1)) && (i.SubType == nItemPropertySubType || nItemPropertySubType == -1)))
        oItem.RemoveItemProperty(ip);
    }
    public static void DecreaseItemDurability(NwItem oItem)
    {
      if (oItem == null)
        return;

      int itemDurability = oItem.GetLocalVariable<int>("_DURABILITY").Value; 
      if (itemDurability <= 1)
        oItem.Destroy();
      else
        oItem.GetLocalVariable<int>("_DURABILITY").Value -= 1;
    }
    public static int GetIdentifiedGoldPieceValue(NwItem oItem)
    {
      bool isIdentified = oItem.Identified;
      if (isIdentified) oItem.Identified = true;
        int nGP = oItem.GoldValue;

      // Re-set the identification flag to its original if it has been changed.
      if (isIdentified) oItem.Identified = false;
      return nGP;
    }

    public static int GetItemPropertyBonus(NwItem oItem, ItemPropertyType ipType, int ipSubType = -1)
    {
      List<ItemProperty> sortedIP;

      if (ipSubType > -1)
        sortedIP = oItem.ItemProperties.Where(i => i.PropertyType == ipType && i.SubType == ipSubType).ToList();
      else
        sortedIP = oItem.ItemProperties.Where(i => i.PropertyType == ipType).ToList();

      ItemProperty maxIp = sortedIP.OrderByDescending(i => i.CostTableValue).FirstOrDefault();
      int nPropBonus = 0;

      if (maxIp != null)
        nPropBonus = maxIp.CostTableValue;

      return nPropBonus;
    }
    public static int GetBaseItemCost(NwItem item)
    {
      float baseCost = 9999999;

      if (item.BaseItemType == BaseItemType.Armor)
        baseCost = Armor2da.armorTable.GetDataEntry(item.BaseACValue).cost;
      else
        baseCost = BaseItems2da.baseItemTable.GetBaseItemDataEntry(item.BaseItemType).baseCost;

      if (baseCost <= 0)
      {
        Utils.LogMessageToDMs($"{item.Name} - baseCost introuvable pour baseItemType : {item.BaseItemType}");
        return 999999;
      }

      return (int)baseCost;
    }
    public static string GetItemDurabilityState(NwItem item)
    {
      int durabilityState = item.GetLocalVariable<int>("_DURABILITY").Value / item.GetLocalVariable<int>("_MAX_DURABILITY").Value * 100;

      if (durabilityState == 100)
        return "Flambant neuf".ColorString(new Color(32, 255, 32));
      else if (durabilityState < 100 && durabilityState >= 75)
        return "Très bon état".ColorString(ColorConstants.Green);
      else if (durabilityState < 75 && durabilityState >= 50)
        return "Bon état".ColorString(ColorConstants.Red);
      else if (durabilityState < 50 && durabilityState >= 25)
        return "Usé".ColorString(ColorConstants.Lime);
      else if (durabilityState < 25 && durabilityState >= 5)
        return "Abimé".ColorString(ColorConstants.Orange);
      else if (durabilityState < 5 && durabilityState >= 1)
        return "Vétuste".ColorString(ColorConstants.Red);
      else if (durabilityState < 1)
        return "Ruiné".ColorString(ColorConstants.Red);

      return "";
    }
    public static int GetItemDamageType(NwItem item)
    {
      if (!int.TryParse(NWScript.Get2DAString("baseitems", "WeaponType", (int)item.BaseItemType), out int damageType))
        return 3; // par défaut, on retourne slashing
      return damageType;
    }
    public static DamageType GetDamageTypeFromItemProperty(IPDamageType ipDamageType)
    {
      switch(ipDamageType)
      {
        case IPDamageType.Bludgeoning: return DamageType.Bludgeoning;
        case IPDamageType.Piercing: return DamageType.Piercing;
        case IPDamageType.Slashing: return DamageType.Slashing;
        case IPDamageType.Acid: return DamageType.Acid;
        case IPDamageType.Magical: return DamageType.Magical;
        case IPDamageType.Fire: return DamageType.Fire;
        case IPDamageType.Cold: return DamageType.Cold;
        case IPDamageType.Electrical: return DamageType.Electrical;
        case IPDamageType.Divine: return DamageType.Divine;
        case IPDamageType.Negative: return DamageType.Negative;
        case IPDamageType.Positive: return DamageType.Positive;
        case IPDamageType.Sonic: return DamageType.Sonic;
        case (IPDamageType)4: return (DamageType)8192; // Physical
        case (IPDamageType)14: return (DamageType)16384; // Elemental
        default: return DamageType.Slashing;
      }
    }
  }
}
