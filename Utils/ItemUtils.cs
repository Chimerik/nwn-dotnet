using NWN.Enums.Item;

namespace NWN
{
  public static class ItemUtils
  {
    public static bool IsEquipable(uint oItem)
    {
      int nBaseType = NWScript.GetBaseItemType(oItem);

      // fix, if we get BASE_ITEM_INVALID (usually because oItem is invalid), we
      // need to make sure that this function returns FALSE
      if (nBaseType == (int)BaseItem.Invalid) return false;

      string sResult = NWScript.Get2DAString("baseitems", "EquipableSlots", nBaseType);
      return (sResult != "0x00000");
    }

    public static bool IsWeapon(uint oItem)
    {
      return NWScript.GetWeaponRanged(oItem) || IsMeleeWeapon(oItem);
    }

    public static bool IsMeleeWeapon(uint oItem)
    {
      var itemType = NWScript.GetBaseItemType(oItem);

      switch(itemType)
      {
        default: return false;

        case (int)BaseItem.BastardSword:
        case (int)BaseItem.BattleAxe:
        case (int)BaseItem.DoubleAxe:
        case (int)BaseItem.GreatAxe:
        case (int)BaseItem.GreatSword:
        case (int)BaseItem.Halberd:
        case (int)BaseItem.HandAxe:
        case (int)BaseItem.Kama:
        case (int)BaseItem.Katana:
        case (int)BaseItem.Kukri:
        case (int)BaseItem.Longsword:
        case (int)BaseItem.Scimitar:
        case (int)BaseItem.Scythe:
        case (int)BaseItem.Sickle:
        case (int)BaseItem.TwoBladedSword:
        case (int)BaseItem.Club:
        case (int)BaseItem.Dagger:
        case (int)BaseItem.DireMace:
        case (int)BaseItem.HeavyFlail:
        case (int)BaseItem.LightFlail:
        case (int)BaseItem.LightHammer:
        case (int)BaseItem.LightMace:
        case (int)BaseItem.MorningStar:
        case (int)BaseItem.QuarterStaff:
        case (int)BaseItem.Rapier:
        case (int)BaseItem.ShortSpear:
        case (int)BaseItem.ShortSword:
        case (int)BaseItem.WarHammer:
        case (int)BaseItem.Whip:
        case (int)BaseItem.DwarvenWarAxe:
        case (int)BaseItem.MagicStaff:
        case (int)BaseItem.Trident:
          return true;
      }
    }

    public static int GetIdentifiedGoldPieceValue(uint oItem)
    {
      var isIdentified = NWScript.GetIdentified(oItem);
      if (!isIdentified) NWScript.SetIdentified(oItem, true);
      int nGP = NWScript.GetGoldPieceValue(oItem);

      // Re-set the identification flag to its original if it has been changed.
      if (!isIdentified) NWScript.SetIdentified(oItem, isIdentified);
      return nGP;
    }

    public static int GetItemPropertyBonus(uint oItem, ItemPropertyType ipType, int ipSubType = -1)
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
