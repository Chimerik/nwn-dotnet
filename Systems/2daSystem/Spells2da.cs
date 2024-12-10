using System.Collections.Generic;
using Anvil.API;
using Anvil.Services;

namespace NWN.Systems
{
  public sealed class SpellEntry : ITwoDimArrayEntry
  {
    public int RowIndex { get; init; }
    public StrRef tlkEntry { get; private set; }
    public string googleDocId { get; private set; }
    public float aoESize { get; private set; }
    public List<DamageType> damageType { get; private set; }
    public int damageDice { get; private set; }
    public int numDice { get; private set; }
    public VfxType damageVFX { get; private set; }
    public int duration { get; private set; }
    public bool isBonusAction { get; private set; }
    public bool requiresConcentration { get; private set; }
    public Ability savingThrowAbility { get; private set; }
    public bool requiresSomatic { get; private set; }
    public bool requiresVerbal { get; private set; }
    public bool bardMagicalSecret { get; private set; }
    public bool hideFromRanger { get; private set; }
    public bool hideFromWarlock { get; private set; }
    public bool ritualSpell { get; private set; }
    public int range { get; private set; }

    public void InterpretEntry(TwoDimArrayEntry entry)
    {
      aoESize = entry.GetFloat("TargetSizeX").GetValueOrDefault(0);
      //damageType = (DamageType)entry.GetInt("DamageType").GetValueOrDefault(4096);

      damageType = new();
      string damageTypeString = entry.GetString("DamageType");

      if (string.IsNullOrEmpty(damageTypeString))
        damageType.Add(DamageType.BaseWeapon);
      else
        foreach(var damage in damageTypeString.Split(";"))
          damageType.Add((DamageType)int.Parse(damage));

      damageVFX = (VfxType)entry.GetInt("VfxType").GetValueOrDefault(-1);
      damageDice = entry.GetInt("DamageDice").GetValueOrDefault(0);
      numDice = entry.GetInt("NumDice").GetValueOrDefault(0);
      duration = entry.GetInt("Duration").GetValueOrDefault(0);
      isBonusAction = entry.GetInt("ActionType").GetValueOrDefault(0).ToBool();
      savingThrowAbility = (Ability)entry.GetInt("SavingThrow").GetValueOrDefault(-1);
      requiresConcentration = entry.GetInt("UseConcentration").GetValueOrDefault(0) == 2;
      requiresSomatic = entry.GetString("VS")?.Contains('s') ?? false;
      requiresVerbal = entry.GetString("VS")?.Contains('v') ?? false;
      bardMagicalSecret = entry.GetBool("BardMagicalSecret").GetValueOrDefault(false);
      hideFromRanger = entry.GetBool("HideFromRanger").GetValueOrDefault(false);
      hideFromWarlock = entry.GetBool("HideFromWarlock").GetValueOrDefault(false);
      ritualSpell = entry.GetBool("Rituel").GetValueOrDefault(false);

      range = entry.GetString("Duration") switch
      {
        "S" => 81,
        "M" => 400,
        "L" => 1600,
        _ => 9,
      };

      StrRef nameEntry = entry.GetStrRef("Name").GetValueOrDefault(StrRef.FromCustomTlk(0));
      googleDocId = entry.GetString("Description");

      if (!string.IsNullOrEmpty(googleDocId))
      {
        nameEntry.Override = StringUtils.ConvertToUTF8(entry.GetString("Label"));
        tlkEntry = entry.GetStrRef("SpellDesc").GetValueOrDefault(StrRef.FromCustomTlk(0));
      }
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
        if (spell.GetSpellLevelForClass(ClassType.Wizard) == 0 && spell.FeatReference?.Id > 0)
          Utils.mageCanTripList.Add(new NuiComboEntry(spell.Name.ToString(), spell.FeatReference.Id));
      }

      SpellUtils.UpdateSpellDescriptionTable();
    }
  }
}
