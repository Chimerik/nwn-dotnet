using System;
using System.Collections.Generic;

namespace NWN.Systems
{
  public static partial class PlaceableSystem
  {
    public static Dictionary<string, Func<uint, int>> Register = new Dictionary<string, Func<uint, int>>
    {
      { "ench_bsn_onclose", EnchantmentBasinSystem.HandleClose },
      { "event_refinery_add_item_before", HandleBeforeItemAddedToRefinery },
      { "refinery_add_item", HandleItemAddedToRefinery },
      { "refinery_close", HandleRefineryClose },
    };
  }
}
