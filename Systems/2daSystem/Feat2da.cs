using System.Collections.Generic;
using NWN.API.Constants;
using NWN.Services;

namespace NWN.Systems
{
  public class FeatTable : ITwoDimArray
  {
    private readonly Dictionary<Feat, Entry> entries = new Dictionary<Feat, Entry>();

    public Entry GetFeatDataEntry(Feat feat)
    {
      return entries[feat];
    }

    void ITwoDimArray.DeserializeRow(int rowIndex, TwoDimEntry twoDimEntry)
    {
      int tlkName = int.Parse(twoDimEntry("FEAT"));
      int tlkDescription = int.Parse(twoDimEntry("DESCRIPTION"));
      entries.Add((Feat)rowIndex, new Entry(tlkName, tlkDescription));
    }
    public readonly struct Entry
    {
      public readonly int tlkName;
      public readonly int tlkDescription;
      //public readonly int XP;

      public Entry(int tlkName, int tlkDescription)
      {
        this.tlkName = tlkName;
        this.tlkDescription = tlkDescription;
      }
    }
  }

  [ServiceBinding(typeof(Feat2da))]
  public class Feat2da
  {
    public static FeatTable featTable;
    public Feat2da(TwoDimArrayFactory twoDimArrayFactory)
    {
      featTable = twoDimArrayFactory.Get2DA<FeatTable>("feat");
      //PlayerSystem.Log.Info($"row : {ipDamageCost.GetDamageCostValueFromRank(8)}");
    }
  }
}
