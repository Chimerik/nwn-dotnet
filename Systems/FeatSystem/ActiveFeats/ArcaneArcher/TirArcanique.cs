using System;
using Anvil.API;

namespace NWN.Systems
{
  public partial class FeatSystem
  {
    private static void TirArcanique(NwCreature caster, int tirId)
    {
      if (ItemUtils.HasBowEquipped(caster.GetItemInSlot(InventorySlot.RightHand)?.BaseItem.ItemType))
      {
        if (caster.GetObjectVariable<LocalVariableInt>(CreatureUtils.TirArcaniqueCooldownVariable).HasNothing)
        {
          caster.OnCreatureAttack += CreatureUtils.OnAttackTirArcanique;
          caster.GetObjectVariable<LocalVariableInt>(CreatureUtils.TirArcaniqueVariable).Value = tirId;
          caster.GetObjectVariable<LocalVariableInt>(CreatureUtils.TirArcaniqueVariable).Value = tirId;
          FeatUtils.DecrementTirArcanique(caster);
          StringUtils.DelayLocalVariableDeletion<LocalVariableInt>(caster, CreatureUtils.TirArcaniqueCooldownVariable, NwTimeSpan.FromRounds(1));
        }
        else
          caster.LoginPlayer?.SendServerMessage("Tir arcanique limité à un par round", ColorConstants.Red);
      }
      else
        caster.LoginPlayer?.SendServerMessage("Vous devez être équipé d'un arc", ColorConstants.Red);
    }
  }
}
