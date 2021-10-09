using System.Collections.Generic;
using System.Linq;

using Anvil.API;
using Anvil.Services;

namespace NWN.Systems
{
  public class TorsoPartsTable : ITwoDimArray
  {
    private readonly Dictionary<int, Entry> entries = new Dictionary<int, Entry>();

    public List<NuiComboEntry> GetValidChestAppearancesForGenderAndAC(Gender gender, int AC)
    {
      List<NuiComboEntry> combo = new List<NuiComboEntry>();

      foreach (var entry in entries.Where(e => (e.Value.gender == gender || e.Value.gender == Gender.Both) && e.Value.AC == AC))
        combo.Add(new NuiComboEntry(entry.Key.ToString(), entry.Key));

      return combo;
    }

    void ITwoDimArray.DeserializeRow(int rowIndex, TwoDimEntry twoDimEntry)
    {
      int AC = float.TryParse(twoDimEntry("ACBONUS"), out float floatAC) ? (int)floatAC : -1;

      if (AC < 0)
        return;

      int genderValue = int.TryParse(twoDimEntry("GENDER"), out genderValue) ? genderValue : 2;

      entries.Add(rowIndex, new Entry(AC, (Gender)genderValue));
    }
    public readonly struct Entry
    {
 
      public readonly int AC;
      public readonly Gender gender;

      public Entry(int AC, Gender gender)
      {
        this.AC = AC;
        this.gender = gender;
      }
    }
  }

  [ServiceBinding(typeof(TorsoParts2da))]
  public class TorsoParts2da
  {
    public static TorsoPartsTable torsoPartsTable;
    public TorsoParts2da(TwoDimArrayFactory twoDimArrayFactory)
    {
      torsoPartsTable = twoDimArrayFactory.Get2DA<TorsoPartsTable>("parts_chest");
    }
  }
}
