using Anvil.API;
using Anvil.Services;

namespace NWN.Systems
{
  public sealed class FamiliarEntry : ITwoDimArrayEntry
  {
    public int RowIndex { get; init; }
    public int spellId { get; private set; }
    public string resRef { get; private set; }
    public void InterpretEntry(TwoDimArrayEntry entry)
    {
      spellId = entry.GetInt("SPELLID").GetValueOrDefault(0);
      resRef = entry.GetString("BASERESREF");
    }
  }

  [ServiceBinding(typeof(Familiars2da))]
  public class Familiars2da
  {
    public static readonly TwoDimArray<FamiliarEntry> familiarTable = NwGameTables.GetTable<FamiliarEntry>("hen_familiar.2da");

    public Familiars2da(ModuleSystem moduleSystem)
    {
      foreach (var entry in familiarTable) ;
    }
  }
}
