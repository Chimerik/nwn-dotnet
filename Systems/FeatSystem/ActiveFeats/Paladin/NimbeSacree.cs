using Anvil.API;

namespace NWN.Systems
{
  public partial class FeatSystem
  {
    private static void NimbeSacree(NwCreature caster)
    {
      caster.ApplyEffect(EffectDuration.Temporary, EffectSystem.NimbeSacree, NwTimeSpan.FromRounds(10));
      caster.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpSuperHeroism));

      StringUtils.DisplayStringToAllPlayersNearTarget(caster, $"{caster.Name.ColorString(ColorConstants.Cyan)} Nimbe Sacrée", StringUtils.gold, true, true);

      caster.SetFeatRemainingUses((Feat)CustomSkill.DevotionNimbeSacree, 0);
    }
  }
}
