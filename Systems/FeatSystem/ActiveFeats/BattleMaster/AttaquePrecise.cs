﻿using Anvil.API;

namespace NWN.Systems
{
  public partial class FeatSystem
  {
    private static void AttaquePrecise(NwCreature caster)
    {
      FeatUtils.ClearPreviousManoeuvre(caster);

      if (caster.GetItemInSlot(InventorySlot.RightHand)?.BaseItem.NumDamageDice > 0)
      {
        int warMasterLevel = caster.GetClassInfo(ClassType.Fighter).Level;
        int superiorityDice = warMasterLevel > 17 ? 12 : warMasterLevel > 9 ? 10 : 8;

        caster.GetObjectVariable<LocalVariableInt>(CreatureUtils.ManoeuvreTypeVariable).Value = CustomSkill.WarMasterAttaquePrecise;
        caster.GetObjectVariable<LocalVariableInt>(CreatureUtils.ManoeuvreDiceVariable).Value = superiorityDice;
        
        caster.LoginPlayer?.SendServerMessage("Vous préparez une attaque précise", ColorConstants.Orange);
        FeatUtils.DecrementManoeuvre(caster);
      }
      else
        caster?.LoginPlayer.SendServerMessage("Veuillez vous équiper d'une arme", ColorConstants.Red);
    }
  }
}
