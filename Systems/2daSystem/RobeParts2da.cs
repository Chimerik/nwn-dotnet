using System.Collections.Generic;
using System.Linq;

using Anvil.API;
using Anvil.Services;

namespace NWN.Systems
{
  public class RobePartsTable : ITwoDimArray
  {
    private readonly Dictionary<int, Entry> entries = new Dictionary<int, Entry>();

    public List<NuiComboEntry> GetValidRobeAppearancesForGender(Gender gender)
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

      int minAC = int.TryParse(twoDimEntry("COSTMODIFIER"), out minAC) ? minAC : 0;
      int genderValue = int.TryParse(twoDimEntry("GENDER"), out genderValue) ? genderValue : 2;

      entries.Add(rowIndex, new Entry(minAC, maxAC, (Gender)genderValue));
    }
    public readonly struct Entry
    {
      public readonly int minAC;
      public readonly int maxAC;
      public readonly Gender gender;

      public Entry(int minAC, int maxAC, Gender gender)
      {
        this.minAC = minAC;
        this.maxAC = maxAC;
        this.gender = gender;
      }
    }
  }

  [ServiceBinding(typeof(RobeParts2da))]
  public class RobeParts2da
  {
    public static RobePartsTable robePartsTable;
    public RobeParts2da(TwoDimArrayFactory twoDimArrayFactory)
    {
      robePartsTable = twoDimArrayFactory.Get2DA<RobePartsTable>("parts_robe");
    }
  }
}
