using System.Collections.Generic;
using NWN.API;
using NWN.API.Constants;
using NWN.Services;

namespace NWN.Systems
{
  public class PortraitsTable : ITwoDimArray
  {
    private readonly Dictionary<int, Entry> entries = new Dictionary<int, Entry>();

    public Entry GetDataEntry(int row)
    {
      return entries[row];
    }
    void ITwoDimArray.DeserializeRow(int rowIndex, TwoDimEntry twoDimEntry)
    {

      string resfRef = twoDimEntry("BaseResRef");

      entries.Add(rowIndex, new Entry(resfRef));
    }
    public readonly struct Entry
    {
      public readonly string resRef;

      public Entry(string resfRef)
      {
        this.resRef = resfRef;
      }
    }
  }

  [ServiceBinding(typeof(Portraits2da))]
  public class Portraits2da
  {
    public static PortraitsTable portraitsTable;
    public Portraits2da(TwoDimArrayFactory twoDimArrayFactory)
    {
      portraitsTable = twoDimArrayFactory.Get2DA<PortraitsTable>("portraits");
    }
  }
}
