using System.Collections.Generic;
using System.Linq;

using Anvil.API;
using Anvil.Services;

namespace NWN.Systems
{
  public sealed class LegPartsEntry : ITwoDimArrayEntry
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

  [ServiceBinding(typeof(LegParts2da))]
  public class LegParts2da
  {
    private readonly TwoDimArray<LegPartsEntry> legPartsTable = new("parts_legs.2da");
    public static readonly List<NuiComboEntry> femaleCombo = new();
    public static readonly List<NuiComboEntry> maleCombo = new();
    public LegParts2da()
    {
      foreach (var entry in legPartsTable.Where(b => b.maxAC > -1 && (b.gender == Gender.Female || b.gender == Gender.Both)))
        femaleCombo.Add(new NuiComboEntry(entry.RowIndex.ToString(), entry.RowIndex));

      foreach (var entry in legPartsTable.Where(b => b.maxAC > -1 && (b.gender == Gender.Male || b.gender == Gender.Both)))
        femaleCombo.Add(new NuiComboEntry(entry.RowIndex.ToString(), entry.RowIndex));
    }
  }
}
