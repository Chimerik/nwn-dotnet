using System.Collections.Generic;
using Anvil.API;
using NWN.Core;
using Anvil.Services;

namespace NWN.Systems
{
  public class IpAbilityTable : ITwoDimArray
  {
    private readonly Dictionary<int, Entry> entries = new Dictionary<int, Entry>();

    public Entry GetIPAbilityDataEntry(int rowIndex)
    {
      return entries[rowIndex];
    }
    public string GetAbilityName(int rowIndex)
    {
      return entries[rowIndex].name;
    }
    void ITwoDimArray.DeserializeRow(int rowIndex, TwoDimEntry twoDimEntry)
    {
      uint strRef = uint.TryParse(twoDimEntry("Name"), out strRef) ? strRef : 0;
      string name = strRef == 0 ? name = "Nom manquant" : name = ItemPropertyAbility2da.tlkTable.GetSimpleString(strRef);

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

  [ServiceBinding(typeof(ItemPropertyAbility2da))]
  public class ItemPropertyAbility2da
  {
    public static TlkTable tlkTable;
    public static IpAbilityTable ipAbilityTable;
    public ItemPropertyAbility2da(TwoDimArrayFactory twoDimArrayFactory, TlkTable tlkService)
    {
      tlkTable = tlkService;
      ipAbilityTable = twoDimArrayFactory.Get2DA<IpAbilityTable>("iprp_abilities");
    }
  }
}
