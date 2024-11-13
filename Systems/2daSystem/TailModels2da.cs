using System.Collections.Generic;
using System.Linq;

using Anvil.API;
using Anvil.Services;

namespace NWN.Systems
{
  public sealed class TailModelsEntry : ITwoDimArrayEntry
  {
    public string label { get; private set; }
    public string model { get; private set; }
    public int RowIndex { get; init; }

    public void InterpretEntry(TwoDimArrayEntry entry)
    {
      label = entry.GetString("LABEL");
      model = entry.GetString("MODEL");
    }
  }

  [ServiceBinding(typeof(TailModels2da))]
  public class TailModels2da
  {
    private readonly TwoDimArray<TailModelsEntry> TailModelsTable = NwGameTables.GetTable<TailModelsEntry>("tailmodel.2da");
    public static readonly List<NuiComboEntry> tailCombo = new();
    public TailModels2da()
    {
      foreach (var entry in TailModelsTable)
        if (!string.IsNullOrEmpty(entry.label))
          tailCombo.Add(new NuiComboEntry(entry.label, entry.RowIndex));
    }
  }
}
