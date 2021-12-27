using System.Collections.Generic;
using Anvil.API;
using Anvil.Services;

namespace NWN.Systems
{
  public class AmbientMusicTable : ITwoDimArray
  {
    public readonly Dictionary<int, Entry> entries = new Dictionary<int, Entry>();

    void ITwoDimArray.DeserializeRow(int rowIndex, TwoDimEntry twoDimEntry)
    {
      int gender = int.TryParse(twoDimEntry("Gender"), out gender) ? gender : -1;

      if (gender < 0)
        return;

      string name = twoDimEntry("DisplayName");

      entries.Add(rowIndex, new Entry(rowIndex, name, (Gender)gender));
    }
    public readonly struct Entry
    {
      public readonly int id;
      public readonly string name;
      public readonly Gender gender;

      public Entry(int id, string name, Gender gender)
      {
        this.id = id;
        this.name = name;
        this.gender = gender;
      }
    }
  }

  [ServiceBinding(typeof(AmbientMusic2da))]
  public class AmbientMusic2da
  {
    public static AmbientMusicTable ambientMusicTable;
    public AmbientMusic2da(TwoDimArrayFactory twoDimArrayFactory, TlkTable tlkService)
    {
      ambientMusicTable = twoDimArrayFactory.Get2DA<AmbientMusicTable>("ambientmusic");
    }
  }
}
