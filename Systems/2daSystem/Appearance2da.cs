﻿using System.Collections.Generic;
using Anvil.API;
using Anvil.Services;

namespace NWN.Systems
{
  public class AppearanceTable : ITwoDimArray
  {
    private readonly Dictionary<AppearanceType, Entry> entries = new Dictionary<AppearanceType, Entry>();

    public Entry GetAppearanceDataEntry(AppearanceType type)
    {
      return entries[type];
    }
    void ITwoDimArray.DeserializeRow(int rowIndex, TwoDimEntry twoDimEntry)
    {
      uint strRef = uint.TryParse(twoDimEntry("STRING_REF"), out strRef) ? strRef : 0;
      string name = strRef == 0 ? name = "Créature" : name = Appearance2da.tlkTable.GetSimpleString(strRef);
      string portrait = twoDimEntry("PORTRAIT") == "****" ? portrait = "po_hu_m_99" : portrait = twoDimEntry("PORTRAIT");
      string race = twoDimEntry("RACE");

      entries.Add((AppearanceType)rowIndex, new Entry(name, portrait, race));
    }
    public readonly struct Entry
    {
      public readonly string name;
      public readonly string portrait;
      public readonly string race;

      public Entry(string name, string portrait, string race)
      {
        this.name = name;
        this.portrait = portrait;
        this.race = race;
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
