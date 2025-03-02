using Anvil.API;

namespace NWN.Systems
{
  public partial class FeatSystem
  {
    private static void ChatimentOcculte(NwCreature caster)
    {
      NwItem mainWeapon = caster.GetItemInSlot(InventorySlot.RightHand);
      NwItem secondaryWeapon = caster.GetItemInSlot(InventorySlot.LeftHand);

      if ((mainWeapon is not null && mainWeapon.GetObjectVariable<LocalVariableObject<NwCreature>>(CreatureUtils.PacteDeLaLameVariable).Value == caster)
        || (secondaryWeapon is not null && secondaryWeapon.GetObjectVariable<LocalVariableObject<NwCreature>>(CreatureUtils.PacteDeLaLameVariable).Value == caster))
      {
        EffectUtils.ClearChatimentEffects(caster);
        caster.ApplyEffect(EffectDuration.Permanent, EffectSystem.ChatimentOcculte);
      }
      else
      {
        caster.LoginPlayer?.SendServerMessage("Vous devez être équipé de votre arme de pacte", ColorConstants.Red);
        return;
      }  
    }
  }
}
