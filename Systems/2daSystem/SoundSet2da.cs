using System.Collections.Generic;
using System.Linq;

using Anvil.API;
using Anvil.Services;

namespace NWN.Systems
{
  public sealed class SoundSetEntry : ITwoDimArrayEntry
  {
    public string label { get; private set; }

    // RowIndex is already populated externally, and we do not need to assign it in InterpretEntry.
    public int RowIndex { get; init; }

    public void InterpretEntry(TwoDimArrayEntry entry)
    {
      label = entry.GetString("LABEL");
    }
  }

  [ServiceBinding(typeof(SoundSet2da))]
  public class SoundSet2da
  {
    public static readonly TwoDimArray<SoundSetEntry> ambientMusicTable = new("soundset.2da");

    public SoundSet2da()
    {

    }
  }
}
