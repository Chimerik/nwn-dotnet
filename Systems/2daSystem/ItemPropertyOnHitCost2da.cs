using System.Collections.Generic;
using Anvil.API;
using NWN.Core;
using Anvil.Services;

namespace NWN.Systems
{
  public class IpOnHitCostTable : ITwoDimArray
  {
    private readonly Dictionary<int, Entry> entries = new Dictionary<int, Entry>();

    public Entry GetIPOnHitCostDataEntry(int rowIndex)
    {
      return entries[rowIndex];
    }
    public string GetCostName(int rowIndex)
    {
      return entries[rowIndex].name;
    }
    void ITwoDimArray.DeserializeRow(int rowIndex, TwoDimEntry twoDimEntry)
    {
      uint strRef = uint.TryParse(twoDimEntry("Name"), out strRef) ? strRef : 0;
      string name = strRef == 0 ? name = "Nom manquant" : name = ItemPropertyOnHitCost2da.tlkTable.GetSimpleString(strRef);

      entries.Add(rowIndex, new Entry(name));
    }
    public readonly struct Entry
    {
      public readonly string name;

      public Entry(string name)
      {
        this.name = name;
      }
    }
  }

  [ServiceBinding(typeof(ItemPropertyOnHitCost2da))]
  public class ItemPropertyOnHitCost2da
  {
    public static TlkTable tlkTable;
    public static IpOnHitCostTable ipOnHitCostTable;
    public ItemPropertyOnHitCost2da(TwoDimArrayFactory twoDimArrayFactory, TlkTable tlkService)
    {
      tlkTable = tlkService;
      ipOnHitCostTable = twoDimArrayFactory.Get2DA<IpOnHitCostTable>("iprp_onhitcost");
    }
  }
}
