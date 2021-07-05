using System.Collections.Generic;
using NWN.API;
using NWN.Services;

namespace NWN.Systems
{
  public class AmbientMusicTable : ITwoDimArray
  {
    private readonly Dictionary<int, Entry> entries = new Dictionary<int, Entry>();

    public Entry GeAmbientMusicDataEntry(int row)
    {
      return entries[row];
    }
    public string GetName(int row)
    {
      return entries[row].name;
    }

    void ITwoDimArray.DeserializeRow(int rowIndex, TwoDimEntry twoDimEntry)
    {
      uint strRef = uint.TryParse(twoDimEntry("Description"), out strRef) ? strRef : 0;
      string name;
      if (strRef == 0)
        name = "Musique";
      else
        name = AmbientMusic2da.tlkTable.GetSimpleString(strRef);

      entries.Add(rowIndex, new Entry(name));
    }
    public readonly struct Entry
    {
      public readonly string name;

      public Entry(string name)
      {
        this.name = name;
      }
    }
  }

  [ServiceBinding(typeof(AmbientMusic2da))]
  public class AmbientMusic2da
  {
    public static TlkTable tlkTable;
    public static AmbientMusicTable ambientMusicTable;
    public AmbientMusic2da(TwoDimArrayFactory twoDimArrayFactory, TlkTable tlkService)
    {
      tlkTable = tlkService;
      ambientMusicTable = twoDimArrayFactory.Get2DA<AmbientMusicTable>("ambientmusic");
    }
  }
}
