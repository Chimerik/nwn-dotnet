using System.Collections.Generic;
using System.Linq;

using Anvil.API;
using Anvil.Services;

namespace NWN.Systems
{
  public sealed class ForearmPartsEntry : ITwoDimArrayEntry
  {
    public int maxAC { get; private set; }
    public Gender gender { get; private set; }
    public int RowIndex { get; init; }

    public void InterpretEntry(TwoDimArrayEntry entry)
    {
      maxAC = (int)entry.GetFloat("ACBONUS").GetValueOrDefault(-1);
      gender = (Gender)entry.GetInt("GENDER").GetValueOrDefault(2);
    }
  }

  [ServiceBinding(typeof(ForearmParts2da))]
  public class ForearmParts2da
  {
    private readonly TwoDimArray<ForearmPartsEntry> bicepPartsTable = new("parts_forearm.2da");
    public static readonly List<NuiComboEntry> femaleCombo = new();
    public static readonly List<NuiComboEntry> maleCombo = new();
    public ForearmParts2da()
    {
      foreach (var entry in bicepPartsTable.Where(b => b.maxAC > -1 && (b.gender == Gender.Female || b.gender == Gender.Both)))
        femaleCombo.Add(new NuiComboEntry(entry.RowIndex.ToString(), entry.RowIndex));

      foreach (var entry in bicepPartsTable.Where(b => b.maxAC > -1 && (b.gender == Gender.Male || b.gender == Gender.Both)))
        femaleCombo.Add(new NuiComboEntry(entry.RowIndex.ToString(), entry.RowIndex));
    }
  }
}
