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

      if (oCreature is null || oItem is null || onEquip.Slot != InventorySlot.LeftHand || !ItemUtils.IsWeapon(oItem.BaseItem))
        return;

      if (!ItemUtils.IsLightWeapon(oItem.BaseItem, oCreature.Size))
      {
        oCreature.ControllingPlayer.SendServerMessage($"{StringUtils.ToWhitecolor(oItem.Name)} n'est pas une arme Légère et n'est pas adaptée au combat ambidextre", ColorConstants.Red);
        onEquip.PreventEquip = true;
        return;
      }

      NwItem rightWeapon =  oCreature.GetItemInSlot(InventorySlot.RightHand);

      if (rightWeapon is null)
        return;

      if (ItemUtils.IsLightWeapon(rightWeapon.BaseItem, oCreature.Size))
        return;

      if (ItemUtils.IsHeavyWeapon(rightWeapon.BaseItem.ItemType))
      {
        oCreature.ControllingPlayer.SendServerMessage($"{StringUtils.ToWhitecolor(rightWeapon.Name)} est une arme Lourde et n'est pas adaptée au combat ambidextre", ColorConstants.Red);
        onEquip.PreventEquip = true;
        return;
      }
      
      if (!oCreature.KnowsFeat(Feat.Ambidexterity))
      {
        oCreature.ControllingPlayer.SendServerMessage($"{StringUtils.ToWhitecolor(rightWeapon.Name)} n'est pas une arme Légère et n'est pas adaptée au combat ambidextre", ColorConstants.Red);
        onEquip.PreventEquip = true;
      }
    }
  }
}
