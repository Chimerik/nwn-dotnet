using System.Collections.Generic;
using System.Linq;

using Anvil.API;
using Anvil.Services;

namespace NWN.Systems
{
  public sealed class CloakModelsEntry : ITwoDimArrayEntry
  {
    public string label { get; private set; }
    public int RowIndex { get; init; }

    public void InterpretEntry(TwoDimArrayEntry entry)
    {
      int model = entry.GetInt("MODEL").GetValueOrDefault(-1);
      label = model > -1 ? entry.GetString("MODEL") : "";
    }
  }

  [ServiceBinding(typeof(CloakModel2da))]
  public class CloakModel2da
  {
    //public static CloakModelTable cloakModelTable;
    private readonly TwoDimArray<CloakModelsEntry> cloakModelTable = new("cloakmodel.2da");
    public static readonly List<NuiComboEntry> combo = new();
    public CloakModel2da()
    {
      foreach(var entry in cloakModelTable.Where(e => e.label.Length > 0))
        combo.Add(new NuiComboEntry(entry.label, entry.RowIndex));
    }
  }
}
