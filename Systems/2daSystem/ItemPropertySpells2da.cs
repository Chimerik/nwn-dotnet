﻿using System.Collections.Generic;
using Anvil.API;
using Anvil.Services;

namespace NWN.Systems
{
  public class ipSpellsTable : ITwoDimArray
  {
    private readonly Dictionary<int, Entry> entries = new Dictionary<int, Entry>();

    public Entry GetSpellDataEntry(int row)
    {
      return entries[row];
    }
    void ITwoDimArray.DeserializeRow(int rowIndex, TwoDimEntry twoDimEntry)
    {
      int spell = int.TryParse(twoDimEntry("SpellIndex"), out spell) ? spell : 0;
      float innateLevel = float.TryParse(twoDimEntry("InnateLvl"), out innateLevel) ? innateLevel : 0;
      string icon = twoDimEntry("Icon");
      entries.Add(rowIndex, new Entry((Spell)spell, (byte)innateLevel, icon));
    }
    public readonly struct Entry
    {
      public readonly Spell spell;
      public readonly byte innateLevel;
      public readonly string icon;

      public Entry(Spell spell, byte innateLevel, string icon)
      {
        this.spell = spell;
        this.innateLevel = innateLevel;
        this.icon = icon;
      }
    }
  }

  [ServiceBinding(typeof(ItemPropertySpells2da))]
  public class ItemPropertySpells2da
  {
    public static TlkTable tlkTable;
    public static ipSpellsTable spellsTable;
    public ItemPropertySpells2da(TwoDimArrayFactory twoDimArrayFactory, TlkTable tlkService)
    {
      tlkTable = tlkService;
      spellsTable = twoDimArrayFactory.Get2DA<ipSpellsTable>("iprp_spells");
    }
  }
}
