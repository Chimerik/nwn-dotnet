using Anvil.API;
using Anvil.Services;

namespace NWN.Systems
{
  public sealed class MonsterDamageEntry : ITwoDimArrayEntry
  {
    public int numDice { get; private set; }
    public int Die { get; private set; }

    // RowIndex is already populated externally, and we do not need to assign it in InterpretEntry.
    public int RowIndex { get; init; }

    public void InterpretEntry(TwoDimArrayEntry entry)
    {
      numDice = entry.GetInt("NumDice").GetValueOrDefault(0);
      Die = entry.GetInt("Die").GetValueOrDefault(0);
    }
  }

  [ServiceBinding(typeof(MonsterDamageCost2da))]
  public class MonsterDamageCost2da
  {
    private static readonly TwoDimArray<MonsterDamageEntry> monsterDamageTable = new("iprp_monstcost.2da");
    public MonsterDamageCost2da()
    {

    }

    public static int GetMaxDamage(int row)
    {
      var entry = monsterDamageTable[row];
      return entry.numDice * entry.Die;
    }
  }
}
