using Anvil.API;
using Anvil.API.Events;

namespace NWN.Systems
{
  public partial class ItemSystem
  {
    public static void OnUnEquipCancelIfInventoryFull(OnItemUnequip onUnequip)
    {
      NwCreature oPC = onUnequip.Creature;
      NwItem oItem = onUnequip.Item;

      if (oPC is null || oItem is null || oPC.Inventory.CheckFit(oItem))
        return;

      oPC.ControllingPlayer.SendServerMessage($"Inventaire est plein. Risque de perte de votre {StringUtils.ToWhitecolor(oItem.Name)} en déséquipant !", ColorConstants.Red);
      onUnequip.PreventUnequip = true;
    }
  }
}
