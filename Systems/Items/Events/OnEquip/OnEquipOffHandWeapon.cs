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

      if (oCreature is null || oItem is null || onEquip.Slot != InventorySlot.LeftHand || ItemUtils.IsLightWeapon(oItem.BaseItem, oCreature.Size))
        return;

      switch(oItem.BaseItem.ItemType)
      {
        case BaseItemType.SmallShield:
        case BaseItemType.LargeShield:
        case BaseItemType.TowerShield:
          return;
      }

      if (oCreature.KnowsFeat(Feat.Ambidexterity) && ItemUtils.IsHeavyWeapon(oItem.BaseItem.ItemType))
      {
        oCreature.ControllingPlayer.SendServerMessage($"{StringUtils.ToWhitecolor(oItem.Name)} est une arme loude et ne peut être équipée dans la main gauche malgré votre maîtrise ambidextre", ColorConstants.Red);
        onEquip.PreventEquip = true;
        return;
      }

      oCreature.ControllingPlayer.SendServerMessage($"{StringUtils.ToWhitecolor(oItem.Name)} n'est pas une arme légère et ne peut être équipée dans la main gauche", ColorConstants.Red);
      onEquip.PreventEquip = true;
      return;
    }
  }
}
