using Anvil.API;

namespace NWN.Systems
{
  public partial class FeatSystem
  {
    private static void MonkPatience(NwCreature caster)
    {
      if (!CreatureUtils.HandleBonusActionUse(caster))
        return;

      caster.ApplyEffect(EffectDuration.Temporary, EffectSystem.MonkPatience, NwTimeSpan.FromRounds(1));

      StringUtils.DisplayStringToAllPlayersNearTarget(caster, "Patience", StringUtils.gold, true);
      FeatUtils.DecrementKi(caster);
    }
  }
}
