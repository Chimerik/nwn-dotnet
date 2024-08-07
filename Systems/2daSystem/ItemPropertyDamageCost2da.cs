﻿using System.Collections.Generic;
using System.Linq;

using Anvil.API;
using Anvil.Services;

namespace NWN.Systems
{
  public sealed class ItemPropertyDamageCostEntry : ITwoDimArrayEntry
  {
    public string label { get; private set; }
    public int rank { get; private set; }
    public int die { get; private set; }
    public int numDice { get; private set; }

    // RowIndex is already populated externally, and we do not need to assign it in InterpretEntry.
    public int RowIndex { get; init; }

    public void InterpretEntry(TwoDimArrayEntry entry)
    {
      label = entry.GetString("Label");
      rank = entry.GetInt("Rank").GetValueOrDefault(0);
      die = entry.GetInt("Die").GetValueOrDefault(0);
      numDice = entry.GetInt("NumDice").GetValueOrDefault(0);
    }
  }

  [ServiceBinding(typeof(ItemPropertyDamageCost2da))]
  public class ItemPropertyDamageCost2da
  {
    public static readonly TwoDimArray<ItemPropertyDamageCostEntry> ipDamageCostTable = NwGameTables.GetTable<ItemPropertyDamageCostEntry>("iprp_damagecost.2da");
    public ItemPropertyDamageCost2da()
    {

    }
    public static int GetDamageCostValueFromRank(int rank)
    {
      return ipDamageCostTable.FirstOrDefault(d => d.rank == rank).RowIndex /*+ 1*/;
    }
    public static int GetRankFromCostValue(int row)
    {
      return ipDamageCostTable[row].rank;
    }
  }
}
