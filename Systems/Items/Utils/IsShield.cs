using Anvil.API;

namespace NWN.Systems
{
  public static partial class ItemUtils
  {
    public static bool IsShield(NwItem item)
    {
      if(item is null) return false;

      return Utils.In(item.BaseItem.ItemType, BaseItemType.SmallShield, BaseItemType.LargeShield, BaseItemType.TowerShield);
    }
  }
}
