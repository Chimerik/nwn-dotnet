using Anvil.API;

namespace NWN.Systems
{
  public partial class FeatSystem
  {
    private static void MonkPaumeVibratoire(NwCreature caster)
    {
      if((caster.GetItemInSlot(InventorySlot.RightHand) is not null))
      {
        caster.LoginPlayer?.SendServerMessage("Cet attaque ne peut être utilisée qu'à mains nues", ColorConstants.Red);
        return;
      }

      if (caster.GetFeatRemainingUses((Feat)CustomSkill.MonkPaumeVibratoire) < 3)
      {
        caster.LoginPlayer?.SendServerMessage("Cette attaque nécessite 3 charges de Ki", ColorConstants.Red);
        return;
      }

      caster.OnCreatureAttack -= CreatureUtils.OnAttackPaumeVibratoire;
      caster.OnCreatureAttack += CreatureUtils.OnAttackPaumeVibratoire;

      FeatUtils.DecrementKi(caster);
      FeatUtils.DecrementKi(caster);
      FeatUtils.DecrementKi(caster);
    }
  }
}
