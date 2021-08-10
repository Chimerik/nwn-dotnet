using System.Collections.Generic;

using Anvil.Services;

namespace NWN.Systems
{
  public class MonsterDamageCostTable : ITwoDimArray
  {
    private readonly Dictionary<int, Entry> entries = new Dictionary<int, Entry>();

    public int GetMaxDamage(int row)
    {
      Entry entry = entries[row];

      return entry.NumDice * entry.Die;
    }
    public Entry GetDataEntry(int row)
    {
      return entries[row];
    }
    void ITwoDimArray.DeserializeRow(int rowIndex, TwoDimEntry twoDimEntry)
    {
      int numDice = int.TryParse(twoDimEntry("numDice"), out numDice) ? numDice : 0;
      int die = int.TryParse(twoDimEntry("numDice"), out die) ? die : 0;
      entries.Add(rowIndex, new Entry(numDice, die));
    }
    public readonly struct Entry
    {
      public readonly int NumDice;
      public readonly int Die;

      public Entry(int numDice, int die)
      {
        this.NumDice = numDice;
        this.Die = die;
      }
    }
  }

  [ServiceBinding(typeof(MonsterDamageCost2da))]
  public class MonsterDamageCost2da
  {
    public static MonsterDamageCostTable monsterDamageCostTable;
    public MonsterDamageCost2da(TwoDimArrayFactory twoDimArrayFactory)
    {
      monsterDamageCostTable = twoDimArrayFactory.Get2DA<MonsterDamageCostTable>("iprp_monstcost");
    }
  }
}
