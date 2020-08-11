using System;
using System.Collections.Generic;

namespace NWN.Systems
{
  public static partial class PlaceableSystem
  {
    public static Dictionary<string, Func<uint, int>> Register = new Dictionary<string, Func<uint, int>>
    {
      { "enchantment_basin_on_close", EnchantmentBasin.HandleClose },
    };
  }
}
