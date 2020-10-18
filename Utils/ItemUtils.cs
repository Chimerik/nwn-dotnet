using NWN.Core;

namespace NWN
{
  public static class ItemUtils
  {
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

      switch(itemType)
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
  }
}
