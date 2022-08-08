﻿using System.Collections.Generic;

using Anvil.API;
using Anvil.Services;

using NWN.Core;

namespace NWN.Systems
{
  public sealed class BaseItemEntry : ITwoDimArrayEntry
  {
    public string workshop { get; private set; }
    public string craftedItem { get; private set; }
    public int craftLearnable { get; private set; }
    public double cost { get; private set; }
    public Dictionary<ItemAppearanceWeaponModel, List<byte>> weaponModels { get; private set; }

    // RowIndex is already populated externally, and we do not need to assign it in InterpretEntry.
    public int RowIndex { get; init; }

    public void InterpretEntry(TwoDimArrayEntry entry)
    {
      string workshop = entry.GetString("Category");
      string craftedItem = entry.GetString("label");
      string resRef = entry.GetString("ItemClass");
      int craftLearnable = entry.GetInt("ILRStackSize").GetValueOrDefault(-1);
      double cost = entry.GetInt("BaseCost").GetValueOrDefault(-1);

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
    }
  }
  [ServiceBinding(typeof(BaseItems2da))]
  public class BaseItems2da
  {
    public static readonly TwoDimArray<BaseItemEntry> baseItemTable = NwGameTables.GetTable<BaseItemEntry>("baseitems.2da");
    public static List<NuiComboEntry> helmetModelEntries = new List<NuiComboEntry>();
    public static Dictionary<string, List<int>> simpleItemModels = new Dictionary<string, List<int>>();
    public BaseItems2da(ResourceManager resMan)
    {     
      simpleItemModels.Add("iashlw", new List<int>());
      simpleItemModels.Add("iit_neck", new List<int>());
      simpleItemModels.Add("iit_belt", new List<int>());
      simpleItemModels.Add("iit_boots", new List<int>());
      simpleItemModels.Add("iit_bracer", new List<int>());
      simpleItemModels.Add("iit_gloves", new List<int>());
      simpleItemModels.Add("iit_potion", new List<int>());
      simpleItemModels.Add("iashsw", new List<int>());
      simpleItemModels.Add("iashto", new List<int>());

      for (int i = 1; i < 255; i++)
      {
        string search = i.ToString().PadLeft(3, '0');
        
        if (resMan.IsValidResource($"helm_{search}", ResRefType.MDL))
          helmetModelEntries.Add(new NuiComboEntry(i.ToString(), i));

        if (resMan.IsValidResource($"iashlw_{search}", ResRefType.TGA))
          simpleItemModels["iashlw"].Add(i);

        if (resMan.IsValidResource($"iit_neck_{search}", ResRefType.TGA))
          simpleItemModels["iit_neck"].Add(i);

        if (resMan.IsValidResource($"iit_belt_{search}", ResRefType.TGA))
          simpleItemModels["iit_belt"].Add(i);

        if (resMan.IsValidResource($"iit_boots_{search}", ResRefType.TGA))
          simpleItemModels["iit_boots"].Add(i);

        if (resMan.IsValidResource($"iit_bracer_{search}", ResRefType.TGA))
          simpleItemModels["iit_bracer"].Add(i);

        if (resMan.IsValidResource($"iit_gloves_{search}", ResRefType.TGA))
          simpleItemModels["iit_gloves"].Add(i);

        if (resMan.IsValidResource($"iit_potion_{search}", ResRefType.TGA))
          simpleItemModels["iit_potion"].Add(i);

        if (resMan.IsValidResource($"iashsw_{search}", ResRefType.TGA))
          simpleItemModels["iashsw"].Add(i);

        if (resMan.IsValidResource($"iashto_{search}", ResRefType.TGA))
          simpleItemModels["iashto"].Add(i);
      }
    }
    public static List<NuiComboEntry> GetWeaponModelList(BaseItemType baseItem, ItemAppearanceWeaponModel part)
    {
      List<NuiComboEntry> comboEntries = new List<NuiComboEntry>();

      foreach (byte model in baseItemTable[(int)baseItem].weaponModels[part])
        comboEntries.Add(new NuiComboEntry(model.ToString(), model));

      return comboEntries;
    }
  }
}
