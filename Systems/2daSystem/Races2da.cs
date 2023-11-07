
using System.Collections.Generic;
using Anvil.API;
using Anvil.Services;

namespace NWN.Systems
{
  public sealed class RaceEntry : ITwoDimArrayEntry
  {
    public int RowIndex { get; init; }
    public bool isPlayableRace { get; private set; }
    public string icon { get; private set; }
    public int appearanceId { get; private set; }
    public void InterpretEntry(TwoDimArrayEntry entry)
    {
      isPlayableRace = entry.GetBool("PlayableRace").GetValueOrDefault(false);
      icon = entry.GetString("Icon");
      appearanceId = entry.GetInt("Appearance").GetValueOrDefault(-1);
    }
  }

  [ServiceBinding(typeof(Races2da))]
  public class Races2da
  {
    public static readonly TwoDimArray<RaceEntry> raceTable = NwGameTables.GetTable<RaceEntry>("racialtypes.2da");
    public static readonly List<RaceEntry> playableRaces = new();

    public Races2da()
    {
      foreach (var entry in raceTable)
        if (entry.isPlayableRace)
          playableRaces.Add(entry);
    }
  }
}
