using Anvil.API;

namespace NWN.Systems
{
  public partial class FeatSystem
  {
    private static void PacteDeLaLameInvoquer(NwCreature caster)
    {
      NwItem pactWeapon = caster.GetObjectVariable<LocalVariableObject<NwItem>>(CreatureUtils.PacteDeLaLameVariable).Value;

      if (pactWeapon is null)
      {
        caster.LoginPlayer?.SendServerMessage("Vous n'êtes lié à aucune arme", ColorConstants.Red);
        return;
      }

      if (!CreatureUtils.HandleBonusActionUse(caster))
        return;

      caster.RunEquip(pactWeapon, EquipmentSlots.RightHand);
      caster.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpHeadOdd));
      StringUtils.DisplayStringToAllPlayersNearTarget(caster, $"{caster.Name.ColorString(ColorConstants.Cyan)} - Pacte de la Lame", StringUtils.gold, true, true);
    }
  }
}
