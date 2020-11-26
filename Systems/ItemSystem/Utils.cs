using System;
using NWN.Core;

namespace NWN.Systems
{
  public static partial class ItemSystem
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
    }

    public static ItemCategory GetItemCategory(int baseItemType)
    {
      switch (baseItemType)
      {
        case NWScript.BASE_ITEM_ARMOR:
          return ItemCategory.Armor;
        case NWScript.BASE_ITEM_SMALLSHIELD:
        case NWScript.BASE_ITEM_TOWERSHIELD:
        case NWScript.BASE_ITEM_LARGESHIELD:
          return ItemCategory.Shield;
        case NWScript.BASE_ITEM_DOUBLEAXE:
        case NWScript.BASE_ITEM_GREATAXE:
        case NWScript.BASE_ITEM_GREATSWORD:
        case NWScript.BASE_ITEM_HALBERD:
        case NWScript.BASE_ITEM_HEAVYFLAIL:
        case NWScript.BASE_ITEM_QUARTERSTAFF:
        case NWScript.BASE_ITEM_SCYTHE:
        case NWScript.BASE_ITEM_TWOBLADEDSWORD:
        case NWScript.BASE_ITEM_DIREMACE:
        case NWScript.BASE_ITEM_TRIDENT:
        case NWScript.BASE_ITEM_SHORTSPEAR:
          return ItemCategory.TwoHandedMeleeWeapon;
        case NWScript.BASE_ITEM_BASTARDSWORD:
        case NWScript.BASE_ITEM_LONGSWORD:
        case NWScript.BASE_ITEM_BATTLEAXE:
        case NWScript.BASE_ITEM_CLUB:
        case NWScript.BASE_ITEM_DAGGER:
        case NWScript.BASE_ITEM_DWARVENWARAXE:
        case NWScript.BASE_ITEM_HANDAXE:
        case NWScript.BASE_ITEM_KAMA:
        case NWScript.BASE_ITEM_KATANA:
        case NWScript.BASE_ITEM_KUKRI:
        case NWScript.BASE_ITEM_LIGHTFLAIL:
        case NWScript.BASE_ITEM_LIGHTHAMMER:
        case NWScript.BASE_ITEM_LIGHTMACE:
        case NWScript.BASE_ITEM_MORNINGSTAR:
        case NWScript.BASE_ITEM_RAPIER:
        case NWScript.BASE_ITEM_SHORTSWORD:
        case NWScript.BASE_ITEM_SCIMITAR:
        case NWScript.BASE_ITEM_SICKLE:
        case NWScript.BASE_ITEM_WARHAMMER:
        case NWScript.BASE_ITEM_WHIP:
          return ItemCategory.OneHandedMeleeWeapon;
        case NWScript.BASE_ITEM_HEAVYCROSSBOW:
        case NWScript.BASE_ITEM_LIGHTCROSSBOW:
        case NWScript.BASE_ITEM_SHORTBOW:
        case NWScript.BASE_ITEM_LONGBOW:
        case NWScript.BASE_ITEM_DART:
        case NWScript.BASE_ITEM_SLING:
        case NWScript.BASE_ITEM_THROWINGAXE:
          return ItemCategory.RangedWeapon;
        case NWScript.BASE_ITEM_ARROW:
        case NWScript.BASE_ITEM_BOLT:
        case NWScript.BASE_ITEM_BULLET:
          return ItemCategory.Ammunition;
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
      ItemProperty ip = NWScript.GetFirstItemProperty(oItem);

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
  }
}
