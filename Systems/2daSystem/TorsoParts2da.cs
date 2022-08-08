using System;
using System.Collections.Generic;
using System.Linq;

using Anvil.API;
using Anvil.Services;

namespace NWN.Systems
{
  public sealed class TorsoPartsEntry : ITwoDimArrayEntry
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

  [ServiceBinding(typeof(TorsoParts2da))]
  public class TorsoParts2da
  {
    private readonly TwoDimArray<TorsoPartsEntry> torsoPartsTable = NwGameTables.GetTable<TorsoPartsEntry>("parts_chest.2da");
    public static readonly List<NuiComboEntry> femaleClothCombo = new();
    public static readonly List<NuiComboEntry> femalePaddedCombo = new();
    public static readonly List<NuiComboEntry> femaleLeatherCombo = new();
    public static readonly List<NuiComboEntry> femaleHideCombo = new();
    public static readonly List<NuiComboEntry> femaleChainCombo = new();
    public static readonly List<NuiComboEntry> femaleBreastplateCombo = new();
    public static readonly List<NuiComboEntry> femaleBandedCombo = new();
    public static readonly List<NuiComboEntry> femaleHalfplateCombo = new();
    public static readonly List<NuiComboEntry> femaleFullplateCombo = new();
    public static readonly List<NuiComboEntry> maleClothCombo = new();
    public static readonly List<NuiComboEntry> malePaddedCombo = new();
    public static readonly List<NuiComboEntry> maleLeatherCombo = new();
    public static readonly List<NuiComboEntry> maleHideCombo = new();
    public static readonly List<NuiComboEntry> maleChainCombo = new();
    public static readonly List<NuiComboEntry> maleBreastplateCombo = new();
    public static readonly List<NuiComboEntry> maleBandedCombo = new();
    public static readonly List<NuiComboEntry> maleHalfplateCombo = new();
    public static readonly List<NuiComboEntry> maleFullplateCombo = new();
    public TorsoParts2da()
    {
      foreach (var entry in torsoPartsTable)
      {
        switch(entry.maxAC)
        {
          case 0:

            if (entry.gender == Gender.Both)
            {
              femaleClothCombo.Add(new NuiComboEntry(entry.RowIndex.ToString(), entry.RowIndex));
              maleClothCombo.Add(new NuiComboEntry(entry.RowIndex.ToString(), entry.RowIndex));
            }
            else if (entry.gender == Gender.Female)
              femaleClothCombo.Add(new NuiComboEntry(entry.RowIndex.ToString(), entry.RowIndex));
            else
              maleClothCombo.Add(new NuiComboEntry(entry.RowIndex.ToString(), entry.RowIndex));

            break;

          case 1:

            if (entry.gender == Gender.Both)
            {
              femalePaddedCombo.Add(new NuiComboEntry(entry.RowIndex.ToString(), entry.RowIndex));
              malePaddedCombo.Add(new NuiComboEntry(entry.RowIndex.ToString(), entry.RowIndex));
            }
            else if (entry.gender == Gender.Female)
              femalePaddedCombo.Add(new NuiComboEntry(entry.RowIndex.ToString(), entry.RowIndex));
            else
              malePaddedCombo.Add(new NuiComboEntry(entry.RowIndex.ToString(), entry.RowIndex));

            break;

          case 2:

            if (entry.gender == Gender.Both)
            {
              femaleLeatherCombo.Add(new NuiComboEntry(entry.RowIndex.ToString(), entry.RowIndex));
              maleLeatherCombo.Add(new NuiComboEntry(entry.RowIndex.ToString(), entry.RowIndex));
            }
            else if (entry.gender == Gender.Female)
              femaleLeatherCombo.Add(new NuiComboEntry(entry.RowIndex.ToString(), entry.RowIndex));
            else
              maleLeatherCombo.Add(new NuiComboEntry(entry.RowIndex.ToString(), entry.RowIndex));

            break;

          case 3:

            if (entry.gender == Gender.Both)
            {
              femaleHideCombo.Add(new NuiComboEntry(entry.RowIndex.ToString(), entry.RowIndex));
              maleHideCombo.Add(new NuiComboEntry(entry.RowIndex.ToString(), entry.RowIndex));
            }
            else if (entry.gender == Gender.Female)
              femaleHideCombo.Add(new NuiComboEntry(entry.RowIndex.ToString(), entry.RowIndex));
            else
              maleHideCombo.Add(new NuiComboEntry(entry.RowIndex.ToString(), entry.RowIndex));

            break;

          case 4:

            if (entry.gender == Gender.Both)
            {
              femaleChainCombo.Add(new NuiComboEntry(entry.RowIndex.ToString(), entry.RowIndex));
              maleChainCombo.Add(new NuiComboEntry(entry.RowIndex.ToString(), entry.RowIndex));
            }
            else if (entry.gender == Gender.Female)
              femaleChainCombo.Add(new NuiComboEntry(entry.RowIndex.ToString(), entry.RowIndex));
            else
              maleChainCombo.Add(new NuiComboEntry(entry.RowIndex.ToString(), entry.RowIndex));

            break;

          case 5:

            if (entry.gender == Gender.Both)
            {
              femaleBreastplateCombo.Add(new NuiComboEntry(entry.RowIndex.ToString(), entry.RowIndex));
              maleBreastplateCombo.Add(new NuiComboEntry(entry.RowIndex.ToString(), entry.RowIndex));
            }
            else if (entry.gender == Gender.Female)
              femaleBreastplateCombo.Add(new NuiComboEntry(entry.RowIndex.ToString(), entry.RowIndex));
            else
              maleBreastplateCombo.Add(new NuiComboEntry(entry.RowIndex.ToString(), entry.RowIndex));

            break;

          case 6:

            if (entry.gender == Gender.Both)
            {
              femaleBandedCombo.Add(new NuiComboEntry(entry.RowIndex.ToString(), entry.RowIndex));
              maleBandedCombo.Add(new NuiComboEntry(entry.RowIndex.ToString(), entry.RowIndex));
            }
            else if (entry.gender == Gender.Female)
              femaleBandedCombo.Add(new NuiComboEntry(entry.RowIndex.ToString(), entry.RowIndex));
            else
              maleBandedCombo.Add(new NuiComboEntry(entry.RowIndex.ToString(), entry.RowIndex));

            break;

          case 7:

            if (entry.gender == Gender.Both)
            {
              femaleHalfplateCombo.Add(new NuiComboEntry(entry.RowIndex.ToString(), entry.RowIndex));
              maleHalfplateCombo.Add(new NuiComboEntry(entry.RowIndex.ToString(), entry.RowIndex));
            }
            else if (entry.gender == Gender.Female)
              femaleHalfplateCombo.Add(new NuiComboEntry(entry.RowIndex.ToString(), entry.RowIndex));
            else
              maleHalfplateCombo.Add(new NuiComboEntry(entry.RowIndex.ToString(), entry.RowIndex));

            break;

          case 8:

            if (entry.gender == Gender.Both)
            {
              femaleFullplateCombo.Add(new NuiComboEntry(entry.RowIndex.ToString(), entry.RowIndex));
              maleFullplateCombo.Add(new NuiComboEntry(entry.RowIndex.ToString(), entry.RowIndex));
            }
            else if (entry.gender == Gender.Female)
              femaleFullplateCombo.Add(new NuiComboEntry(entry.RowIndex.ToString(), entry.RowIndex));
            else
              maleFullplateCombo.Add(new NuiComboEntry(entry.RowIndex.ToString(), entry.RowIndex));

            break;
        }
      }
    }
  }
}
