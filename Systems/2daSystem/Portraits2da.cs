using System;
using System.Collections.Generic;

using Anvil.API;
using Anvil.Services;
using NWN.Core;

namespace NWN.Systems
{
  public sealed class PortraitEntry : ITwoDimArrayEntry
  {
    public string resRef { get; private set; }
    public string mediumPortrait { get; private set; }
    public string customPortrait { get; private set; }
    public string player { get; private set; }
    public int gender { get; private set; }
    public int racialType { get; private set; }

    // RowIndex is already populated externally, and we do not need to assign it in InterpretEntry.
    public int RowIndex { get; init; }

    public void InterpretEntry(TwoDimArrayEntry entry)
    {
      resRef = entry.GetString("BaseResRef");
      player = entry.GetString("Player");
      mediumPortrait = $"po_{resRef}m";
      customPortrait = $"po_{resRef}m2";
      gender = entry.GetInt("Sex").GetValueOrDefault(4);
      racialType = entry.GetInt("Race").GetValueOrDefault(28);
    }
  }

  [ServiceBinding(typeof(Portraits2da))]
  public class Portraits2da
  {
    public static readonly TwoDimArray<PortraitEntry> portraitsTable = NwGameTables.GetTable<PortraitEntry>("portraits.2da");
    public static readonly List<string>[,] portraitFilteredEntries = new List<string>[30, 5];
    public static readonly Dictionary<string, List<string>> playerCustomPortraits = new();
    public Portraits2da(ModuleSystem _)
    {
      ReloadPortraits();
    }
    public static async void ReloadPortraits()
    {
      await NwTask.SwitchToMainThread();

      //portraitEntries = portraitsTable.Where(p => !string.IsNullOrEmpty(p.resRef));
      foreach (var portrait in portraitsTable)
        if (!string.IsNullOrEmpty(portrait.resRef))
        {
          try
          {
            portraitFilteredEntries[portrait.racialType, portrait.gender].Add(portrait.mediumPortrait);
          }
          catch (Exception)
          {
            portraitFilteredEntries[portrait.racialType, portrait.gender] = new() { portrait.mediumPortrait };
          }

          if (!string.IsNullOrEmpty(portrait.player))
          {
            string portraitResRef = String.IsNullOrEmpty(NWScript.ResManGetAliasFor(portrait.customPortrait, (int)ResRefType.TGA)) ? portrait.mediumPortrait : portrait.customPortrait;

            if (!playerCustomPortraits.TryGetValue(portrait.player, out var value))
              playerCustomPortraits.Add(portrait.player, new List<string>() { portraitResRef });
            else
              value.Add(portraitResRef);
          }
        }
    }
  }
}
