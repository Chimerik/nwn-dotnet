using Anvil.API;

namespace NWN.Systems
{
  public partial class FeatSystem
  {
    private static void MonkBonusAttack(NwCreature caster)
    {
      NwItem mainHandWeapon = caster.GetItemInSlot(InventorySlot.RightHand);
      NwItem offHandWeapon = caster.GetItemInSlot(InventorySlot.LeftHand);

      if((mainHandWeapon is not null && !mainHandWeapon.BaseItem.IsMonkWeapon)
        || offHandWeapon is not null && !offHandWeapon.BaseItem.IsMonkWeapon)
      {
        caster.LoginPlayer?.SendServerMessage("Vous devez être à mains nues ou équipé d'une arme de moine", ColorConstants.Red);
        return;
      }

      if (!CreatureUtils.HandleBonusActionUse(caster))
        return;

      caster.GetObjectVariable<LocalVariableInt>(CreatureUtils.MonkBonusAttackVariable).Value = 1;
    }
  }
}
