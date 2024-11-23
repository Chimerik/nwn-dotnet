using Anvil.API;

namespace NWN.Systems
{
  public static partial class ItemUtils
  {
    public static bool IsHeavyArmor(NwItem item)
    {
      return item is not null && item.BaseACValue > 5;
    }
  }
}
