using Anvil.API;

namespace NWN.Systems
{
  public partial class FeatSystem
  {
    private static void ClercIllumination(NwCreature caster, NwGameObject oTarget)
    {
      if(oTarget is not NwCreature target)
      {
        caster.LoginPlayer?.SendServerMessage("Cible invalide", ColorConstants.Red);
        return;
      }

      target.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpSuperHeroism));
      EffectSystem.ApplyIlluminationProtectrice(caster, target); 
      caster.DecrementRemainingFeatUses((Feat)CustomSkill.ClercIllumination);

      StringUtils.DisplayStringToAllPlayersNearTarget(caster, $"{caster.Name.ColorString(ColorConstants.Cyan)} - Illumination Protectrice sur {target.Name.ColorString(ColorConstants.Cyan)}", StringUtils.gold, true, true);
    }
  }
}
