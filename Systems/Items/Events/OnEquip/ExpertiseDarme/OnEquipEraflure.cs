using Anvil.API;
using Anvil.API.Events;

namespace NWN.Systems
{
  public partial class ItemSystem
  {
    public static void OnEquipEraflure(ModuleEvents.OnPlayerEquipItem onEquip)
    {
      NwCreature oPC = onEquip.Player;
      NwItem oItem = onEquip.Item;
      //NwItem swappedItem = oPC.GetItemInSlot(onEquip.Slot);

      if (oPC is null || oItem is null || !ItemUtils.IsWeapon(oItem.BaseItem))
        return;

      var weapon = oPC.GetItemInSlot(InventorySlot.RightHand);
      var secondWeapon = oPC.GetItemInSlot(InventorySlot.LeftHand);

      if ((weapon is not null && ItemUtils.IsCreatureWeaponExpert(oPC, weapon) && Utils.In(weapon.BaseItem.ItemType, BaseItemType.LightFlail, BaseItemType.HeavyFlail, BaseItemType.Morningstar, BaseItemType.LightMace, BaseItemType.DireMace, BaseItemType.LightHammer, BaseItemType.Warhammer))
      || (secondWeapon is not null && ItemUtils.IsCreatureWeaponExpert(oPC, secondWeapon) && Utils.In(weapon.BaseItem.ItemType, BaseItemType.LightFlail, BaseItemType.HeavyFlail, BaseItemType.Morningstar, BaseItemType.LightMace, BaseItemType.DireMace, BaseItemType.LightHammer, BaseItemType.Warhammer)))
      {
        oPC.OnCreatureAttack -= CreatureUtils.OnAttackEraflure;
        oPC.OnCreatureAttack += CreatureUtils.OnAttackEraflure;
      }
      else
        oPC.OnCreatureAttack -= CreatureUtils.OnAttackEraflure;
    }
  }
}
