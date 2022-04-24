using System;
using System.Collections.Generic;
using System.Linq;

using Anvil.API;
using Anvil.Services;

namespace NWN.Systems
{
  public sealed class PortraitEntry : ITwoDimArrayEntry
  {
    public string resRef { get; private set; }
    public string mediumPortrait { get; private set; }
    public int gender { get; private set; }
    public int racialType { get; private set; }

    // RowIndex is already populated externally, and we do not need to assign it in InterpretEntry.
    public int RowIndex { get; init; }

    public void InterpretEntry(TwoDimArrayEntry entry)
    {
      resRef = entry.GetString("BaseResRef");
      mediumPortrait = $"po_{resRef}m";
      gender = entry.GetInt("Sex").GetValueOrDefault(4);
      racialType = entry.GetInt("Race").GetValueOrDefault(28);
    }
  }

  [ServiceBinding(typeof(Portraits2da))]
  public class Portraits2da
  {
    public static readonly TwoDimArray<PortraitEntry> portraitsTable = new("portraits.2da");
    //public static IEnumerable<PortraitEntry> portraitEntries;
    public static List<string>[,] portraitFilteredEntries;
    public Portraits2da(ModuleSystem module)
    {
      //portraitEntries = portraitsTable.Where(p => !string.IsNullOrEmpty(p.resRef));
      portraitFilteredEntries = new List<string>[30, 5];
      
      foreach (var portrait in portraitsTable)
        if (!string.IsNullOrEmpty(portrait.resRef))
          try { portraitFilteredEntries[portrait.racialType, portrait.gender].Add(portrait.mediumPortrait); }
          catch(Exception)
          {
            portraitFilteredEntries[portrait.racialType, portrait.gender] = new();
            portraitFilteredEntries[portrait.racialType, portrait.gender].Add(portrait.mediumPortrait);
          }    
    }
  }
}
