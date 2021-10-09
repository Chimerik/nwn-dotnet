using System.Collections.Generic;
using System.Linq;

using Anvil.API;
using Anvil.Services;

namespace NWN.Systems
{
  public class NeckPartsTable : ITwoDimArray
  {
    private readonly List<int> entries = new List<int>();

    public List<NuiComboEntry> GetValidNeckAppearances()
    {
      List<NuiComboEntry> combo = new List<NuiComboEntry>();

      foreach (var entry in entries)
        combo.Add(new NuiComboEntry(entry.ToString(), entry));

      return combo;
    }

    void ITwoDimArray.DeserializeRow(int rowIndex, TwoDimEntry twoDimEntry)
    {
      int maxAC = float.TryParse(twoDimEntry("ACBONUS"), out float floatAC) ? (int)floatAC : -1;

      if (maxAC < 0)
        return;

      entries.Add(rowIndex);
    }
  }

  [ServiceBinding(typeof(NeckParts2da))]
  public class NeckParts2da
  {
    public static NeckPartsTable neckPartsTable;
    public NeckParts2da(TwoDimArrayFactory twoDimArrayFactory)
    {
      neckPartsTable = twoDimArrayFactory.Get2DA<NeckPartsTable>("parts_neck");
    }
  }
}
