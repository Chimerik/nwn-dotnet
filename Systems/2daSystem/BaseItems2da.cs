using System;
using System.Collections.Generic;
using NWN.API;
using NWN.API.Constants;
using NWN.Services;

namespace NWN.Systems
{
  public class BaseItemTable : ITwoDimArray
  {
    private readonly Dictionary<BaseItemType, Entry> entries = new Dictionary<BaseItemType, Entry>();

    public Entry GetBaseItemDataEntry(BaseItemType baseItem)
    {
      return entries[baseItem];
    }
    public int[] GetDamageDices(BaseItemType baseItem)
    {
      Entry damageEntry = entries[baseItem];
      return new int[] { damageEntry.damageDice, damageEntry.numDamageDice };
    }

    void ITwoDimArray.DeserializeRow(int rowIndex, TwoDimEntry twoDimEntry)
    {
      uint strRef = uint.TryParse(twoDimEntry("Name"), out strRef) ? strRef : 0;
      string name;
      if (strRef == 0)
        name = "Nom manquant";
      else
        name = BaseItems2da.tlkTable.GetSimpleString(strRef);

      strRef = uint.TryParse(twoDimEntry("Description"), out strRef) ? strRef : 0;
      string description;
      if (strRef == 0)
        description = "Nom manquant";
      else
        description = BaseItems2da.tlkTable.GetSimpleString(strRef);

      string workshop = twoDimEntry("Category");
      string craftedItem = twoDimEntry("label");

      int numDamageDice = int.TryParse(twoDimEntry("NumDice"), out numDamageDice) ? numDamageDice : 0;
      int damageDice = int.TryParse(twoDimEntry("DieToRoll"), out damageDice) ? damageDice : 0;
      float baseCost = float.TryParse(twoDimEntry("BaseCost"), out baseCost) ? baseCost : 1;

      string equippableSlot = twoDimEntry("EquipableSlots");
      bool IsEquippable = true;
      if (equippableSlot == "0x00000")
        IsEquippable = false;

      int range = int.TryParse(twoDimEntry("RangedWeapon"), out range) ? range : 0;
      int damageType = int.TryParse(twoDimEntry("WeaponType"), out damageType) ? damageType : 0;

      entries.Add((BaseItemType)rowIndex, new Entry(name, description, numDamageDice, damageDice, baseCost, workshop, craftedItem, IsEquippable, damageDice > 0, range > 0, damageDice > 0 && range == 0, damageType));
    }
    public readonly struct Entry
    {
      public readonly string name;
      public readonly string description;
      public readonly int numDamageDice;
      public readonly int damageDice;
      public readonly int damageType;
      public readonly float baseCost;
      public readonly string workshop;
      public readonly string craftedItem;
      public readonly bool IsEquippable;
      public readonly bool IsWeapon;
      public readonly bool IsRangedWeapon;
      public readonly bool IsMeleeWeapon;

      public Entry(string name, string description, int numDamageDice, int damageDice, float baseCost, string workshop, string craftedItem, bool IsEquippable, bool IsWeapon, bool IsRangedWeapon, bool IsMeleeWeapon, int damageType)
      {
        this.name = name;
        this.description = description;
        this.numDamageDice = numDamageDice;
        this.damageDice = damageDice;
        this.baseCost = baseCost;
        this.workshop = workshop;
        this.craftedItem = craftedItem;
        this.IsEquippable = IsEquippable;
        this.IsWeapon = IsWeapon;
        this.IsMeleeWeapon = IsMeleeWeapon;
        this.IsRangedWeapon = IsRangedWeapon;
        this.damageType = damageType;
      }
    }
  }

  [ServiceBinding(typeof(BaseItems2da))]
  public class BaseItems2da
  {
    public static TlkTable tlkTable;
    public static BaseItemTable baseItemTable;
    public BaseItems2da(TwoDimArrayFactory twoDimArrayFactory, TlkTable tlkService)
    {
      tlkTable = tlkService;
      baseItemTable = twoDimArrayFactory.Get2DA<BaseItemTable>("baseitems");
    }
  }
}
