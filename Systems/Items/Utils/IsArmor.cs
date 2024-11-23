using Anvil.API;

namespace NWN.Systems
{
  public static partial class ItemUtils
  {
    public static bool IsArmor(NwItem item)
    {
      return item is not null && item.BaseItem.ItemType == BaseItemType.Armor && item.BaseACValue > 0;
    }
  }
}
