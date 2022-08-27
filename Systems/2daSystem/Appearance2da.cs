using System;
using System.Collections.Generic;
using System.Linq;

using Anvil.API;
using Anvil.Services;

using NWN.Core;

namespace NWN.Systems
{
  public sealed class AppearanceEntry : ITwoDimArrayEntry
  {
    public string name { get; private set; }
    public string race { get; private set; }

    // RowIndex is already populated externally, and we do not need to assign it in InterpretEntry.
    public int RowIndex { get; init; }

    public void InterpretEntry(TwoDimArrayEntry entry)
    {
      name = entry.GetString("LABEL");
      race = entry.GetString("RACE");
    }
  }

  [ServiceBinding(typeof(Appearance2da))]
  public class Appearance2da
  {
    private readonly TwoDimArray<AppearanceEntry> appearanceTable = NwGameTables.GetTable<AppearanceEntry>("appearance.2da");
    public static List<NuiComboEntry> appearanceEntries = new();
    private readonly Dictionary<string, List<string>> duplicates = new();

    public Appearance2da()
    {
      //int i = 0;
      foreach (var entry in appearanceTable)
      {
        if (!string.IsNullOrEmpty(entry.name))
          appearanceEntries.Add(new NuiComboEntry(entry.name, entry.RowIndex));
        else if (!string.IsNullOrEmpty(entry.race))
        {
          string file = NWScript.ResManGetAliasFor(entry.race.ToLower(), NWScript.RESTYPE_MDL);
          if (!duplicates.TryAdd(file, new List<string>() { entry.race }))
            duplicates[file].Add(entry.race);
        }
      }

      foreach(var file in duplicates)
      {
        Console.WriteLine("FILE : " + file.Key);

        foreach(var res in file.Value.OrderBy(n => n))
          Console.WriteLine(res);
      }
      
      appearanceEntries = appearanceEntries.OrderBy(a => a.Label).ToList();
      

      /*foreach (var entry in appearanceEntries)
      {
        Console.WriteLine($"{i} - {entry.Label} - {entry.Value}");
        i++;
      }*/
    }
  }
}
