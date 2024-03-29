﻿using Anvil.API;

namespace NWN.Systems
{
  public partial class FeatSystem
  {
    private static void Balayage(NwCreature caster)
    {
      FeatUtils.ClearPreviousManoeuvre(caster);

      NwItem weapon = caster.GetItemInSlot(InventorySlot.RightHand);

      if (weapon is not null && weapon.BaseItem.NumDamageDice > 0 && !weapon.IsRangedWeapon)
      {
        caster.GetObjectVariable<LocalVariableInt>(CreatureUtils.ManoeuvreBalayageTargetVariable).Value = 1;
        FeatUtils.DecrementManoeuvre(caster);
      }
      else
        caster?.LoginPlayer.SendServerMessage("Veuillez vous équiper d'une arme de mêlée", ColorConstants.Red);
    }
  }
}
