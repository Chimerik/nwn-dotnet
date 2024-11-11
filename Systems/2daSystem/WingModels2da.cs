using System.Collections.Generic;
using System.Linq;

using Anvil.API;
using Anvil.Services;

namespace NWN.Systems
{
  public sealed class WingModelsEntry : ITwoDimArrayEntry
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

  [ServiceBinding(typeof(WingModels2da))]
  public class WingModels2da
  {
    private readonly TwoDimArray<WingModelsEntry> WingModelsTable = NwGameTables.GetTable<WingModelsEntry>("wingmodel.2da");
    public static readonly List<NuiComboEntry> wingCombo = new();
    public WingModels2da()
    {
      foreach (var entry in WingModelsTable.Where(b => !string.IsNullOrEmpty(b.model)))
        wingCombo.Add(new NuiComboEntry(entry.label, entry.RowIndex));
    }
  }
}
