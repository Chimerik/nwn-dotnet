using Anvil.API;

namespace NWN.Systems
{
  public partial class FeatSystem
  {
    private static void MonkSlowFall(NwCreature caster)
    {
      if (CreatureUtils.HandleReactionUse(caster))
      {
        caster.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpPulseWind));
        StringUtils.DisplayStringToAllPlayersNearTarget(caster, "Chute contrôlée", StringUtils.gold, true);
      }
    }
  }
}
