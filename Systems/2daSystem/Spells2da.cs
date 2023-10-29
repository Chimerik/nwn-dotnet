using Anvil.API;
using Anvil.Services;

namespace NWN.Systems
{
  public sealed class SpellEntry : ITwoDimArrayEntry
  {
    public int RowIndex { get; init; }
    public StrRef tlkEntry { get; private set; }
    public string googleDocId { get; private set; }
    public int aoESize { get; private set; }
    public DamageType damageType { get; private set; }
    public int damageDice { get; private set; }
    public int numDice { get; private set; }
    public VfxType damageVFX { get; private set; }
    public int duration { get; private set; }
    public int bonusAction { get; private set; }
    public bool requiresConcentration { get; private set; }
    public Ability savingThrowAbility { get; private set; }

    public void InterpretEntry(TwoDimArrayEntry entry)
    {
      aoESize = entry.GetInt("TargetSizeX").GetValueOrDefault(0);
      damageType = (DamageType)entry.GetInt("DamageType").GetValueOrDefault(4096);
      damageVFX = (VfxType)entry.GetInt("VfxType").GetValueOrDefault(-1);
      damageDice = entry.GetInt("DamageDice").GetValueOrDefault(0);
      numDice = entry.GetInt("NumDice").GetValueOrDefault(0);
      duration = entry.GetInt("Duration").GetValueOrDefault(0);
      bonusAction = entry.GetInt("ActionType").GetValueOrDefault(0);
      savingThrowAbility = (Ability)entry.GetInt("SavingThrow").GetValueOrDefault(-1);
      requiresConcentration = entry.GetInt("UseConcentration").GetValueOrDefault(0) == 2;

      StrRef nameEntry = entry.GetStrRef("Name").GetValueOrDefault(StrRef.FromCustomTlk(0));

      if (nameEntry.Id > 0 && string.IsNullOrEmpty(nameEntry.ToString()))
        nameEntry.Override = StringUtils.ConvertToUTF8(entry.GetString("Label"));

      googleDocId = entry.GetString("Description");

      if(!string.IsNullOrEmpty(googleDocId))
        tlkEntry = entry.GetStrRef("SpellDesc").GetValueOrDefault(StrRef.FromCustomTlk(0));
    }
  }

  [ServiceBinding(typeof(Spells2da))]
  public class Spells2da
  {
    public static readonly TwoDimArray<SpellEntry> spellTable = NwGameTables.GetTable<SpellEntry>("spells.2da");

    public Spells2da()
    {
      Utils.mageCanTripList.Add(new NuiComboEntry("Haut-Elfe : Sélection d'un tour de magie", -1));

      foreach (var entry in spellTable)
      {
        NwSpell spell = NwSpell.FromSpellId(entry.RowIndex);
        if(spell.GetSpellLevelForClass(NwClass.FromClassType(ClassType.Wizard)) == 0)
          Utils.mageCanTripList.Add(new NuiComboEntry(spell.Name.ToString(), spell.Id));
      }

      SpellUtils.UpdateSpellDescriptionTable();
    }
  }
}
