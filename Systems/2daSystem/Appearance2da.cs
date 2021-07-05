using System.Collections.Generic;
using NWN.API;
using NWN.Services;

namespace NWN.Systems
{
  public class AppearanceTable : ITwoDimArray
  {
    private readonly Dictionary<int, Entry> entries = new Dictionary<int, Entry>();

    public Entry GetAppearanceDataEntry(int type)
    {
      return entries[type];
    }
    public string GetName(int type)
    {
      return entries[type].name;
    }
    public string GetPortrait(int type)
    {
      return entries[type].portrait;
    }

    void ITwoDimArray.DeserializeRow(int rowIndex, TwoDimEntry twoDimEntry)
    {
      uint strRef = uint.TryParse(twoDimEntry("STRING_REF"), out strRef) ? strRef : 0;
      string name;
      if (strRef == 0)
        name = "Créature";
      else
        name = Appearance2da.tlkTable.GetSimpleString(strRef);

      string portrait = twoDimEntry("PORTRAIT");
      if (portrait == "****")
        portrait = "po_hu_m_99";

      entries.Add(rowIndex, new Entry(name, portrait));
    }
    public readonly struct Entry
    {
      public readonly string name;
      public readonly string portrait;

      public Entry(string name, string value)
      {
        this.name = name;
        this.portrait = value;
      }
    }
  }

  [ServiceBinding(typeof(Appearance2da))]
  public class Appearance2da
  {
    public static TlkTable tlkTable;
    public static AppearanceTable appearanceTable;
    public Appearance2da(TwoDimArrayFactory twoDimArrayFactory, TlkTable tlkService)
    {
      tlkTable = tlkService;
      appearanceTable = twoDimArrayFactory.Get2DA<AppearanceTable>("appearance");
    }
  }
}
