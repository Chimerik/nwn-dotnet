using System.Collections.Generic;
using System.Linq;

using Anvil.API;
using Anvil.Services;

namespace NWN.Systems
{
  public sealed class RobePartsEntry : ITwoDimArrayEntry
  {
    public int maxAC { get; private set; }
    public int minAC { get; private set; }
    public Gender gender { get; private set; }
    public int RowIndex { get; init; }

    public void InterpretEntry(TwoDimArrayEntry entry)
    {
      maxAC = (int)entry.GetFloat("ACBONUS").GetValueOrDefault(-1);
      gender = (Gender)entry.GetInt("GENDER").GetValueOrDefault(2);
      minAC = entry.GetInt("COSTMODIFIER").GetValueOrDefault(0);
    }
  }

  [ServiceBinding(typeof(RobeParts2da))]
  public class RobeParts2da
  {
    private readonly TwoDimArray<RobePartsEntry> robePartsTable = NwGameTables.GetTable<RobePartsEntry>("parts_robe.2da");
    public static readonly List<NuiComboEntry> femaleCombo = new();
    public static readonly List<NuiComboEntry> maleCombo = new();
    public RobeParts2da()
    {
      foreach (var entry in robePartsTable.Where(b => b.maxAC > -1 && (b.gender == Gender.Female || b.gender == Gender.Both)))
        femaleCombo.Add(new NuiComboEntry(entry.RowIndex.ToString(), entry.RowIndex));

      foreach (var entry in robePartsTable.Where(b => b.maxAC > -1 && (b.gender == Gender.Male || b.gender == Gender.Both)))
        maleCombo.Add(new NuiComboEntry(entry.RowIndex.ToString(), entry.RowIndex));
    }
  }
}
