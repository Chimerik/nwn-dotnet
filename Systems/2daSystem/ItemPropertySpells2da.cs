using System.Collections.Generic;
using Anvil.API;
using Anvil.Services;

namespace NWN.Systems
{
  public sealed class ItemPropertySpellsEntry : ITwoDimArrayEntry
  {
    public Spell spell { get; private set; }
    public byte innateLevel { get; private set; }
    public string icon { get; private set; }
    public int RowIndex { get; init; }

    public void InterpretEntry(TwoDimArrayEntry entry)
    {
      spell = (Spell)entry.GetInt("SpellIndex").GetValueOrDefault(0);
      innateLevel = (byte)entry.GetFloat("InnateLvl").GetValueOrDefault(0);
      icon = entry.GetString("Icon");
    }
  }

  [ServiceBinding(typeof(ItemPropertySpells2da))]
  public class ItemPropertySpells2da
  {
    public static readonly TwoDimArray<ItemPropertySpellsEntry> ipSpellTable = NwGameTables.GetTable<ItemPropertySpellsEntry>("iprp_spells.2da");
    public ItemPropertySpells2da()
    {

    }
  }
}
