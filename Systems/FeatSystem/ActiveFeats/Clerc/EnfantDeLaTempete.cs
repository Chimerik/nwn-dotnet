using Anvil.API;

namespace NWN.Systems
{
  public partial class FeatSystem
  {
    private static void EnfantDeLaTempete(NwCreature caster)
    {
      if (caster.Area.IsAboveGround && !caster.Area.IsInterior)
      {
        StringUtils.DisplayStringToAllPlayersNearTarget(caster, $"{caster.Name.ColorString(ColorConstants.Cyan)} - Enfant de la Tempête", StringUtils.gold, true, true);
        caster.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpSuperHeroism));
        caster.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.FnfElectricExplosion));
      }
      else
      {
        caster.LoginPlayer?.SendServerMessage("Vous ne pouvez pas faire usage de cette capacité en intérieur ou sous terre", ColorConstants.Red);
      }
    }
  }
}
