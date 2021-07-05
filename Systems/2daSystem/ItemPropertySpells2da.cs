using System.Collections.Generic;
using System.Linq;
using NWN.API;
using NWN.API.Constants;
using NWN.Services;

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
      entries.Add(rowIndex, new Entry((Spell)spell, (byte)innateLevel));
    }
    public readonly struct Entry
    {
      public readonly Spell spell;
      public readonly byte innateLevel;

      public Entry(Spell spell, byte innateLevel)
      {
        this.spell = spell;
        this.innateLevel = innateLevel;
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
