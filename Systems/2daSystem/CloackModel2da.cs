using System;
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
      label = entry.GetString("LABEL");
    }
  }

  [ServiceBinding(typeof(CloakModel2da))]
  public class CloakModel2da
  {
    //public static CloakModelTable cloakModelTable;
    private readonly TwoDimArray<CloakModelsEntry> cloakModelTable = NwGameTables.GetTable<CloakModelsEntry>("cloakmodel.2da");
    public static readonly List<NuiComboEntry> combo = new();
    public CloakModel2da()
    {
      foreach(var entry in cloakModelTable)
        if(!string.IsNullOrEmpty(entry.label))
          combo.Add(new NuiComboEntry(entry.label, entry.RowIndex));
    }
  }
}
