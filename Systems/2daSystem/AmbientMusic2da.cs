using System.Collections.Generic;
using Anvil.API;
using Anvil.Services;

namespace NWN.Systems
{
  public sealed class AmbientMusicEntry : ITwoDimArrayEntry
  {
    public string name { get; private set; }
    public Gender gender { get; private set; }

    // RowIndex is already populated externally, and we do not need to assign it in InterpretEntry.
    public int RowIndex { get; init; }

    public void InterpretEntry(TwoDimArrayEntry entry)
    {
      int genderValue = entry.GetInt("Gender").GetValueOrDefault(-1);

      if (gender < 0)
        return;

      gender = (Gender)genderValue;
      name = entry.GetString("DisplayName");
    }
  }

  [ServiceBinding(typeof(AmbientMusic2da))]
  public class AmbientMusic2da
  {
    public static readonly TwoDimArray<AmbientMusicEntry> ambientMusicTable = new ("ambientmusic.2da");
    
    public AmbientMusic2da()
    {
      
    }
  }
}
