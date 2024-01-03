using Anvil.API;

namespace NWN.Systems
{
  public partial class FeatSystem
  {
    private static void TirArcanique(NwCreature caster, int tirId)
    {
      if (ItemUtils.HasBowEquipped(caster.GetItemInSlot(InventorySlot.RightHand)?.BaseItem.ItemType))
      {
        caster.OnCreatureAttack += CreatureUtils.OnAttackTirArcanique;
        caster.GetObjectVariable<LocalVariableInt>(CreatureUtils.TirArcaniqueVariable).Value = tirId;
        FeatUtils.DecrementTirArcanique(caster);
      }
      else
        caster.LoginPlayer?.SendServerMessage("Vous devez être équipé d'un arc", ColorConstants.Red);
    }
  }
}
