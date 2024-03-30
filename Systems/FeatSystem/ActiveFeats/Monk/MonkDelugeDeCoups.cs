using Anvil.API;

namespace NWN.Systems
{
  public partial class FeatSystem
  {
    private static void MonkDelugeDeCoups(NwCreature caster)
    {

      NwItem weapon = caster.GetItemInSlot(InventorySlot.RightHand);

      if(weapon is not null && weapon.BaseItem.IsRangedWeapon)
      {
        caster.LoginPlayer?.SendServerMessage("Action non utilisable avec une arme à distance", ColorConstants.Red);
        return;
      }

      if (!CreatureUtils.HandleBonusActionUse(caster))
        return;

      caster.GetObjectVariable<LocalVariableInt>(CreatureUtils.MonkDelugeVariable).Value = 1;

      if (caster.KnowsFeat((Feat)CustomSkill.MonkPaumeTechnique))
      {
        caster.OnCreatureAttack -= CreatureUtils.OnAttackPaumeTechnique;
        caster.OnCreatureAttack += CreatureUtils.OnAttackPaumeTechnique;

        caster.GetObjectVariable<LocalVariableInt>(CreatureUtils.MonkPaumeTechniqueVariable).Value = 2;
      }
    }
  }
}
