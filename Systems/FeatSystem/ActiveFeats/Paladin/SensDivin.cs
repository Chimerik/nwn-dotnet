using Anvil.API;

namespace NWN.Systems
{
  public partial class FeatSystem
  {
    private static void SensDivin(NwCreature caster)
    {
      if (!CreatureUtils.HandleBonusActionUse(caster))
        return;

      caster.ApplyEffect(EffectDuration.Temporary, EffectSystem.SensDivin, NwTimeSpan.FromRounds(2));
      caster.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpSuperHeroism));

      StringUtils.DisplayStringToAllPlayersNearTarget(caster, "Sens Divin", StringUtils.gold, true);

      PaladinUtils.ConsumeOathCharge(caster);
    }
  }
}
