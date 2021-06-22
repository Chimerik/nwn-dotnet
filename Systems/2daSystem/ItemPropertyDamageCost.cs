using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NWN.Services;

namespace NWN.Systems
{
  public class ExpTable : ITwoDimArray
  {
    private readonly List<int> entries = new List<int>();

    public int GetDamageCostValueFromRank(int rank)
    {
      return entries.IndexOf(rank) + 1;
    }

    void ITwoDimArray.DeserializeRow(int rowIndex, TwoDimEntry twoDimEntry)
    {
      // Use twoDimEntry(columnName) to get your serialized data, then convert it here.
      if(int.TryParse(twoDimEntry("Rank"), out int rank))
      entries.Add(rank);
    }
  }

  [ServiceBinding(typeof(ItemPropertyDamageCost))]
  public class ItemPropertyDamageCost
  {
    public static ExpTable ipDamageCost;
    public ItemPropertyDamageCost(TwoDimArrayFactory twoDimArrayFactory)
    {
      ipDamageCost = twoDimArrayFactory.Get2DA<ExpTable>("iprp_damagecost");
      //PlayerSystem.Log.Info($"row : {ipDamageCost.GetDamageCostValueFromRank(8)}");
    }
  }
}
