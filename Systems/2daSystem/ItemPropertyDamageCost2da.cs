using System.Collections.Generic;
using Anvil.Services;

namespace NWN.Systems
{
  public class ItemPropertyRankDamageTable : ITwoDimArray
  {
    private readonly List<int> entries = new List<int>();
    private readonly Dictionary<int, Entry> entryList = new Dictionary<int, Entry>();

    public int GetDamageCostValueFromRank(int rank)
    {
      return entries.IndexOf(rank) + 1;
    }
    public int GetRankFromCostValue(int row)
    {
      return entries[row];
    }

    public string GetLabelFromIPCostTableValue(int cost)
    {
      return entryList[cost].label;
    }

    void ITwoDimArray.DeserializeRow(int rowIndex, TwoDimEntry twoDimEntry)
    {
      if (int.TryParse(twoDimEntry("Rank"), out int rank))
      {
        entries.Add(rank);
        entryList.Add(rowIndex, new Entry(twoDimEntry("Label")));
      }
    }
    public readonly struct Entry
    {
      public readonly string label;

      public Entry(string label)
      {
        this.label = label;
      }
    }

  }

  [ServiceBinding(typeof(ItemPropertyDamageCost2da))]
  public class ItemPropertyDamageCost2da
  {
    public static ItemPropertyRankDamageTable ipDamageCost;
    public ItemPropertyDamageCost2da(TwoDimArrayFactory twoDimArrayFactory)
    {
      ipDamageCost = twoDimArrayFactory.Get2DA<ItemPropertyRankDamageTable>("iprp_damagecost");
    }
  }
}
