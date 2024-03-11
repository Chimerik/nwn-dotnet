using Anvil.API;

namespace NWN.Systems
{
  public partial class FeatSystem
  {
    private static void MonkStunStrike(NwCreature caster)
    {
      NwItem mainHandWeapon = caster.GetItemInSlot(InventorySlot.RightHand);
      
      if(mainHandWeapon is not null && mainHandWeapon.IsRangedWeapon)
      {
        caster.LoginPlayer?.SendServerMessage("Vous devez être à main nue ou équipé d'une arme de mêlée", ColorConstants.Red);
        return;
      }

      caster.OnCreatureAttack -= CreatureUtils.OnMonkStunStrike;
      caster.OnCreatureAttack += CreatureUtils.OnMonkStunStrike;

      FeatUtils.DecrementKi(caster);
    }
  }
}
