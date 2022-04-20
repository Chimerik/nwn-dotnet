using System.Collections.Generic;
using Anvil.API;
using Anvil.Services;

namespace NWN.Systems
{
  public sealed class ItemPropertyOnHitCostEntry : ITwoDimArrayEntry
  {
    public string name { get; private set; }
    public int RowIndex { get; init; }

    public void InterpretEntry(TwoDimArrayEntry entry)
    {
      name = entry.GetStrRef("Name").ToString();
    }
  }

  [ServiceBinding(typeof(ItemPropertyOnHitCost2da))]
  public class ItemPropertyOnHitCost2da
  {
    public static readonly TwoDimArray<ItemPropertyOnHitCostEntry> ipOnHitCostTable = new("iprp_onhitcost.2da");
    public ItemPropertyOnHitCost2da()
    {

    }
  }
}
