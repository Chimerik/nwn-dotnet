using System.Collections.Generic;
using System.Linq;

using Anvil.API;
using Anvil.Services;

namespace NWN.Systems
{
  public sealed class BeltPartsEntry : ITwoDimArrayEntry
  {
    public int maxAC { get; private set; }
    public int RowIndex { get; init; }

    public void InterpretEntry(TwoDimArrayEntry entry)
    {
      maxAC = (int)entry.GetFloat("ACBONUS").GetValueOrDefault(-1);
    }
  }

  [ServiceBinding(typeof(BeltParts2da))]
  public class BeltParts2da
  {
    private readonly TwoDimArray<BeltPartsEntry> BeltPartsTable = NwGameTables.GetTable<BeltPartsEntry>("parts_belt.2da");
    public static readonly List<NuiComboEntry> combo = new ();
    public BeltParts2da()
    {
      foreach(var entry in BeltPartsTable.Where(b => b.maxAC > -1))
        combo.Add(new NuiComboEntry(entry.RowIndex.ToString(), entry.RowIndex));
    }
  }
}
