using System.Collections.Generic;
using System.Linq;

using Anvil.API;
using Anvil.Services;

namespace NWN.Systems
{
  public sealed class NeckPartsEntry : ITwoDimArrayEntry
  {
    public int maxAC { get; private set; }
    public int RowIndex { get; init; }

    public void InterpretEntry(TwoDimArrayEntry entry)
    {
      maxAC = (int)entry.GetFloat("ACBONUS").GetValueOrDefault(-1);
    }
  }

  [ServiceBinding(typeof(NeckParts2da))]
  public class NeckParts2da
  {
    private readonly TwoDimArray<NeckPartsEntry> neckPartsTable = NwGameTables.GetTable<NeckPartsEntry>("parts_neck.2da");
    public static readonly List<NuiComboEntry> combo = new();
    public NeckParts2da()
    {
      foreach (var entry in neckPartsTable.Where(b => b.maxAC > -1))
        combo.Add(new NuiComboEntry(entry.RowIndex.ToString(), entry.RowIndex));
    }
  }
}
