using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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

      name = string.IsNullOrEmpty(entry.GetString("DisplayName")) ? string.IsNullOrEmpty(entry.GetStrRef("Description").ToString()) ? entry.GetString("Resource") : entry.GetStrRef("Description").ToString() : Encoding.UTF8.GetString(Encoding.GetEncoding("iso-8859-1").GetBytes(entry.GetString("DisplayName")));
    }
  }

    [ServiceBinding(typeof(AmbientMusic2da))]
    public class AmbientMusic2da
    {
      public static readonly TwoDimArray<AmbientMusicEntry> ambientMusicTable = NwGameTables.GetTable<AmbientMusicEntry>("ambientmusic.2da");
      public static IEnumerable<AmbientMusicEntry> maleAmbientMusicEntry;
      public static IEnumerable<AmbientMusicEntry> femaleAmbientMusicEntry;

      public AmbientMusic2da()
      {
        maleAmbientMusicEntry = ambientMusicTable.Where(m => m.gender == Gender.Male || m.gender == Gender.Both);
        femaleAmbientMusicEntry = ambientMusicTable.Where(m => m.gender == Gender.Female || m.gender == Gender.Both);
      }
    }
}
