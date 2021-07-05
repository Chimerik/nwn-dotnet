using System.Collections.Generic;
using NWN.Services;

namespace NWN.Systems
{
  public class ItemPropertyRankDamageTable : ITwoDimArray
  {
    private readonly List<int> entries = new List<int>();

    public int GetDamageCostValueFromRank(int rank)
    {
      return entries.IndexOf(rank) + 1;
    }
    public int GetRankFromCostValue(int row)
    {
      return entries[row];
    }

    void ITwoDimArray.DeserializeRow(int rowIndex, TwoDimEntry twoDimEntry)
    {
      // Use twoDimEntry(columnName) to get your serialized data, then convert it here.
      if(int.TryParse(twoDimEntry("Rank"), out int rank))
      entries.Add(rank);
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
