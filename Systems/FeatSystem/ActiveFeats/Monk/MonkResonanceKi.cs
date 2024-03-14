using Anvil.API;

namespace NWN.Systems
{
  public partial class FeatSystem
  {
    private static void MonkResonanceKi(NwCreature caster)
    {
      if((caster.GetItemInSlot(InventorySlot.RightHand) is not null))
      {
        caster.LoginPlayer?.SendServerMessage("Cet attaque ne peut être utilisée qu'à mains nues", ColorConstants.Red);
        return;
      }

      if (!CreatureUtils.HandleBonusActionUse(caster))
        return;

      caster.GetObjectVariable<LocalVariableInt>(CreatureUtils.MonkBonusAttackVariable).Value = 1;

      caster.OnCreatureAttack -= CreatureUtils.OnAttackResonanceKi;
      caster.OnCreatureAttack += CreatureUtils.OnAttackResonanceKi;
    }
  }
}
