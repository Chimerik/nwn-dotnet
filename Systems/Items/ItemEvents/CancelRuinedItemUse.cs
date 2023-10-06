using Anvil.API;
using Anvil.API.Events;

namespace NWN.Systems
{
  public partial class ItemSystem
  {
    public static void CancelRuinedItemUse(OnItemValidateUse onItemValidateUse)
    {
      NwItem item = onItemValidateUse.Item;

      if (item.BaseItem.ItemType != BaseItemType.Armor && item.GetObjectVariable<LocalVariableInt>("_MAX_DURABILITY").HasValue 
        && item.GetObjectVariable<LocalVariableInt>("_DURABILITY") < 1)
        onItemValidateUse.CanUse = false;
    }
  }
}
