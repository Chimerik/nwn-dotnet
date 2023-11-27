using Anvil.API;
using Anvil.API.Events;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public void CancelSomaticSpellIfOffHandBusy(OnSpellAction onSpellAction)
    {
      NwItem offHand = onSpellAction.Caster.GetItemInSlot(InventorySlot.LeftHand);
      NwItem rightHand = onSpellAction.Caster.GetItemInSlot(InventorySlot.RightHand);

      if (rightHand is null || (offHand is null && !ItemUtils.IsTwoHandedWeapon(rightHand.BaseItem, onSpellAction.Caster.Size)))
        return;

      onSpellAction.PreventSpellCast = true;
      onSpellAction.Caster.LoginPlayer?.SendServerMessage($"Vous ne pouvez pas lancer de sort nécessitant une composante somatique en ayant vos deux mains occupées", ColorConstants.Red);
    }
  }
}
