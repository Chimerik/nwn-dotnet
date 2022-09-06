using System.Collections.Generic;
using System.Linq;

using Anvil.API;
using Anvil.Services;
using NWN.Native.API;

namespace NWN.Systems
{
  public sealed class LoadScreenEntry : ITwoDimArrayEntry
  {
    public string resRef { get; private set; }

    // RowIndex is already populated externally, and we do not need to assign it in InterpretEntry.
    public int RowIndex { get; init; }

    public void InterpretEntry(TwoDimArrayEntry entry)
    {
      resRef = entry.GetString("BMPResRef");
    }
  }

  [ServiceBinding(typeof(LoadScreen2da))]
  public class LoadScreen2da
  {
    private readonly TwoDimArray<LoadScreenEntry> ambientMusicTable = NwGameTables.GetTable<LoadScreenEntry>("loadscreens.2da");
    public static Dictionary<int, string> loadScreenResRef;

    public LoadScreen2da()
    {
      foreach(var entry in ambientMusicTable)
        if(!string.IsNullOrEmpty(entry.resRef))
          loadScreenResRef.Add(entry.RowIndex, entry.resRef);
    }
  }
}
