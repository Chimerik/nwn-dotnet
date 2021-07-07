using System.Collections.Generic;
using NWN.API;
using NWN.API.Constants;
using NWN.Core;
using NWN.Services;

namespace NWN.Systems
{
  public class IpDefinitionTable : ITwoDimArray
  {
    private readonly Dictionary<ItemPropertyType, Entry> entries = new Dictionary<ItemPropertyType, Entry>();

    public Entry GetIPDefinitionlDataEntry(ItemPropertyType ipType)
    {
      return entries[ipType];
    }
    public string GetSubTypeName(ItemPropertyType ipType, int subType)
    {
      return NWScript.Get2DAString(entries[ipType].subTypeResRef, "Name", subType);
    }
    void ITwoDimArray.DeserializeRow(int rowIndex, TwoDimEntry twoDimEntry)
    {
      uint strRef = uint.TryParse(twoDimEntry("Name"), out strRef) ? strRef : 0;
      string name;
      if (strRef == 0)
        name = "Nom manquant";
      else
        name = ItemPropertyDefinition2da.tlkTable.GetSimpleString(strRef);

      string subTypeResRef = twoDimEntry("SubTypeResRef");

      entries.Add((ItemPropertyType)rowIndex, new Entry(name, subTypeResRef));
    }
    public readonly struct Entry
    {
      public readonly string name;
      public readonly string subTypeResRef;

      public Entry(string name, string subTypeResRef)
      {
        this.name = name;
        this.subTypeResRef = subTypeResRef;
      }
    }
  }

  [ServiceBinding(typeof(ItemPropertyDefinition2da))]
  public class ItemPropertyDefinition2da
  {
    public static TlkTable tlkTable;
    public static IpDefinitionTable ipDefinitionTable;
    public ItemPropertyDefinition2da(TwoDimArrayFactory twoDimArrayFactory, TlkTable tlkService)
    {
      tlkTable = tlkService;
      ipDefinitionTable = twoDimArrayFactory.Get2DA<IpDefinitionTable>("itempropdef");
    }
  }
}
