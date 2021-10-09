using System.Collections.Generic;
using System.Linq;

using Anvil.API;
using Anvil.Services;

namespace NWN.Systems
{
  public class BeltPartsTable : ITwoDimArray
  {
    private readonly List<int> entries = new List<int>();

    public List<NuiComboEntry> GetValidBeltAppearances()
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

  [ServiceBinding(typeof(BeltParts2da))]
  public class BeltParts2da
  {
    public static BeltPartsTable beltPartsTable;
    public BeltParts2da(TwoDimArrayFactory twoDimArrayFactory)
    {
      beltPartsTable = twoDimArrayFactory.Get2DA<BeltPartsTable>("parts_belt");
    }
  }
}
