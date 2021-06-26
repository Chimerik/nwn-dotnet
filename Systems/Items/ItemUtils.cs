﻿using System;
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
    // returns TRUE if item can be equipped. Uses Get2DAString, so do not use in a loop!
    // ----------------------------------------------------------------------------
    public static Boolean GetIsItemEquipable(uint oItem)
    {
      int nBaseType = NWScript.GetBaseItemType(oItem);

      // fix, if we get BASE_ITEM_INVALID (usually because oItem is invalid), we
      // need to make sure that this function returns FALSE
      if (nBaseType == NWScript.BASE_ITEM_INVALID) return false;

      string sResult = NWScript.Get2DAString("baseitems", "EquipableSlots", nBaseType);
      return (sResult != "0x00000");
    }
    // ----------------------------------------------------------------------------
    // Removes all itemproperties with matching nItemPropertyType and
    // nItemPropertyDuration (a DURATION_TYPE_* constant)
    // ----------------------------------------------------------------------------
    public static void RemoveMatchingItemProperties(uint oItem, int nItemPropertyType, int nItemPropertyDuration = NWScript.DURATION_TYPE_TEMPORARY, int nItemPropertySubType = -1)
    {
      Core.ItemProperty ip = NWScript.GetFirstItemProperty(oItem);

      // valid ip?
      while (NWScript.GetIsItemPropertyValid(ip) == 1)
      {
        // same property type?
        if (NWScript.GetItemPropertyType(ip) == nItemPropertyType)
        {
          // same duration or duration ignored?
          if (NWScript.GetItemPropertyDurationType(ip) == nItemPropertyDuration || nItemPropertyDuration == -1)
          {
            // same subtype or subtype ignored
            if (NWScript.GetItemPropertySubType(ip) == nItemPropertySubType || nItemPropertySubType == -1)
            {
              // Put a warning into the logfile if someone tries to remove a permanent ip with a temporary one!
              /*if (nItemPropertyDuration == DURATION_TYPE_TEMPORARY &&  GetItemPropertyDurationType(ip) == DURATION_TYPE_PERMANENT)
              {
                 WriteTimestampedLogEntry("x2_inc_itemprop:: IPRemoveMatchingItemProperties() - WARNING: Permanent item property removed by temporary on "+GetTag(oItem));
              }
              */
              NWScript.RemoveItemProperty(oItem, ip);
            }
          }
        }
        ip = NWScript.GetNextItemProperty(oItem);
      }
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
    public static bool IsEquipable(uint oItem)
    {
      int nBaseType = NWScript.GetBaseItemType(oItem);

      // fix, if we get BASE_ITEM_INVALID (usually because oItem is invalid), we
      // need to make sure that this function returns FALSE
      if (nBaseType == NWScript.BASE_ITEM_INVALID) return false;

      string sResult = NWScript.Get2DAString("baseitems", "EquipableSlots", nBaseType);
      return (sResult != "0x00000");
    }

    public static bool IsWeapon(uint oItem)
    {
      return NWScript.GetWeaponRanged(oItem) == 1 || IsMeleeWeapon(oItem);
    }

    public static bool IsMeleeWeapon(uint oItem)
    {
      var itemType = NWScript.GetBaseItemType(oItem);

      switch (itemType)
      {
        default: return false;

        case NWScript.BASE_ITEM_BASTARDSWORD:
        case NWScript.BASE_ITEM_BATTLEAXE:
        case NWScript.BASE_ITEM_DOUBLEAXE:
        case NWScript.BASE_ITEM_GREATAXE:
        case NWScript.BASE_ITEM_GREATSWORD:
        case NWScript.BASE_ITEM_HALBERD:
        case NWScript.BASE_ITEM_HANDAXE:
        case NWScript.BASE_ITEM_KAMA:
        case NWScript.BASE_ITEM_KATANA:
        case NWScript.BASE_ITEM_KUKRI:
        case NWScript.BASE_ITEM_LONGSWORD:
        case NWScript.BASE_ITEM_SCIMITAR:
        case NWScript.BASE_ITEM_SCYTHE:
        case NWScript.BASE_ITEM_SICKLE:
        case NWScript.BASE_ITEM_TWOBLADEDSWORD:
        case NWScript.BASE_ITEM_CLUB:
        case NWScript.BASE_ITEM_DAGGER:
        case NWScript.BASE_ITEM_DIREMACE:
        case NWScript.BASE_ITEM_HEAVYFLAIL:
        case NWScript.BASE_ITEM_LIGHTFLAIL:
        case NWScript.BASE_ITEM_LIGHTHAMMER:
        case NWScript.BASE_ITEM_LIGHTMACE:
        case NWScript.BASE_ITEM_MORNINGSTAR:
        case NWScript.BASE_ITEM_QUARTERSTAFF:
        case NWScript.BASE_ITEM_RAPIER:
        case NWScript.BASE_ITEM_SHORTSPEAR:
        case NWScript.BASE_ITEM_SHORTSWORD:
        case NWScript.BASE_ITEM_WARHAMMER:
        case NWScript.BASE_ITEM_WHIP:
        case NWScript.BASE_ITEM_DWARVENWARAXE:
        case NWScript.BASE_ITEM_MAGICSTAFF:
        case NWScript.BASE_ITEM_TRIDENT:
          return true;
      }
    }

    public static int GetIdentifiedGoldPieceValue(uint oItem)
    {
      var isIdentified = NWScript.GetIdentified(oItem);
      if (isIdentified == 0) NWScript.SetIdentified(oItem, 1);
      int nGP = NWScript.GetGoldPieceValue(oItem);

      // Re-set the identification flag to its original if it has been changed.
      if (isIdentified == 0) NWScript.SetIdentified(oItem, isIdentified);
      return nGP;
    }

    public static int GetItemPropertyBonus(uint oItem, int ipType, int ipSubType = -1)
    {
      var ip = NWScript.GetFirstItemProperty(oItem);
      int nPropBonus = 0;

      while (nPropBonus == 0 && NWScript.GetIsItemPropertyValid(ip) == 1)
      {
        if (NWScript.GetItemPropertyType(ip) == (int)ipType)
        {
          if (ipSubType != -1)
          {
            // If a subType has been given
            if (NWScript.GetItemPropertySubType(ip) == ipSubType)
            {
              nPropBonus = NWScript.GetItemPropertyCostTableValue(ip);
            }
          }
          else
          {
            // If no subType
            nPropBonus = NWScript.GetItemPropertyCostTableValue(ip);
          }
        }
        ip = NWScript.GetNextItemProperty(oItem);
      }

      return nPropBonus;
    }
    public static int GetBaseItemCost(NwItem item)
    {
      BaseItemType baseItemType = item.BaseItemType;
      int baseCost;

      if (baseItemType == BaseItemType.CreatureItem)
        return 999999;

      if (baseItemType == BaseItemType.Armor)
      {
        if (!int.TryParse(NWScript.Get2DAString("armor", "COST", item.BaseACValue), out baseCost))
        {
          Utils.LogMessageToDMs($"{item.Name} - baseCost introuvable pour baseItemType : {baseItemType}");
          return 999999;
        }
      }
      else
      {
        if (!int.TryParse(NWScript.Get2DAString("baseitems", "BaseCost", (int)baseItemType), out baseCost))
        {
          Utils.LogMessageToDMs($"{item.Name} - baseCost introuvable pour baseItemType : {baseItemType}");
          return 999999;
        }
      }

      return baseCost;
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
