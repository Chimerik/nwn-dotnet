using System;
using Anvil.API;

namespace NWN.Systems
{
  public partial class FeatSystem
  {
    private static void DonDuProtecteur(NwCreature caster, NwGameObject oTarget)
    {
      if (oTarget is not NwCreature target)
      {
        caster.LoginPlayer?.SendServerMessage("Cible invalide", ColorConstants.Red);
        return;
      }
      
      caster.DecrementRemainingFeatUses((Feat)CustomSkill.DonDuProtecteur);
      caster.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpDeathWard));
      caster.ApplyEffect(EffectDuration.Temporary, EffectSystem.ProtectionContreLaMort, TimeSpan.FromHours(8));
      StringUtils.DisplayStringToAllPlayersNearTarget(caster, $"{caster.Name.ColorString(ColorConstants.Cyan)} - inscrit le nom de {target.Name.ColorString(ColorConstants.Cyan)} au sein du livre des ombres", StringUtils.brightPurple, true, true);
    }
  }
}
