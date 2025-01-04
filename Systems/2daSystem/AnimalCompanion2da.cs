using Anvil.API;
using Anvil.Services;

namespace NWN.Systems
{
  public sealed class AnimalCompanionEntry : ITwoDimArrayEntry
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

  [ServiceBinding(typeof(AnimalCompanion2da))]
  public class AnimalCompanion2da
  {
    public static readonly TwoDimArray<AnimalCompanionEntry> companionTable = NwGameTables.GetTable<AnimalCompanionEntry>("hen_companion.2da");

    public AnimalCompanion2da(ModuleSystem moduleSystem)
    {
      foreach (var entry in companionTable) ;
    }
  }
}
