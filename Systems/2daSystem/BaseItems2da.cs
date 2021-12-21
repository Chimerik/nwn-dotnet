using System;
using System.Collections.Generic;
using System.Linq;

using Anvil.API;
using Anvil.Services;

using NWN.Core;

namespace NWN.Systems
{
  public class BaseItemTable : ITwoDimArray
  {
    private readonly Dictionary<BaseItemType, Entry> entries = new Dictionary<BaseItemType, Entry>();
    public List<NuiComboEntry> helmetModelEntries = new List<NuiComboEntry>();
    public Dictionary<string, List<int>> simpleItemModels = new Dictionary<string, List<int>>();

    public Entry GetBaseItemDataEntry(BaseItemType baseItem)
    {
      return entries[baseItem];
    }
    public List<NuiComboEntry> GetWeaponModelList(BaseItemType baseItem, ItemAppearanceWeaponModel part)
    {
      List<NuiComboEntry> comboEntries = new List<NuiComboEntry>();

      foreach (byte model in entries[baseItem].weaponModels[part])
        comboEntries.Add(new NuiComboEntry(model.ToString(), model));

      return comboEntries;
    }
    void ITwoDimArray.DeserializeRow(int rowIndex, TwoDimEntry twoDimEntry)
    {
      uint strRef = uint.TryParse(twoDimEntry("Name"), out strRef) ? strRef : 0;

      if (strRef < 1)
        return;

      string workshop = twoDimEntry("Category");
      string craftedItem = twoDimEntry("label");
      bool IsEquippable = twoDimEntry("EquipableSlots") == "0x00000" ? IsEquippable = false : IsEquippable = true;
      string resRef = twoDimEntry("ItemClass");

      Dictionary<ItemAppearanceWeaponModel, List<byte>> weaponModels = new Dictionary<ItemAppearanceWeaponModel, List<byte>>();

      if (!string.IsNullOrEmpty(resRef))
      {
        weaponModels.Add(ItemAppearanceWeaponModel.Top, new List<byte>());
        weaponModels.Add(ItemAppearanceWeaponModel.Middle, new List<byte>());
        weaponModels.Add(ItemAppearanceWeaponModel.Bottom, new List<byte>());

        for (byte i = 10; i < 255; i++)
        {
          string search = i.ToString().PadLeft(3, '0');
          
          if (NWScript.ResManGetAliasFor($"{resRef}_t_{search}", NWScript.RESTYPE_MDL) != "")
            weaponModels[ItemAppearanceWeaponModel.Top].Add(i);

          if (NWScript.ResManGetAliasFor($"{resRef}_m_{search}", NWScript.RESTYPE_MDL) != "")
            weaponModels[ItemAppearanceWeaponModel.Middle].Add(i);

          if (NWScript.ResManGetAliasFor($"{resRef}_b_{search}", NWScript.RESTYPE_MDL) != "")
            weaponModels[ItemAppearanceWeaponModel.Bottom].Add(i);
        }
      }

      entries.Add((BaseItemType)rowIndex, new Entry(workshop, craftedItem, weaponModels));
    }
    public readonly struct Entry
    {
      public readonly string workshop;
      public readonly string craftedItem;
      public readonly Dictionary<ItemAppearanceWeaponModel, List<byte>> weaponModels;

      public Entry(string workshop, string craftedItem, Dictionary<ItemAppearanceWeaponModel, List<byte>> weaponModels)
      {
        this.workshop = workshop;
        this.craftedItem = craftedItem;
        this.weaponModels = weaponModels;
      } 
    }
  }

  [ServiceBinding(typeof(BaseItems2da))]
  public class BaseItems2da
  {
    public static TlkTable tlkTable;
    public static BaseItemTable baseItemTable;
    public BaseItems2da(TwoDimArrayFactory twoDimArrayFactory, TlkTable tlkService, ResourceManager resMan)
    {
      tlkTable = tlkService;
      baseItemTable = twoDimArrayFactory.Get2DA<BaseItemTable>("baseitems");
      
      baseItemTable.simpleItemModels.Add("iashlw", new List<int>());
      baseItemTable.simpleItemModels.Add("iit_neck", new List<int>());
      baseItemTable.simpleItemModels.Add("iit_belt", new List<int>());
      baseItemTable.simpleItemModels.Add("iit_boots", new List<int>());
      baseItemTable.simpleItemModels.Add("iit_bracer", new List<int>());
      baseItemTable.simpleItemModels.Add("iit_gloves", new List<int>());
      baseItemTable.simpleItemModels.Add("iit_potion", new List<int>());
      baseItemTable.simpleItemModels.Add("iashsw", new List<int>());
      baseItemTable.simpleItemModels.Add("iashto", new List<int>());

      for (int i = 1; i < 255; i++)
      {
        string search = i.ToString().PadLeft(3, '0');
        
        if (resMan.IsValidResource($"helm_{search}", ResRefType.MDL))
          baseItemTable.helmetModelEntries.Add(new NuiComboEntry(i.ToString(), i));

        if (resMan.IsValidResource($"iashlw_{search}", ResRefType.TGA))
          baseItemTable.simpleItemModels["iashlw"].Add(i);

        if (resMan.IsValidResource($"iit_neck_{search}", ResRefType.TGA))
          baseItemTable.simpleItemModels["iit_neck"].Add(i);

        if (resMan.IsValidResource($"iit_belt_{search}", ResRefType.TGA))
          baseItemTable.simpleItemModels["iit_belt"].Add(i);

        if (resMan.IsValidResource($"iit_boots_{search}", ResRefType.TGA))
          baseItemTable.simpleItemModels["iit_boots"].Add(i);

        if (resMan.IsValidResource($"iit_bracer_{search}", ResRefType.TGA))
          baseItemTable.simpleItemModels["iit_bracer"].Add(i);

        if (resMan.IsValidResource($"iit_gloves_{search}", ResRefType.TGA))
          baseItemTable.simpleItemModels["iit_gloves"].Add(i);

        if (resMan.IsValidResource($"iit_potion_{search}", ResRefType.TGA))
          baseItemTable.simpleItemModels["iit_potion"].Add(i);

        if (resMan.IsValidResource($"iashsw_{search}", ResRefType.TGA))
          baseItemTable.simpleItemModels["iashsw"].Add(i);

        if (resMan.IsValidResource($"iashto_{search}", ResRefType.TGA))
          baseItemTable.simpleItemModels["iashto"].Add(i);
      }
    }
  }
}
