using Anvil.API;
using Anvil.Services;

namespace NWN.Systems
{
  public sealed class FeatEntry : ITwoDimArrayEntry
  {
    public int RowIndex { get; init; }
    public int spellId { get; private set; }
    public SkillSystem.Category skillCategory { get; private set; }
    public StrRef descriptionTlkEntry { get; private set; }
    public StrRef nameTlkEntry { get; private set; }

    public void InterpretEntry(TwoDimArrayEntry entry)
    {
      nameTlkEntry = entry.GetStrRef("FEAT").GetValueOrDefault(StrRef.FromCustomTlk(0));
      descriptionTlkEntry = entry.GetStrRef("DESCRIPTION").GetValueOrDefault(StrRef.FromCustomTlk(0));
      spellId = entry.GetInt("SPELLID").GetValueOrDefault(-1);
      skillCategory = (SkillSystem.Category)entry.GetInt("EFFECTSSTACK").GetValueOrDefault(0);
    }
  }

  [ServiceBinding(typeof(Feats2da))]
  public class Feats2da
  {
    public static readonly TwoDimArray<FeatEntry> featTable = NwGameTables.GetTable<FeatEntry>("feat.2da");

    public Feats2da()
    {

    }
  }
}
