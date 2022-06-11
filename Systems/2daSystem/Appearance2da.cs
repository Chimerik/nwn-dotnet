using System;
using System.Collections.Generic;
using System.Linq;

using Anvil.API;
using Anvil.Services;

namespace NWN.Systems
{
  public sealed class AppearanceEntry : ITwoDimArrayEntry
  {
    public string name { get; private set; }

    // RowIndex is already populated externally, and we do not need to assign it in InterpretEntry.
    public int RowIndex { get; init; }

    public void InterpretEntry(TwoDimArrayEntry entry)
    {
      name = entry.GetString("LABEL");
    }
  }

  [ServiceBinding(typeof(Appearance2da))]
  public class Appearance2da
  {
    private readonly TwoDimArray<AppearanceEntry> appearanceTable = new("appearance.2da");
    public static List<NuiComboEntry> appearanceEntries = new();

    public Appearance2da()
    {
      int i = 0;
      foreach (var entry in appearanceTable)
        if (!string.IsNullOrEmpty(entry.name))
          appearanceEntries.Add(new NuiComboEntry(entry.name, entry.RowIndex));

      appearanceEntries = appearanceEntries.OrderBy(a => a.Label).ToList();

      /*foreach (var entry in appearanceEntries)
      {
        Console.WriteLine($"{i} - {entry.Label} - {entry.Value}");
        i++;
      }*/
    }
  }
}
