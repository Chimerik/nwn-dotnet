using Anvil.API;

namespace NWN.Systems
{
  public partial class FeatSystem
  {
    private static void EnfantDeLaTempete(NwCreature caster)
    {
      StringUtils.DisplayStringToAllPlayersNearTarget(caster, $"{caster.Name.ColorString(ColorConstants.Cyan)} - Enfant de la Tempête", StringUtils.gold, true, true);
      caster.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpSuperHeroism));
      caster.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.FnfElectricExplosion));
    }
  }
}
