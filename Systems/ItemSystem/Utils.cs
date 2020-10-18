using System;
using System.Collections.Generic;
using System.Text;
using NWN.Enums.Item;

namespace NWN.Systems
{
  public static partial class ItemSystem
  {
      public enum ItemCategory
      {
        Invalid = 0,
        OneHandedMeleeWeapon = 1,
        TwoHandedMeleeWeapon = 2,
        Shield = 3,
        Armor = 4,
      }

      public static List<BaseItem> twoHandedMeleeWeaponList = new List<BaseItem>() { BaseItem.DoubleAxe, BaseItem.GreatAxe,
      BaseItem.GreatSword, BaseItem.Halberd, BaseItem.HeavyFlail, BaseItem.QuarterStaff, BaseItem.Scythe, BaseItem.TwoBladedSword,
      BaseItem.DireMace, BaseItem.Trident, BaseItem.ShortSpear};

      public static List<BaseItem> oneHandedMeleeWeaponList = new List<BaseItem>() { BaseItem.BastardSword, BaseItem.BattleAxe,
      BaseItem.Club, BaseItem.Dagger, BaseItem.DwarvenWarAxe, BaseItem.HandAxe, BaseItem.Kama, BaseItem.Katana,
      BaseItem.Kukri, BaseItem.LightFlail, BaseItem.LightHammer, BaseItem.LightMace, BaseItem.Longsword,
      BaseItem.MorningStar, BaseItem.Rapier, BaseItem.Scimitar, BaseItem.ShortSword, BaseItem.Sickle,
      BaseItem.WarHammer, BaseItem.Whip};

      public static List<BaseItem> shieldList = new List<BaseItem>() { BaseItem.SmallShield, BaseItem.LargeShield,
      BaseItem.TowerShield};
      public static ItemCategory GetItemCategory(uint oItem)
      {
        if (oItem.AsItem().BaseItemType == BaseItem.Armor)
          return ItemCategory.Armor;
        if (shieldList.Contains(oItem.AsItem().BaseItemType))
          return ItemCategory.Shield;
        if (oneHandedMeleeWeaponList.Contains(oItem.AsItem().BaseItemType))
          return ItemCategory.OneHandedMeleeWeapon;
        if (twoHandedMeleeWeaponList.Contains(oItem.AsItem().BaseItemType))
          return ItemCategory.TwoHandedMeleeWeapon;

        return ItemCategory.Invalid;
      }
  }
}
