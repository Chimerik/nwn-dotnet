using System.Collections.Generic;
using Anvil.API;
using Anvil.Services;

namespace NWN.Systems
{
  public class ArmorTable : ITwoDimArray
  {
    private readonly Dictionary<int, Entry> entries = new Dictionary<int, Entry>();

    public Entry GetDataEntry(int row)
    {
      return entries[row];
    }
    
    void ITwoDimArray.DeserializeRow(int rowIndex, TwoDimEntry twoDimEntry)
    {
      int cost = int.TryParse(twoDimEntry("COST"), out cost) ? cost : 1;
      uint strRef = uint.TryParse(twoDimEntry("NAME"), out strRef) ? strRef : 0;
      string name = strRef == 0 ? name = "Nom manquant" : name = Armor2da.tlkTable.GetSimpleString(strRef);
      string workshop = twoDimEntry("WORKSHOP");
      string craftResRef = twoDimEntry("CRAFTRESREF");

      entries.Add(rowIndex, new Entry(name, cost, workshop, craftResRef));
    }
    public readonly struct Entry
    {
      public readonly string name;
      public readonly int cost;
      public readonly string workshop;
      public readonly string craftResRef;

      public Entry(string name, int cost, string workshop, string craftResRef)
      {
        this.name = name;
        this.cost = cost;
        this.workshop = workshop;
        this.craftResRef = craftResRef;
      }
    }
  }

  [ServiceBinding(typeof(Armor2da))]
  public class Armor2da
  {
    public static TlkTable tlkTable;
    public static ArmorTable armorTable;
    public Armor2da(TwoDimArrayFactory twoDimArrayFactory, TlkTable tlkService)
    {
      tlkTable = tlkService;
      armorTable = twoDimArrayFactory.Get2DA<ArmorTable>("armor");
    }
  }
}
