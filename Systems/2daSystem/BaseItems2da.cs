using System.Collections.Generic;
using System.Linq;

using Anvil.API;
using Anvil.Services;

using NWN.Core;

namespace NWN.Systems
{
  public sealed class BaseItemEntry : ITwoDimArrayEntry
  {
    public string name { get; private set; }
    public string workshop { get; private set; }
    public string resRef { get; private set; }
    public string craftedItem { get; private set; }
    public int craftLearnable { get; private set; }
    public double cost { get; private set; }
    public int RowIndex { get; init; }

    public void InterpretEntry(TwoDimArrayEntry entry)
    {
      workshop = entry.GetString("Category");
      craftedItem = entry.GetString("label");
      resRef = entry.GetString("ItemClass");
      name = entry.GetStrRef("Name").HasValue && entry.GetInt("Name") > 0 ?  entry.GetStrRef("Name").Value.ToParsedString() : "";
      craftLearnable = entry.GetInt("ILRStackSize").GetValueOrDefault(-1);
      cost = entry.GetInt("BaseCost").GetValueOrDefault(-1);
    }
  }
  [ServiceBinding(typeof(BaseItems2da))]
  public class BaseItems2da
  {
    public static readonly TwoDimArray<BaseItemEntry> baseItemTable = NwGameTables.GetTable<BaseItemEntry>("baseitems.2da");
    public static List<NuiComboEntry> helmetModelEntries = new List<NuiComboEntry>();
    public static List<NuiComboEntry> baseItemNameEntries = new List<NuiComboEntry>();
    public static Dictionary<string, List<int>> simpleItemModels = new Dictionary<string, List<int>>();
    public BaseItems2da(ResourceManager resMan)
    {
      int count = 0;
      foreach (var entry in baseItemTable)
      {
        if (!string.IsNullOrEmpty(entry.name))
          baseItemNameEntries.Add(new NuiComboEntry(entry.name.ToString(), count));

        count++;
      }

      baseItemNameEntries = baseItemNameEntries.OrderBy(b => b.Label).ToList();

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
  }
}
