using Anvil.API;

namespace NWN.Systems
{
  public partial class FeatSystem
  {
    private static void AngeDeLaVengeance(NwCreature caster)
    {
      caster.ApplyEffect(EffectDuration.Temporary, EffectSystem.AngeDeLaVengeance, NwTimeSpan.FromRounds(100));
      caster.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpSuperHeroism));

      StringUtils.DisplayStringToAllPlayersNearTarget(caster, $"{caster.Name.ColorString(ColorConstants.Cyan)} - Ange de la Vengeance", StringUtils.gold, true, true);

      caster.SetFeatRemainingUses((Feat)CustomSkill.AngeDeLaVengeance, 0);
    }
  }
}
