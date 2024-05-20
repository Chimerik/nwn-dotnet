using Anvil.API;

namespace NWN.Systems
{
  public partial class FeatSystem
  {
    private static void Volee(NwCreature caster)
    {
      NwItem weapon = caster.GetItemInSlot(InventorySlot.RightHand);

      if(weapon is null || !ItemUtils.IsWeapon(weapon.BaseItem))
      {
        caster.LoginPlayer?.SendServerMessage("Vous devez être équipé d'une arme afin de faire usage de cette ", ColorConstants.Red);
        return;
      }

      if (!CreatureUtils.HandleBonusActionUse(caster))
        return;

      caster.GetObjectVariable<LocalVariableInt>(CreatureUtils.HunterVoleeVariable).Value = 1;

      StringUtils.DisplayStringToAllPlayersNearTarget(caster, "Volée", StringUtils.gold, true, true);
    }
  }
}
