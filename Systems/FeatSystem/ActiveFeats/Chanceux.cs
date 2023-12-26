using Anvil.API;

namespace NWN.Systems
{
  public partial class FeatSystem
  {
    private static void Chanceux(NwCreature caster)
    {
      caster.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpHolyAid));
      StringUtils.DisplayStringToAllPlayersNearTarget(caster, $"{caster.Name} utilise chanceux afin de bénéficier d'un avantage", StringUtils.gold, true);
      caster.DecrementRemainingFeatUses(NwFeat.FromFeatId(CustomSkill.Chanceux));      
    }
  }
}
