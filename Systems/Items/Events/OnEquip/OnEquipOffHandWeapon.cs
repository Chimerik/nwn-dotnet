using Anvil.API;
using Anvil.API.Events;

namespace NWN.Systems
{
  public partial class ItemSystem
  {
    public static void OnEquipOffHandWeapon(OnItemEquip onEquip)
    {
      NwCreature oCreature = onEquip.EquippedBy;
      NwItem oItem = onEquip.Item;

      if (oCreature is null || oItem is null)
        return;

      if (onEquip.Slot == InventorySlot.LeftHand && !ItemUtils.IsLightWeapon(oItem.BaseItem, oCreature.Size))
      {
        oCreature.ControllingPlayer.SendServerMessage($"{StringUtils.ToWhitecolor(oItem.Name)} n'est pas une arme légère et ne peut être équipée dans la main gauche", ColorConstants.Red);
        onEquip.PreventEquip = true;
        return;
      }
    }
  }
}
