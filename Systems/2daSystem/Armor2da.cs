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
      string maxDex = twoDimEntry("DEXBONUS");
      string ACPenalty = twoDimEntry("ACCHECK");
      string arcaneFailure = twoDimEntry("ARCANEFAILURE%");

      entries.Add(rowIndex, new Entry(name, cost, workshop, craftResRef, maxDex, ACPenalty, arcaneFailure));
    }
    public readonly struct Entry
    {
      public readonly string name;
      public readonly int cost;
      public readonly string workshop;
      public readonly string craftResRef;
      public readonly string maxDex;
      public readonly string ACPenalty;
      public readonly string arcaneFailure;

      public Entry(string name, int cost, string workshop, string craftResRef, string maxDex, string ACPenalty, string arcaneFailure)
      {
        this.name = name;
        this.cost = cost;
        this.workshop = workshop;
        this.craftResRef = craftResRef;
        this.maxDex = maxDex;
        this.ACPenalty = ACPenalty;
        this.arcaneFailure = arcaneFailure;
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
