﻿using Anvil.API;

namespace NWN.Systems
{
  public partial class FeatSystem
  {
    private static void AttaqueMenacante(NwCreature caster)
    {
      if (caster.GetItemInSlot(InventorySlot.RightHand)?.BaseItem.NumDamageDice > 0)
      {
        caster.OnCreatureAttack -= CreatureUtils.OnAttackMenacante;
        caster.OnCreatureAttack += CreatureUtils.OnAttackMenacante;

        int warMasterLevel = caster.GetClassInfo(NwClass.FromClassId(CustomClass.Warmaster)).Level;
        int superiorityDice = warMasterLevel > 9 ? warMasterLevel > 17 ? 10 : 12 : 8;

        caster.GetObjectVariable<LocalVariableInt>(CreatureUtils.ManoeuvreTypeVariable).Value = CustomSkill.WarMasterAttaqueMenacante;
        caster.GetObjectVariable<LocalVariableInt>(CreatureUtils.ManoeuvreDiceVariable).Value = superiorityDice;

        FeatUtils.DecrementManoeuvre(caster);
      }
      else
        caster?.LoginPlayer.SendServerMessage("Veuillez vous équiper d'une arme", ColorConstants.Red);
    }
  }
}
