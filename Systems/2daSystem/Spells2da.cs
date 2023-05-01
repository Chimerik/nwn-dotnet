using Anvil.API;
using Anvil.Services;

namespace NWN.Systems
{
  public sealed class SpellEntry : ITwoDimArrayEntry
  {
    public int RowIndex { get; init; }
    public int energyCost { get; private set; }
    public int cooldown { get; private set; }
    public int inscriptionSkill { get; private set; }

    public void InterpretEntry(TwoDimArrayEntry entry)
    {
      energyCost = entry.GetInt("ImmunityType").GetValueOrDefault(0);
      cooldown = entry.GetInt("ItemImmunity").GetValueOrDefault(0);
      inscriptionSkill = cooldown;

      StrRef tlkEntry = entry.GetStrRef("Name").GetValueOrDefault(StrRef.FromCustomTlk(0));

      if (tlkEntry.Id > 0 && string.IsNullOrEmpty(tlkEntry.ToString()))
        tlkEntry.Override = StringUtils.ConvertToUTF8(entry.GetString("Label"));

      tlkEntry = entry.GetStrRef("SpellDesc").GetValueOrDefault(StrRef.FromCustomTlk(0));

      if (tlkEntry.Id > 0 && string.IsNullOrEmpty(tlkEntry.ToString()))
        tlkEntry.Override = StringUtils.ConvertToUTF8(entry.GetString("Description")).Replace("$", "\n");
    }
  }

  [ServiceBinding(typeof(Spells2da))]
  public class Spells2da
  {
    public static readonly TwoDimArray<SpellEntry> spellTable = NwGameTables.GetTable<SpellEntry>("spells.2da");

    public Spells2da()
    {
      foreach (var entry in spellTable)
        SpellUtils.spellCostDictionary.Add(NwSpell.FromSpellId(entry.RowIndex), new int[] { entry.energyCost, entry.cooldown });
    }
  }
}
