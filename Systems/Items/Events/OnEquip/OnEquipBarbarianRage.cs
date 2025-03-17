using Anvil.API;
using Anvil.API.Events;

namespace NWN.Systems
{
  public partial class ItemSystem
  {
    public static void OnEquipBarbarianRage(OnItemEquip onEquip)
    {
      NwCreature oPC = onEquip.EquippedBy;
      NwItem oItem = onEquip.Item;

      if (oPC is null || oItem is null || onEquip.Slot != InventorySlot.Chest)
        return;

      if(oItem.BaseACValue > 4)
      {
        onEquip.PreventEquip = true;
        oPC.LoginPlayer?.SendServerMessage("Vous ne pouvez pas équiper d'armure lourde sous l'effet de rage", ColorConstants.Red);

        if(!ItemUtils.CheckArmorShieldProficiency(oPC, oPC.GetItemInSlot(InventorySlot.LeftHand)) && 
          !ItemUtils.CheckArmorShieldProficiency(oPC, oPC.GetItemInSlot(InventorySlot.Chest)))
          EffectUtils.RemoveTaggedEffect(oPC, EffectSystem.ShieldArmorDisadvantageEffectTag);
      }
    }
  }
}
