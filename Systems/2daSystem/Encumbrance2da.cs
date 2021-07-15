using System.Collections.Generic;
using Anvil.API;
using Anvil.Services;

namespace NWN.Systems
{
  public class EncumbranceTable : ITwoDimArray
  {
    private readonly Dictionary<int, Entry> entries = new Dictionary<int, Entry>();

    public Entry GetDataEntry(int feat)
    {
      return entries[feat];
    }

    void ITwoDimArray.DeserializeRow(int rowIndex, TwoDimEntry twoDimEntry)
    {
      int heavy = int.TryParse(twoDimEntry("Heavy"), out heavy) ? heavy : 0;
      int normal = int.TryParse(twoDimEntry("Normal"), out normal) ? normal : 0;

      entries.Add(rowIndex, new Entry(heavy, normal));
    }
    public readonly struct Entry
    {
      public readonly int heavy;
      public readonly int normal;

      public Entry(int heavy, int normal)
      {
        this.heavy = heavy;
        this.normal = normal;
      }
    }
  }

  [ServiceBinding(typeof(Encumbrance2da))]
  public class Encumbrance2da
  {
    public static EncumbranceTable encumbranceTable;
    public Encumbrance2da(TwoDimArrayFactory twoDimArrayFactory)
    {
      encumbranceTable = twoDimArrayFactory.Get2DA<EncumbranceTable>("encumbrance");
    }
  }
}
