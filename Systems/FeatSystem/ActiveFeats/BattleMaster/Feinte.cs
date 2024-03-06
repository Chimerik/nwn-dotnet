﻿using Anvil.API;

namespace NWN.Systems
{
  public partial class FeatSystem
  {
    private static void Feinte(NwCreature caster)
    {
      FeatUtils.ClearPreviousManoeuvre(caster);

      if (!CreatureUtils.HandleBonusActionUse(caster))
        return;

      if (caster.GetItemInSlot(InventorySlot.RightHand)?.BaseItem.NumDamageDice > 0)
      {
        int warMasterLevel = caster.GetClassInfo(NwClass.FromClassId(CustomClass.Fighter)).Level;
        int superiorityDice = warMasterLevel > 17 ? 12 : warMasterLevel > 9 ? 10 : 8;

        caster.GetObjectVariable<LocalVariableInt>(CreatureUtils.ManoeuvreTypeVariable).Value = CustomSkill.WarMasterFeinte;
        caster.GetObjectVariable<LocalVariableInt>(CreatureUtils.ManoeuvreDiceVariable).Value = superiorityDice;

        FeatUtils.DecrementManoeuvre(caster);
      }
      else
        caster?.LoginPlayer.SendServerMessage("Veuillez vous équiper d'une arme", ColorConstants.Red);
    }
  }
}
