using Anvil.API;
using Anvil.API.Events;

namespace NWN.Systems
{
  public partial class ItemSystem
  {
    public static void CancelRuinedItemEquip(OnItemValidateEquip onItemValidateEquip)
    {
      NwItem item = onItemValidateEquip.Item;

      if (item.BaseItem.ItemType != BaseItemType.Armor && item.GetObjectVariable<LocalVariableInt>("_MAX_DURABILITY").HasValue 
        && item.GetObjectVariable<LocalVariableInt>("_DURABILITY") < 1)
      {
        onItemValidateEquip.Result = EquipValidationResult.Denied;
        onItemValidateEquip.UsedBy.ControllingPlayer.SendServerMessage($"{onItemValidateEquip.Item.Name} nécessite des réparations.", ColorConstants.Red);
      }
    }
  }
}
