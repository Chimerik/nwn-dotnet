using Anvil.API;
using Anvil.Services;

namespace NWN.Systems
{
  public sealed class EncumbranceTableEntry : ITwoDimArrayEntry
  {
    public int heavy { get; private set; }
    public int normal { get; private set; }
    public int RowIndex { get; init; }

    public void InterpretEntry(TwoDimArrayEntry entry)
    {
      normal = entry.GetInt("Normal").GetValueOrDefault(0);
      heavy = entry.GetInt("Heavy").GetValueOrDefault(0);
    }
  }

  [ServiceBinding(typeof(Encumbrance2da))]
  public class Encumbrance2da
  {
    private static readonly TwoDimArray<EncumbranceTableEntry> encumbranceTable = NwGameTables.GetTable<EncumbranceTableEntry>("encumbrance.2da");

    public Encumbrance2da()
    {

    }

    public static bool IsCreatureHeavilyEncumbred(NwCreature creature)
    {
      return creature.TotalWeight > encumbranceTable[creature.GetAbilityScore(Ability.Strength)].heavy ? true : false;
    }
  }
}
