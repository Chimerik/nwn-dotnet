using Anvil.API;

namespace NWN.Systems
{
  public partial class FeatSystem
  {
    private static void HaloDeLumiere(NwCreature caster)
    {
      caster.ApplyEffect(EffectDuration.Temporary, EffectSystem.HaloDeLumiereAura, NwTimeSpan.FromRounds(10));
      caster.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpSuperHeroism));

      StringUtils.DisplayStringToAllPlayersNearTarget(caster, $"{caster.Name.ColorString(ColorConstants.Cyan)} - Halo de Lumière", StringUtils.gold, true, true);

      caster.SetFeatRemainingUses((Feat)CustomSkill.ClercHaloDeLumiere, 0);
    }
  }
}
