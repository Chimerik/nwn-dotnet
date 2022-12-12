using Anvil.API;
using Anvil.Services;

namespace NWN.Systems
{
  public sealed class SpellEntry : ITwoDimArrayEntry
  {
    public int RowIndex { get; init; }

    public void InterpretEntry(TwoDimArrayEntry entry)
    {
      if (RowIndex < 840)
        return;

      if(!entry.GetStrRef("Name").HasValue)
      {
        Utils.LogMessageToDMs($"SPELL SYSTEM - {RowIndex} - N'a pas de nom TLK défini");
        return;
      }

      if (!entry.GetStrRef("SpellDesc").HasValue)
      {
        Utils.LogMessageToDMs($"SPELL SYSTEM - {RowIndex} - N'a pas de description TLK défini");
        return;
      }

      StrRef tlkEntry = entry.GetStrRef("Name").Value;
      tlkEntry.Override = StringUtils.ConvertToUTF8(entry.GetString("Label"));
      tlkEntry = entry.GetStrRef("SpellDesc").Value;
      tlkEntry.Override = StringUtils.ConvertToUTF8(entry.GetString("Description"));
    }
  }

  [ServiceBinding(typeof(Spells2da))]
  public class Spells2da
  {
    public static readonly TwoDimArray<SpellEntry> spellTable = NwGameTables.GetTable<SpellEntry>("spells.2da");

    public Spells2da()
    {
      foreach (var entry in spellTable) ;
    }
  }
}
