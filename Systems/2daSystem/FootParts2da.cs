using System.Collections.Generic;
using System.Linq;

using Anvil.API;
using Anvil.Services;

namespace NWN.Systems
{
  public class FootPartsTable : ITwoDimArray
  {
    private readonly Dictionary<int, Entry> entries = new Dictionary<int, Entry>();

    public List<NuiComboEntry> GetValidFootAppearancesForGender(Gender gender)
    {
      List<NuiComboEntry> combo = new List<NuiComboEntry>();

      foreach (var entry in entries.Where(e => e.Value.gender == gender || e.Value.gender == Gender.Both))
        combo.Add(new NuiComboEntry(entry.Key.ToString(), entry.Key));

      return combo;
    }

    void ITwoDimArray.DeserializeRow(int rowIndex, TwoDimEntry twoDimEntry)
    {
      int maxAC = float.TryParse(twoDimEntry("ACBONUS"), out float floatAC) ? (int)floatAC : -1;

      if (maxAC < 0)
        return;

      int genderValue = int.TryParse(twoDimEntry("GENDER"), out genderValue) ? genderValue : 2;

      entries.Add(rowIndex, new Entry((Gender)genderValue));
    }
    public readonly struct Entry
    {
      public readonly Gender gender;

      public Entry(Gender gender)
      {
        this.gender = gender;
      }
    }
  }

  [ServiceBinding(typeof(FootParts2da))]
  public class FootParts2da
  {
    public static FootPartsTable footPartsTable;
    public FootParts2da(TwoDimArrayFactory twoDimArrayFactory)
    {
      footPartsTable = twoDimArrayFactory.Get2DA<FootPartsTable>("parts_foot");
    }
  }
}
