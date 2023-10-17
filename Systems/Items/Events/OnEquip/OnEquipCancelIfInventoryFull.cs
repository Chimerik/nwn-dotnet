using Anvil.API;
using Anvil.API.Events;

namespace NWN.Systems
{
  public partial class ItemSystem
  {
    public static void OnEquipCancelIfInventoryFull(OnItemEquip onEquip)
    {
      NwCreature oPC = onEquip.EquippedBy;
      NwItem oItem = onEquip.Item;
      NwItem oUnequip = oPC.GetItemInSlot(onEquip.Slot);

      if (oPC is null || oItem is null || oUnequip is null || oPC.Inventory.CheckFit(oUnequip))
        return;

      oPC.ControllingPlayer.SendServerMessage($"Inventaire plein. Risque de perte de votre {StringUtils.ToWhitecolor(oUnequip.Name)} en déséquipant !", ColorConstants.Red);
      onEquip.PreventEquip = true;
    }
  }
}
